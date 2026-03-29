namespace PortfolioScheduler.Domain.Entities;
public class Delivery
{
    public long Id { get; }
    public long PurchaseOrderId { get; private set; }
    public long CustodyCustomerId { get; private set; }
    public string Ticker { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public DateTime DeliveryDate { get; }

    protected Delivery() { }

    public Delivery(long purchaseOrderId, long custodyCustomerId, string ticker, int quantity, decimal unitPrice)
    {
        PurchaseOrderId = purchaseOrderId;
        CustodyCustomerId = custodyCustomerId;
        Ticker = ticker;
        Quantity = quantity;
        Quantity = quantity;
        UnitPrice = unitPrice;
        DeliveryDate = DateTime.Now;
    }
}
