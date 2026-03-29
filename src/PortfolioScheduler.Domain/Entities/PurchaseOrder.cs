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

    public PurchaseOrder(long masterAccountId, string ticker, int quantity, decimal unitPrice, MarketType marketType)
    {
        MasterAccountId = masterAccountId;
        Ticker = ticker;
        Quantity = quantity;
        UnitPrice = unitPrice;
        MarketType = marketType;
        ExecutionDate = DateTime.Now;
    }
}

public enum MarketType
{
    BATCH,
    FRACTIONAL
}
