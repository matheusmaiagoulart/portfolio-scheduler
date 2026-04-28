using MediatR;
using FluentResults;
using PortfolioScheduler.Domain.DomainErrors;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Domain.Services.DTOs;
using PortfolioScheduler.Domain.Services.Interfaces;

namespace PortfolioScheduler.Application.Commands.ExecutePortfolioPurchase;

public class ExecutePortfolioPurchaseHandler : IRequestHandler<ExecutePortfolioPurchaseCommand, Result<ExecutePortfolioPurchaseResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IPortfolioDistribution _portfolioDistribution;
    private readonly IPurchaseOrdersRepository _purchaseOrdersRepository;
    private readonly IPurchaseQuotesCalculator _purchaseQuotesCalculator;
    private readonly IRecommendedPortfolioRepository _recommendedPortfolioRepository;

    public ExecutePortfolioPurchaseHandler(ICustomerRepository customerRepository, IPurchaseOrdersRepository purchaseOrdersRepository, IPurchaseQuotesCalculator quotesCalculator, IRecommendedPortfolioRepository recommendedPortfolioRepository, IPortfolioDistribution portfolioDistribution, IDeliveryRepository deliveryRepository)
    {
        _customerRepository = customerRepository;
        _deliveryRepository = deliveryRepository;
        _purchaseQuotesCalculator = quotesCalculator;
        _portfolioDistribution = portfolioDistribution;
        _purchaseOrdersRepository = purchaseOrdersRepository;
        _recommendedPortfolioRepository = recommendedPortfolioRepository;
    }

    public async Task<Result<ExecutePortfolioPurchaseResponse>> Handle(ExecutePortfolioPurchaseCommand request, CancellationToken ct)
    {
        if (!request.LastCotahist.Any())
            return Result.Fail(CotahistErrors.AssetsPricesIsEmpty());

        var portfolio = await _recommendedPortfolioRepository.GetRecommendedPortfolioActive(ct);
        if (portfolio is null)
            return Result.Fail(PortfolioErrors.NoActiveRecommendedPortfolio());

        var masterAccount = await _customerRepository.GetMasterAccount(ct);
        if (masterAccount is null)
            return Result.Fail(BrokerageAccountErrors.NoMasterAccount());

        // 3.2. Agrupamento de Pedidos (1/3 de investimento de cada usuário ativo) e os IDs de cada usuário presente na compra
        var thirdValue = await _customerRepository.GetOneThirdAmountOfAllActiveCustomersAsync(ct);
        if (thirdValue <= 0)
            return Result.Fail(PurchaseErrors.PurchaseAmountInvalid(thirdValue));

        // 3.3 Definir a quantidade de cada ativo a ser comprado
        var amountPerAssetResult = _purchaseQuotesCalculator.CalculateAmountPerAsset(thirdValue, portfolio);

        var calculateQuantityPerAssetResult = _purchaseQuotesCalculator.CalculateQuantityPerAsset(amountPerAssetResult, request.LastCotahist, masterAccount.BrokerageAccount);
        if (calculateQuantityPerAssetResult.IsFailed)
            return Result.Fail(calculateQuantityPerAssetResult.Errors);

        // 3.4 Definir se é LOTE ou FRACIONÁRIO
        var definePurchaseMarketResult = _purchaseQuotesCalculator.DefinePurchaseMarketType(calculateQuantityPerAssetResult.Value);

        var createPurchaseOrdersResult = PurchaseOrder.CreatePurchaseOrders(masterAccount.BrokerageAccount.Id, definePurchaseMarketResult.Values);
        if (createPurchaseOrdersResult.IsFailed)
            return Result.Fail(createPurchaseOrdersResult.Errors);

        // Save before distrbution to generate sequencial id on DB. Transactions watched by transaction Behavior with Commit and Roolback allowed
        await _purchaseOrdersRepository.AddAsync(createPurchaseOrdersResult.Value, ct);
        var resultSave = await _purchaseOrdersRepository.SaveAsync(ct);
        if (resultSave.IsFailed)
            return Result.Fail(resultSave.Errors);

        var distributeOrdersResult = await ProcessDistributionInBatch(createPurchaseOrdersResult.Value, thirdValue, portfolio.Items.ToList(), masterAccount.BrokerageAccount, ct);
        if (distributeOrdersResult.IsFailed)
            return Result.Fail(distributeOrdersResult.Errors);

        return Result.Ok(BuildResponse(distributeOrdersResult.Value));
    }

    private async Task<Result<DistributionResultDTO>> ProcessDistributionInBatch(IEnumerable<PurchaseOrder> purchasedAssets, decimal thirdValue, List<PortfolioItem> portfolio, BrokerageAccount masterAccount, CancellationToken ct)
    {
        var batchSize = 1000;
        var lastId = 0L;
        var responseDistributions = new List<DistributionResultDTO.Distributions>();
        var responsePurchaseOrdersPerAsset = new List<DistributionResultDTO.PurchaseOrdersPerAsset>();
        var residualFromMaster = new List<DistributionResultDTO.ResidualsFromMaster>();
        var deliveries = new List<Delivery>();

        while (true)
        {
            var customersChunk = await _customerRepository.GetChunkOfCustomerAsync(batchSize, lastId, ct);
            var purchaseRoundData = new PurchaseRoundDataDTO(thirdValue, customersChunk);

            var resultDistribution = _portfolioDistribution.Distribute(purchasedAssets, purchaseRoundData, portfolio, masterAccount);
            if (resultDistribution.IsFailed)
                return Result.Fail(resultDistribution.Errors);

            responseDistributions.AddRange(resultDistribution.Value.distributions);
            responsePurchaseOrdersPerAsset.AddRange(resultDistribution.Value.purchaseOrders);
            residualFromMaster.AddRange(resultDistribution.Value.ResidualsFromMasterAccount);
            deliveries.AddRange(resultDistribution.Value.deliveries);
            
            await _deliveryRepository.AddAsync(resultDistribution.Value.deliveries, ct);

            if (customersChunk.Count < batchSize)
                break;

            lastId = customersChunk.Keys.Max();
        }

        var saveResult = await _deliveryRepository.SaveAsync(ct);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);

        return Result.Ok(new DistributionResultDTO(responsePurchaseOrdersPerAsset, responseDistributions, residualFromMaster, deliveries));
    }

    private ExecutePortfolioPurchaseResponse BuildResponse(DistributionResultDTO result)
    {
        var response = new ExecutePortfolioPurchaseResponse(
            ExecutionDate: DateTime.UtcNow,
            TotalCustomers: result.distributions.Count,
            TotalPurchaseAmount: result.purchaseOrders.Sum(p => p.TotalPrice),
            PurchaseOrder: result.purchaseOrders,
            Distributions: result.distributions,
            ResidualsUsed: result.ResidualsFromMasterAccount,
            Message: $"Compra programada executada com sucesso para {result.distributions.Count} clientes.");

        return response;
    }
}