using FluentResults;
using MediatR;
using PortfolioScheduler.Application.Services.Interfaces;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Domain.Services.Interfaces;

namespace PortfolioScheduler.Application.Commands.RebalancePortfolio;

public class RebalancePortfolioHandler : IRequestHandler<RebalancePortfolioCommand, Result<RebalancePortfolioResponse>>
{
    private const int CHUNK_SIZE = 300;

    private readonly ICotahistParser _cotahistParser;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPortfolioRebalancer _portfolioRebalancer;
    private readonly IPurchaseQuotesCalculator _purchaseQuotesCalculator;
    private readonly IPurchaseOrdersRepository _purchaseOrdersRepository;
    private readonly IRecommendedPortfolioRepository _portfolioRepository;
    private readonly IProcessDistributionInBatch _processDistributionInBatch;
    private readonly IPortfolioRebalanceRepository _portfolioRebalanceRepository;

    public RebalancePortfolioHandler(
        ICotahistParser cotahistParser,
        ICustomerRepository customerRepository,
        IPortfolioRebalancer portfolioRebalancer,
        IPurchaseQuotesCalculator purchaseQuotesCalculator,
        IPurchaseOrdersRepository purchaseOrdersRepository,
        IRecommendedPortfolioRepository portfolioRepository,
        IProcessDistributionInBatch processDistributionInBatch,
        IPortfolioRebalanceRepository portfolioRebalanceRepository)
    {
        _cotahistParser = cotahistParser;
        _customerRepository = customerRepository;
        _portfolioRebalancer = portfolioRebalancer;
        _purchaseQuotesCalculator = purchaseQuotesCalculator;
        _purchaseOrdersRepository = purchaseOrdersRepository;
        _portfolioRepository = portfolioRepository;
        _processDistributionInBatch = processDistributionInBatch;
        _portfolioRebalanceRepository = portfolioRebalanceRepository;
    }

    public async Task<Result<RebalancePortfolioResponse>> Handle(RebalancePortfolioCommand request, CancellationToken ct)
    {
        var portfolio = await _portfolioRepository.GetActivePortfolioAsync(ct);
        var comparison = request.PortfolioComparison;
        if (!comparison.HasChanges())
            return Result.Ok(new RebalancePortfolioResponse());

        var allTickers =
            comparison.Removed.Select(r => r.Ticker)
            .Concat(comparison.Added.Select(a => a.Ticker))
            .Concat(comparison.Altered.Select(a => a.Ticker))
            .Concat(portfolio.Items.Select(i => i.Ticker))
            .ToHashSet();

        var cotacoes = _cotahistParser.GetTickerByLastCotahist(allTickers);
        var pricesByTicker = cotacoes.ToDictionary(q => q.Ticker, q => q.ClosePrice);

        var masterAccount = await _customerRepository.GetMasterAccount(ct);

        // Vendas das ações que foram Removidas ou Excedem a porcentagem da nova Cesta/Portfolio
        long lastId = 0;
        decimal totalSelledAmount = 0m;
        var allAuditRecords = new List<Domain.Entities.PortfolioRebalance>();

        while (true)
        {
            var batch = await _customerRepository.GetChunkOfCustomerAsync(CHUNK_SIZE, lastId, ct);
            if (batch == null || !batch.Any()) break;

            foreach (var customer in batch)
            {
                var custodies = customer.Value.CustomerCustodies.Custodies.ToDictionary(x => x.Ticker);

                var removedResult = _portfolioRebalancer.SellRemovedAssets(
                    custodies, masterAccount.BrokerageAccount,
                    comparison.Removed.Select(r => r.Ticker), pricesByTicker,
                    customer.Value.CustomerId);

                 var alteredResult = _portfolioRebalancer.SellExcessFromAlteredAssets(
                    custodies, masterAccount.BrokerageAccount,
                    comparison.Altered.Select(a => (a.Ticker, a.NewPercentage)), pricesByTicker,
                    customer.Value.CustomerId);

                totalSelledAmount += removedResult.TotalReleased + alteredResult.TotalReleased;
                allAuditRecords.AddRange(removedResult.AuditRecords);
                allAuditRecords.AddRange(alteredResult.AuditRecords);

                _portfolioRebalancer.ReplaceCustodyTickers(
                    custodies,
                    comparison.Removed.Select(r => r.Ticker),
                    comparison.Added.Select(a => a.Ticker));
            }

            var saveResult = await _customerRepository.SaveChangesAsync(ct);
            if (saveResult.IsFailed)
                return Result.Fail<RebalancePortfolioResponse>(saveResult.Errors);

            lastId = batch.Values.Max(c => c.CustomerId);
        }

        // Persistir registros de auditoria do rebalanceamento
        await _portfolioRebalanceRepository.AddRangeAsync(allAuditRecords, ct);

        // Zera custódias na master dos ativos envolvidos no rebalanceamento (removidos + alterados)
        var tickersToReset = comparison.Removed.Select(r => r.Ticker)
            .Concat(comparison.Altered.Select(a => a.Ticker))
            .ToHashSet();

        foreach (var ticker in tickersToReset)
        {
            var masterCustody = masterAccount.BrokerageAccount.Custodies
                .FirstOrDefault(c => c.Ticker == ticker);
            if (masterCustody != null)
                masterCustody.SellAllAssets();
        }

        // Compra e Distribuição (Mesma Lógica do Handler de ExecutionPurchase)

        var amountPerAsset = _purchaseQuotesCalculator.CalculateAmountPerAsset(totalSelledAmount, portfolio);

        var quantityResult = _purchaseQuotesCalculator.CalculateQuantityPerAsset(
            amountPerAsset, cotacoes, masterAccount.BrokerageAccount);
        if (quantityResult.IsFailed)
            return Result.Fail<RebalancePortfolioResponse>(quantityResult.Errors);

        var marketType = _purchaseQuotesCalculator.DefinePurchaseMarketType(quantityResult.Value);

        var ordersResult = PurchaseOrder.CreatePurchaseOrders(masterAccount.BrokerageAccount.Id, marketType.Values);
        if (ordersResult.IsFailed)
            return Result.Fail<RebalancePortfolioResponse>(ordersResult.Errors);

        await _purchaseOrdersRepository.AddAsync(ordersResult.Value, ct);
        var saveOrderResult = await _purchaseOrdersRepository.SaveAsync(ct);
        if (saveOrderResult.IsFailed)
            return Result.Fail<RebalancePortfolioResponse>(saveOrderResult.Errors);

        var distributeResult = await _processDistributionInBatch.ProcessInBatchAsync(
            ordersResult.Value, totalSelledAmount, portfolio.Items.ToList(),
            masterAccount.BrokerageAccount, ct);
        if (distributeResult.IsFailed)
            return Result.Fail<RebalancePortfolioResponse>(distributeResult.Errors);

        return Result.Ok(new RebalancePortfolioResponse());
    }
}



