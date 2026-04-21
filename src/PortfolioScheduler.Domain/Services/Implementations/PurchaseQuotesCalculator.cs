using FluentResults;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Services.DTOs;
using PortfolioScheduler.Domain.Services.Interfaces;

namespace PortfolioScheduler.Domain.Services.Implementations;

public class PurchaseQuotesCalculator : IPurchaseQuotesCalculator
{
    public Dictionary<string, decimal> CalculateAmountPerAsset(decimal totalAmount, RecommendedPortfolio portfolio)
    {
        var amountPerAsset = new Dictionary<string, decimal>();

        foreach (var item in portfolio.Items)
        {
            var assetAmount = Math.Round((item.Percentage / 100m) * totalAmount, 2);
            amountPerAsset[item.Ticker] = assetAmount;
        }

        return amountPerAsset;
    }

    public Result<Dictionary<string, AssetPurchaseDTO>> CalculateQuantityPerAsset(Dictionary<string, decimal> amountPerAsset, IEnumerable<QuoteDTO> lastClosePricesCotahist, BrokerageAccount masterAccount)
    {
        var result = new Dictionary<string, AssetPurchaseDTO>();

        foreach (var ticker in amountPerAsset.Keys)
        {
            var tickerAmount = amountPerAsset[ticker];

            var lastClosePrice = lastClosePricesCotahist.First(q => q.Ticker == ticker).ClosePrice;
            if (lastClosePrice <= 0)
                return Result.Fail(new Error($"Preço inválido para o ticker {ticker}.").WithMetadata("status", 400));

            var quantity = (int)Math.Floor(tickerAmount / lastClosePrice);

            var masterResidualQuantity = masterAccount.Custodies
                .Where(c => c.Ticker == ticker)
                .Sum(c => c.Quantity);

            var quantityFromMaster = Math.Min(quantity, masterResidualQuantity);

            quantity -= quantityFromMaster;

            var response = new AssetPurchaseDTO
            {
                Ticker = ticker,
                QuantityToBuy = quantity,
                QuantityFromMasterAccount = quantityFromMaster,
                MarketType = new MarketTypePurchase(),
                LastClosePriceBatch = lastClosePricesCotahist.First(q => q.Ticker == ticker).ClosePrice,
                LastClosePriceFractional = lastClosePricesCotahist.First(q => q.Ticker == String.Concat(ticker, "F")).ClosePrice,
            };

            result.Add(ticker, response);
        }
        return Result.Ok(result);
    }

    public Dictionary<string, AssetPurchaseDTO> DefinePurchaseMarketType(Dictionary<string, AssetPurchaseDTO> amountPerAsset)
    {
        foreach (var ticker in amountPerAsset.Keys)
        {
            var quantityToBuy = amountPerAsset[ticker].QuantityToBuy;

            int loteQuantity = (quantityToBuy / 100) * 100;
            int fractionalQuantity = quantityToBuy % 100;

            amountPerAsset[ticker].MarketType.LoteQuantity = loteQuantity;
            amountPerAsset[ticker].MarketType.FractionalQuantity = fractionalQuantity;
        }

        return amountPerAsset;
    }
}
