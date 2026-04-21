namespace PortfolioScheduler.Domain.Services.DTOs;

public class QuoteDTO
{
    public DateTime DataPregao { get; set; }
    public string Ticker { get; set; } = string.Empty;
    public string BDICode { get; set; } = string.Empty;
    public int MarketType { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal OpenPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal ClosePrice { get; set; }
    public decimal AveragePrice { get; set; }
    public long TradeQuantity { get; set; }
    public decimal TradedVolume { get; set; }
}
