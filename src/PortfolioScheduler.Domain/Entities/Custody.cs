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
       AveragePrice = (Quantity * AveragePrice + newQuantity * newPrice) / (Quantity + newQuantity);
    }
}
