
namespace PortfolioScheduler.Domain.Models
{
    public class PurchaseOrder
    {
        public long Id { get; }
        public long MasterAccountId { get; private set; }
        public String Ticker { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public MarketType MarketType { get; private set; }
        public DateTime ExecutionDate { get; }

        public BrokerageAccount BrokerageAccount { get; }
        public ICollection<Delivery> Deliveries { get; }

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
}
