using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioScheduler.Domain.Models
{
    public class TaxEvent
    {
        public long Id { get; }
        public long CustomerId { get; private set; }
        public TaxEventType Type { get; private set; }
        public decimal Amount { get; private set; }
        public bool IsSent { get; private set; }
        public DateTime EventDate { get; private set; }

        public Customer Customer { get; private set; }
        protected TaxEvent() { }

        public TaxEvent(long customerId, TaxEventType type, decimal amount)
        {
            CustomerId = customerId;
            Type = type;
            Amount = amount;
            IsSent = false;
            EventDate = DateTime.Now;
        }
    }

    public enum TaxEventType
    {
        WITHHOLDING_TAX, // "Dedo-duro"
        CAPITAL_GAIN_TAX // IR sobre venda
    }
}
