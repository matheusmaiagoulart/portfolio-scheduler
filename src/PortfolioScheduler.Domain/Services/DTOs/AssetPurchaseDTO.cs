namespace PortfolioScheduler.Domain.Services.DTOs;

public class AssetPurchaseDTO
{
    public string Ticker { get; init; }
    public decimal LastClosePriceBatch { get; init; }
    public decimal LastClosePriceFractional { get; init; }
    public int QuantityToBuy { get; init; }
    public int QuantityFromMasterAccount { get; init; }
    public int TotalQuantity => QuantityToBuy + QuantityFromMasterAccount;
    public MarketTypePurchase? MarketType { get; set; }
}

public class MarketTypePurchase
{
    public int LoteQuantity { get; set; } = 0;
    public int FractionalQuantity { get; set; } = 0;
}