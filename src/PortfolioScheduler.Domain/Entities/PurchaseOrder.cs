using FluentResults;
using PortfolioScheduler.Domain.Services.DTOs;

namespace PortfolioScheduler.Domain.Entities;

public class PurchaseOrder
{
    public long Id { get; }
    public long MasterAccountId { get; private set; }
    public string Ticker { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public MarketType MarketType { get; private set; }
    public DateTime ExecutionDate { get; }

    private readonly List<Delivery> _deliveries = new();
    public IReadOnlyCollection<Delivery> Deliveries => _deliveries.AsReadOnly();

    protected PurchaseOrder() { }

    private PurchaseOrder(long masterAccountId, string ticker, int quantity, decimal unitPrice, MarketType marketType)
    {
        MasterAccountId = masterAccountId;
        Ticker = ticker;
        Quantity = quantity;
        UnitPrice = unitPrice;
        MarketType = marketType;
        ExecutionDate = DateTime.Now;
    }

    public static Result<IEnumerable<PurchaseOrder>> CreatePurchaseOrders(long masterAccountId, IEnumerable<AssetPurchaseDTO> assetPurchases)
    {
        if (assetPurchases == null || !assetPurchases.Any())
            return Result.Ok(Enumerable.Empty<PurchaseOrder>());

        var purchaseOrders = new List<PurchaseOrder>();

        foreach (var asset in assetPurchases)
        {
            if (asset.MarketType.LoteQuantity > 0)
            {
                purchaseOrders.Add(new PurchaseOrder(
                    masterAccountId: masterAccountId,
                    ticker: asset.Ticker,          // "PETR4"
                    quantity: asset.MarketType.LoteQuantity, // 1  = 100 quotities
                    unitPrice: asset.LastClosePriceBatch,
                    marketType: MarketType.BATCH
                ));
            }

            if (asset.MarketType.FractionalQuantity > 0)
            {
                purchaseOrders.Add(new PurchaseOrder(
                    masterAccountId: masterAccountId,
                    ticker: String.Concat(asset.Ticker, "F"),    // "PETR4F"
                    quantity: asset.MarketType.FractionalQuantity,
                    unitPrice: asset.LastClosePriceFractional,
                    marketType: MarketType.FRACTIONAL
                ));
            }
        }

        return Result.Ok<IEnumerable<PurchaseOrder>>(purchaseOrders);
    }
}

public enum MarketType
{
    BATCH,
    FRACTIONAL
}
