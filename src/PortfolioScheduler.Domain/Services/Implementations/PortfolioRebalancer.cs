using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Services.DTOs;
using PortfolioScheduler.Domain.Services.Interfaces;

namespace PortfolioScheduler.Domain.Services.Implementations;

public class PortfolioRebalancer : IPortfolioRebalancer
{
    public RebalanceResultDTO SellRemovedAssets(
        Dictionary<string, Custody> custodies,
        BrokerageAccount masterAccount,
        IEnumerable<string> removedTickers,
        Dictionary<string, decimal> pricesByTicker,
        long customerId)
    {
        var totalReleased = 0.0m;
        var auditRecords = new List<PortfolioRebalance>();

        foreach (var ticker in removedTickers)
        {
            if (!custodies.TryGetValue(ticker, out var custody)) continue;
            if (custody.Quantity == 0) continue;

            var currentPrice = pricesByTicker.GetValueOrDefault(ticker);
            if (currentPrice <= 0) continue;

            var saleAmount = custody.Quantity * currentPrice;
            totalReleased += saleAmount;
            masterAccount.GetOrCreateCustody(ticker).AddPurchaseQuantity(custody.Quantity, currentPrice);
            custody.SellAllAssets();

            auditRecords.Add(new PortfolioRebalance(
                customerId,
                RebalanceType.PORTFOLIO_CHANGE,
                soldTicker: ticker,
                boughtTicker: string.Empty,
                saleAmount: saleAmount));
        }

        return new RebalanceResultDTO(totalReleased, auditRecords);
    }

    public RebalanceResultDTO SellExcessFromAlteredAssets(
        Dictionary<string, Custody> custodies,
        BrokerageAccount masterAccount,
        IEnumerable<(string Ticker, decimal NewPercentage)> alteredAssets,
        Dictionary<string, decimal> pricesByTicker,
        long customerId)
    {
        var totalReleased = 0.0m;
        var auditRecords = new List<PortfolioRebalance>();
        var alteredList = alteredAssets.ToList();

        var alteredTickers = alteredList.Select(a => a.Ticker).ToHashSet();
        var portfolioValue = custodies
            .Where(x => alteredTickers.Contains(x.Key))
            .Sum(c => c.Value.Quantity * pricesByTicker.GetValueOrDefault(c.Key));

        foreach (var (ticker, newPercentage) in alteredList)
        {
            if (!custodies.TryGetValue(ticker, out var custody)) continue;
            if (custody.Quantity == 0) continue;

            var currentPrice = pricesByTicker.GetValueOrDefault(ticker);
            if (currentPrice <= 0) continue;

            var valorAlvo = portfolioValue * (newPercentage / 100m);
            var quantidadeAlvo = (int)(valorAlvo / currentPrice);
            var excesso = custody.Quantity - quantidadeAlvo;

            if (excesso > 0)
            {
                var saleAmount = custody.SellQuantity(excesso, currentPrice);
                totalReleased += saleAmount;
                masterAccount.GetOrCreateCustody(ticker).AddPurchaseQuantity(excesso, currentPrice);

                auditRecords.Add(new PortfolioRebalance(
                    customerId,
                    RebalanceType.PORTFOLIO_CHANGE,
                    soldTicker: ticker,
                    boughtTicker: ticker,
                    saleAmount: saleAmount));
            }
        }

        return new RebalanceResultDTO(totalReleased, auditRecords);
    }

    public void ReplaceCustodyTickers(
        Dictionary<string, Custody> custodies,
        IEnumerable<string> removedTickers,
        IEnumerable<string> addedTickers)
    {
        var removedList = removedTickers.ToList();
        var addedList = addedTickers.ToList();

        for (int i = 0; i < removedList.Count; i++)
        {
            if (!custodies.TryGetValue(removedList[i], out var custody)) continue;
            custody.ReplaceAsset(addedList[i]);
        }
    }
}

