using MediatR;
using FluentResults;
using PortfolioScheduler.Domain.DomainErrors;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Domain.Services.Interfaces;
using PortfolioScheduler.Application.Services.Interfaces;

namespace PortfolioScheduler.Application.Commands.ExecutePortfolioPurchase;

public class ExecutePortfolioPurchaseHandler : IRequestHandler<ExecutePortfolioPurchaseCommand, Result<ExecutePortfolioPurchaseResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProcessDistributionInBatch _batchDistribution;
    private readonly IPurchaseOrdersRepository _purchaseOrdersRepository;
    private readonly IPurchaseQuotesCalculator _purchaseQuotesCalculator;
    private readonly IRecommendedPortfolioRepository _recommendedPortfolioRepository;

    public ExecutePortfolioPurchaseHandler(
        ICustomerRepository customerRepository,
        IProcessDistributionInBatch batchDistribution,
        IPurchaseOrdersRepository purchaseOrdersRepository,
        IPurchaseQuotesCalculator purchaseQuotesCalculator,
        IRecommendedPortfolioRepository recommendedPortfolioRepository)
    {
        _batchDistribution = batchDistribution;
        _customerRepository = customerRepository;
        _purchaseQuotesCalculator = purchaseQuotesCalculator;
        _purchaseOrdersRepository = purchaseOrdersRepository;
        _recommendedPortfolioRepository = recommendedPortfolioRepository;
    }

    public async Task<Result<ExecutePortfolioPurchaseResponse>> Handle(ExecutePortfolioPurchaseCommand request, CancellationToken ct)
    {
        if (!request.LastCotahist.Any())
            return Result.Fail(CotahistErrors.AssetsPricesIsEmpty());

        var portfolio = await _recommendedPortfolioRepository.GetActivePortfolioAsync(ct);
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

        var distributeOrdersResult = await _batchDistribution.ProcessInBatchAsync(createPurchaseOrdersResult.Value, thirdValue, portfolio.Items.ToList(), masterAccount.BrokerageAccount, ct);
        if (distributeOrdersResult.IsFailed)
            return Result.Fail(distributeOrdersResult.Errors);

        return Result.Ok(ExecutePortfolioPurchaseResponse.BuildResponse(distributeOrdersResult.Value));
    }
}