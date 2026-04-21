using FluentResults;
using MediatR;
using PortfolioScheduler.Domain.DomainErrors;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Domain.Services.DTOs;
using PortfolioScheduler.Domain.Services.Interfaces;
using static PortfolioScheduler.Domain.Services.DTOs.DistributionResultDTO;

namespace PortfolioScheduler.Application.Commands.ExecutePortfolioPurchase;

public class ExecutePortfolioPurchaseHandler : IRequestHandler<ExecutePortfolioPurchaseCommand, Result<ExecutePortfolioPurchaseResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPortfolioDistribuition _portfolioDistribuition;
    private readonly IPurchaseOrdersRepository _purchaseOrdersRepository;
    private readonly IPurchaseQuotesCalculator _purchaseQuotesCalculator;
    private readonly IRecommendedPortfolioRepository _recommendedPortfolioRepository;

    public ExecutePortfolioPurchaseHandler(ICustomerRepository customerRepository, IPurchaseOrdersRepository purchaseOrdersRepository, IPurchaseQuotesCalculator quotesCalculator, IRecommendedPortfolioRepository recommendedPortfolioRepository, IPortfolioDistribuition portfolioDistribuition)
    {
        _customerRepository = customerRepository;
        _purchaseQuotesCalculator = quotesCalculator;
        _portfolioDistribuition = portfolioDistribuition;
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

        var masterAccount = await _customerRepository.GetByIdAsync(1, ct); // Master sempre vai ser o ID 1, criado na Migration Inicial
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

        var distributeOrdersResult = await ProcessDistributionInBatch(createPurchaseOrdersResult.Value, thirdValue, portfolio.Items.ToList(), masterAccount.BrokerageAccount, ct);
        if (distributeOrdersResult.IsFailed)
            return Result.Fail(distributeOrdersResult.Errors);

        await _purchaseOrdersRepository.AddAsync(createPurchaseOrdersResult.Value, ct);

        var resultSave = await _purchaseOrdersRepository.SaveAsync(ct);
        if (resultSave.IsFailed)
            return Result.Fail(resultSave.Errors);

        return Result.Ok(BuildResponse(distributeOrdersResult.Value));
    }

    private async Task<Result<DistributionResultDTO>> ProcessDistributionInBatch(IEnumerable<PurchaseOrder> purchasedAssets, decimal thirdValue, List<PortfolioItem> portfolio, BrokerageAccount masterAccount, CancellationToken ct)
    {
        var batchSize = 1000;
        var lastId = 0L;
        var responseDistribuitions = new List<DistributionResultDTO.Distribuitions>();
        var responsePurchaseOrdersPerAsset = new List<DistributionResultDTO.PurchaseOrdersPerAsset>();
        var residualFromMaster = new List<DistributionResultDTO.ResidualsFromMaster>();

        while (true)
        {
            var customersChunk = await _customerRepository.GetChunkOfCustomerAsync(batchSize, lastId, ct);
            var purchaseRoundData = new PurchaseRoundDataDTO(thirdValue, customersChunk);

            var resultDistribution = _portfolioDistribuition.Distribute(purchasedAssets, purchaseRoundData, portfolio, masterAccount);
            if (resultDistribution.IsFailed)
                return Result.Fail(resultDistribution.Errors);

            responseDistribuitions.AddRange(resultDistribution.Value.distribuitions);
            responsePurchaseOrdersPerAsset.AddRange(resultDistribution.Value.purchaseOrders);
            residualFromMaster.AddRange(resultDistribution.Value.ResidualsFromMasterAccount);

            if (customersChunk.Count < batchSize)
                break;

            lastId = customersChunk.Keys.Max();
        }

        return Result.Ok(new DistributionResultDTO(responsePurchaseOrdersPerAsset, responseDistribuitions, residualFromMaster));
    }

    private ExecutePortfolioPurchaseResponse BuildResponse(DistributionResultDTO result)
    {
        var response = new ExecutePortfolioPurchaseResponse(
            ExecutionDate: DateTime.UtcNow,
            TotalCustomers: result.distribuitions.Count,
            TotalPurchaseAmount: result.purchaseOrders.Sum(p => p.TotalPrice),
            PurchaseOrder: result.purchaseOrders,
            Distribuitions: result.distribuitions,
            ResidualsUsed: result.ResidualsFromMasterAccount,
            Message: $"Compra programada executada com sucesso para {result.distribuitions.Count} clientes.");

        return response;
    }
}