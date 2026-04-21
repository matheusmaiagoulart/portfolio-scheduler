using FluentResults;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Services.DTOs;

namespace PortfolioScheduler.Domain.Services.Interfaces;

public interface IPortfolioDistribuition
{
    Result<DistributionResultDTO> Distribute(IEnumerable<PurchaseOrder> purchasedAssets, PurchaseRoundDataDTO purchaseRoundData, List<PortfolioItem> portfolio, BrokerageAccount masterAccount);
}
