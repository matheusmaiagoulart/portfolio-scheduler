namespace PortfolioScheduler.Domain.Entities;

public class AssetPrices
{
    public long Id { get; }
    public DateTime TradingDate { get; private set; } // Data de referência para os preços (Data Pregão)
    public string Ticker { get; private set; }
    public decimal OpenPrice { get; private set; }
    public decimal ClosePrice { get; private set; }
    public decimal MaxPrice { get; private set; }
    public decimal MinPrice { get; private set; }

    protected AssetPrices() { }

    public AssetPrices(DateTime tradingDate, string ticker, decimal openPrice, decimal closePrice, decimal maxPrice, decimal minPrice)
    {
        TradingDate = tradingDate;
        Ticker = ticker;
        OpenPrice = openPrice;
        ClosePrice = closePrice;
        MaxPrice = maxPrice;
        MinPrice = minPrice;
    }
}
