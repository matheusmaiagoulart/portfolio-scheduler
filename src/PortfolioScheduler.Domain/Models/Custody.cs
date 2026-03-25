using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioScheduler.Domain.Models
{
    public class Custody
    {
        public long Id { get; }
        public long BrokarageAccountId { get; private set; }
        public string Ticker { get; private set; }
        public int Quantity { get; private set; }
        public decimal AveragePrice { get; private set; }
        public DateTime LastUpdate { get; private set; }

        public IReadOnlyCollection<BrokerageAccount> BrokarageAccounts { get; }

        protected Custody() { }
        public Custody(long brokarageAccountId, string ticker, int quantity, decimal averagePrice)
        {
            BrokarageAccountId = brokarageAccountId;
            Ticker = ticker;
            Quantity = quantity;
            AveragePrice = averagePrice;
            LastUpdate = DateTime.UtcNow;
        }
    }
}
