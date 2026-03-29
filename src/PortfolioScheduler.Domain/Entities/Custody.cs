namespace PortfolioScheduler.Domain.Entities;

public class Custody
{
    public long Id { get; }
    public long BrokerageAccountId { get; private set; }
    public string Ticker { get; private set; }
    public int Quantity { get; private set; }
    public decimal AveragePrice { get; private set; }
    public DateTime LastUpdate { get; private set; }

    protected Custody() { }
    public Custody(string ticker, int quantity, decimal averagePrice)
    {
        Ticker = ticker;
        Quantity = quantity;
        AveragePrice = averagePrice;
        LastUpdate = DateTime.UtcNow;
    }
}
