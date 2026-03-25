using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
