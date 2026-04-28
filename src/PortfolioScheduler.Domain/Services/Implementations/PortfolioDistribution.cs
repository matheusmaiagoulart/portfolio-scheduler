using FluentResults;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Services.DTOs;
using PortfolioScheduler.Domain.Services.Interfaces;
using static PortfolioScheduler.Domain.Services.DTOs.DistributionResultDTO;

namespace PortfolioScheduler.Domain.Services.Implementations;

public class PortfolioDistribution : IPortfolioDistribution
{
    private record TickerData(long purchaseId, int TotalQuantity, decimal AssetPrice);

    public Result<DistributionResultDTO> Distribute(IEnumerable<PurchaseOrder> purchasedAssets, PurchaseRoundDataDTO purchaseRoundData, List<PortfolioItem> portfolio, BrokerageAccount masterAccount)
    {
        var responseDistributions = new List<DistributionResultDTO.Distributions>();
        var deliveries = new List<Delivery>();
        var masterCustodyByTicker = masterAccount.Custodies.ToDictionary(c => c.Ticker);
        
        var purchasedByTicker = purchasedAssets
            .GroupBy(p => NormalizeTickerName(p.Ticker))
            .ToDictionary(g => g.Key, g => new TickerData(g.FirstOrDefault().Id, g.Sum(p => p.Quantity), g.First().UnitPrice));

        var totalQuantityPerTicker = masterAccount.Custodies
            .ToDictionary(
            p => NormalizeTickerName(p.Ticker),
           c =>
           {
               var purchased = purchasedByTicker.GetValueOrDefault(c.Ticker);
               return new TickerData(
                   purchased?.purchaseId ?? 0,
                   c.Quantity + (purchased?.TotalQuantity ?? 0),
                   purchased?.AssetPrice ?? c.AveragePrice);
           });

        var distribuitedPerTicker =
            masterAccount.Custodies.GroupBy(c => NormalizeTickerName(c.Ticker))
             .ToDictionary(
                p => NormalizeTickerName(p.Key),
                p => 0);
        
        foreach (var customerData in purchaseRoundData.CustodiesPerCustomer.Values)
        {
            var proportion = customerData.ThirdPartyBalance / purchaseRoundData.TotalPurchaseAmount;
            var assetsToCustomer = new List<DistributionPerAsset>();

            foreach (var custody in customerData.CustomerCustodies)
            {
                var tickerValue = totalQuantityPerTicker.GetValueOrDefault(custody.Ticker)?.AssetPrice ?? 0;
                var tickerQuantity = totalQuantityPerTicker.GetValueOrDefault(custody.Ticker)?.TotalQuantity ?? 0;

                var quantityToDistribute = (int)Math.Truncate(tickerQuantity * proportion);
                if (quantityToDistribute <= 0)
                    continue;

                custody.AddPurchaseQuantity(quantityToDistribute, tickerValue);
                distribuitedPerTicker[custody.Ticker] += quantityToDistribute;

                assetsToCustomer.Add(new DistributionPerAsset(custody.Ticker, quantityToDistribute));
                deliveries.Add(Delivery.CreateDelivery(totalQuantityPerTicker[custody.Ticker].purchaseId, custody.BrokerageAccountId, custody.Ticker, quantityToDistribute, tickerValue));
            }
            
            responseDistributions.Add(new DistributionResultDTO.Distributions(
                customerData.CustomerId,
                customerData.FullName,
                customerData.ThirdPartyBalance,
                assetsToCustomer));
        }

        var masterResiduals = UpdateResidualsFromMasterAccount(
            distribuitedPerTicker, totalQuantityPerTicker, masterAccount);

        return Result.Ok(new DistributionResultDTO(CreateResponseForAssetsPurchased(purchasedAssets, masterCustodyByTicker), responseDistributions, masterResiduals, deliveries));
    }

    private string NormalizeTickerName(string ticker)
    {
        return ticker.EndsWith("F") ? ticker.Remove(ticker.Length - 1) : ticker;
    }

    private List<DistributionResultDTO.ResidualsFromMaster> UpdateResidualsFromMasterAccount(Dictionary<string, int> distribuitedPerTicker, Dictionary<string, TickerData> totalQuantityPerTicker, BrokerageAccount masterAccount)
    {
        var residuals = new List<DistributionResultDTO.ResidualsFromMaster>();

        foreach (var custody in masterAccount.Custodies)
        {
            var ticker = NormalizeTickerName(custody.Ticker);

            int totalQuantity = totalQuantityPerTicker.TryGetValue(ticker, out var value) ? value.TotalQuantity : 0;
            int distribuitedQuantity = distribuitedPerTicker.TryGetValue(ticker, out var distributedValue) ? distributedValue : 0;

            int residualQuantity = totalQuantity - distribuitedQuantity;
            if (residualQuantity <= 0)
                continue;

            custody.UpdateResidualQuantity(residualQuantity, custody.AveragePrice);
            residuals.Add(new DistributionResultDTO.ResidualsFromMaster(ticker, residualQuantity));
        }
        return residuals;
    }

    private IReadOnlyCollection<DistributionResultDTO.PurchaseOrdersPerAsset> CreateResponseForAssetsPurchased(IEnumerable<PurchaseOrder> purchasedAssets, Dictionary<string, Custody> residualsFromMaster)
    {
        return purchasedAssets.Select(asset => new DistributionResultDTO.PurchaseOrdersPerAsset(
            Ticker: NormalizeTickerName(asset.Ticker),
            TotalQuantity: asset.Quantity,
            Details: new DistributionResultDTO.PurchaseOrdersPerAssetDetails(
                Type: asset.MarketType.ToString(),
                Ticker: asset.Ticker,
                Quantity: asset.Quantity),
            UnitPrice: asset.UnitPrice)).ToList();
    }
}
