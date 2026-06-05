using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Services.DTOs;

namespace PortfolioScheduler.Domain.Services.Interfaces;

public interface IPortfolioRebalancer
{
    RebalanceResultDTO SellRemovedAssets(
        Dictionary<string, Custody> custodies,
        BrokerageAccount masterAccount,
        IEnumerable<string> removedTickers,
        Dictionary<string, decimal> pricesByTicker,
        long customerId);

    RebalanceResultDTO SellExcessFromAlteredAssets(
        Dictionary<string, Custody> custodies,
        BrokerageAccount masterAccount,
        IEnumerable<(string Ticker, decimal NewPercentage)> alteredAssets,
        Dictionary<string, decimal> pricesByTicker,
        long customerId);

    void ReplaceCustodyTickers(
        Dictionary<string, Custody> custodies,
        IEnumerable<string> removedTickers,
        IEnumerable<string> addedTickers);
}
