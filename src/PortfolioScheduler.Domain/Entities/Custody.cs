using PortfolioScheduler.Domain.Utils;

namespace PortfolioScheduler.Domain.Entities;

public class Custody
{
    public long Id { get; }
    public long BrokerageAccountId { get; private set; }
    public string Ticker { get; private set; }
    public int Quantity { get; private set; }
    public decimal AveragePrice { get; private set; }
    public DateTime LastUpdate { get; private set; }

    private Custody() { }

    private Custody(string ticker, int quantity, decimal averagePrice)
    {
        Ticker = ticker;
        Quantity = quantity;
        AveragePrice = averagePrice;
        LastUpdate = DateTime.UtcNow;
    }

    public static Custody Create(string ticker)
    {
        return new Custody(ticker, 0, 0);
    }

    public void AddPurchaseQuantity(int quantity, decimal newPrice)
    {
        UpdateAveragePrice(newPrice, quantity);
        Quantity += quantity;
        UpdateLastUpdate();
    }

    public void UpdateResidualQuantity(int quantity, decimal newPrice)
    {
        Quantity = quantity;
        UpdateAveragePrice(newPrice, quantity);
        UpdateLastUpdate();
    }

    private void UpdateLastUpdate()
    {
        LastUpdate = DateTime.Now;
    }

    private void UpdateAveragePrice(decimal newPrice, int newQuantity)
    {
        var totalQuantity = Quantity + newQuantity;
        if (totalQuantity == 0)
        {
            AveragePrice = 0;
            return;
        }
        AveragePrice = (Quantity * AveragePrice + newQuantity * newPrice) / totalQuantity;
    }

    public decimal CalcPL(decimal currentPrice)
    {
        return (currentPrice - AveragePrice) * Quantity;
    }

    public static decimal CalcPortfolioTotalValue(IReadOnlyCollection<Custody> custodies, Dictionary<string, decimal> currentPrices)
    {
        return custodies.Sum(c => c.Quantity * currentPrices[c.Ticker]);
    }

    public static decimal CalcPortfolioProfitability(decimal PortfolioCurrentValue, decimal TotalInvestedAmount)
    {
        if (TotalInvestedAmount == 0) return 0;
        return ((PortfolioCurrentValue - TotalInvestedAmount) / TotalInvestedAmount) * 100;
    }

    public decimal CalcPlPercentual(decimal currentPrice)
    {
        if (AveragePrice == 0) return 0;
        return ((currentPrice - AveragePrice) / AveragePrice) * 100;
    }

    public decimal CalcCompositionPercentage(decimal currentPrice, decimal portfolioCurrentValue)
    {
        if (portfolioCurrentValue == 0) return 0;
        return ((currentPrice * Quantity) / portfolioCurrentValue) * 100;
    }

    public static decimal CalcTotalInvestedAmount(IReadOnlyCollection<Custody> custodies)
    {
        return custodies.Sum(c => c.Quantity * c.AveragePrice);
    }
    public decimal SellQuantity(int quantity, decimal currentPrice)
    {
        var actualQty = Math.Min(quantity, Quantity);
        var valueReleased = actualQty * currentPrice;
        Quantity -= actualQty;
        UpdateLastUpdate();
        return valueReleased.ToMoney();
    }

    public void SellAllAssets()
    {
        Quantity = 0;
        AveragePrice = 0;
        UpdateLastUpdate();
    }

    public void ReplaceAsset(string asset)
    {
        Ticker = asset;
        UpdateLastUpdate();
    }
}
