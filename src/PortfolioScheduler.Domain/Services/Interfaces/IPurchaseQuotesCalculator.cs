using FluentResults;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Services.DTOs;

namespace PortfolioScheduler.Domain.Services.Interfaces;

public interface IPurchaseQuotesCalculator
{
    Dictionary<string, decimal> CalculateAmountPerAsset(decimal totalAmount, RecommendedPortfolio portfolio);

    Result<Dictionary<string, AssetPurchaseDTO>> CalculateQuantityPerAsset(Dictionary<string, decimal> amountPerAsset, IEnumerable<QuoteDTO> lastClosePricesCotahist, BrokerageAccount masterAccount);

    Dictionary<string, AssetPurchaseDTO> DefinePurchaseMarketType(Dictionary<string, AssetPurchaseDTO> amountPerAsset);
}