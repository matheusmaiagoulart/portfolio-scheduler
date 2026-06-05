using FluentResults;
using PortfolioScheduler.Application.Services.Interfaces;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Domain.Services.DTOs;
using PortfolioScheduler.Domain.Services.Interfaces;

namespace PortfolioScheduler.Application.Services;

public class ProcessDistributionInBatch : IProcessDistributionInBatch
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IPortfolioDistribution _portfolioDistribution;

    public ProcessDistributionInBatch(
        ICustomerRepository customerRepository, 
        IDeliveryRepository deliveryRepository, 
        IPortfolioDistribution portfolioDistribution)
    {
        _customerRepository = customerRepository;
        _deliveryRepository = deliveryRepository;
        _portfolioDistribution = portfolioDistribution;
    }

    public async Task<Result<DistributionResultDTO>> ProcessInBatchAsync(IEnumerable<PurchaseOrder> purchasedAssets, decimal thirdValue, List<PortfolioItem> portfolio, BrokerageAccount masterAccount, CancellationToken ct)
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

            lastId = customersChunk.Values.Max(v => v.CustomerId);
        }

        var saveResult = await _deliveryRepository.SaveAsync(ct);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);

        return Result.Ok(new DistributionResultDTO(responsePurchaseOrdersPerAsset, responseDistributions, residualFromMaster, deliveries));
    }
}
