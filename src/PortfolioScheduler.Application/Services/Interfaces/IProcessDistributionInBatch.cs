using FluentResults;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Services.DTOs;

namespace PortfolioScheduler.Application.Services.Interfaces;

public interface IProcessDistributionInBatch
{
    Task<Result<DistributionResultDTO>> ProcessInBatchAsync(
       IEnumerable<PurchaseOrder> purchasedAssets,
       decimal thirdValue,
       List<PortfolioItem> portfolio,
       BrokerageAccount masterAccount,
        CancellationToken ct);
}
