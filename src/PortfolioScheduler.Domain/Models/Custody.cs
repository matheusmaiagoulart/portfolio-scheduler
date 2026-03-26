
namespace PortfolioScheduler.Domain.Models
{
    public class Custody
    {
        public long Id { get; }
        public long BrokerageAccountId { get; private set; }
        public string Ticker { get; private set; }
        public int Quantity { get; private set; }
        public decimal AveragePrice { get; private set; }
        public DateTime LastUpdate { get; private set; }

        public BrokerageAccount BrokerageAccount { get; }
        public ICollection<Delivery> Deliveries { get; }

        protected Custody() { }
        public Custody(long brokerageAccountId, string ticker, int quantity, decimal averagePrice)
        {
            BrokerageAccountId = brokerageAccountId;
            Ticker = ticker;
            Quantity = quantity;
            AveragePrice = averagePrice;
            LastUpdate = DateTime.UtcNow;
        }
    }
}
