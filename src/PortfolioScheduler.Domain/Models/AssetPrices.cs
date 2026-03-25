using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioScheduler.Domain.Models
{
    public class AssetPrices
    {
        public long Id { get; }
        public DateTime TradingDate { get; private set; } // Data de referência para os preços (Data Pregão)
        public string Ticker { get; private set; }
        public decimal OpenPrice { get; private set; }
        public decimal ClosePrice { get; private set; }
        public decimal HighPrice { get; private set; }
        public decimal LowPrice { get; private set; }

        protected AssetPrices() { }

        public AssetPrices(DateTime tradingDate, string ticker, decimal openPrice, decimal closePrice, decimal highPrice, decimal lowPrice)
        {
            TradingDate = tradingDate;
            Ticker = ticker;
            OpenPrice = openPrice;
            ClosePrice = closePrice;
            HighPrice = highPrice;
            LowPrice = lowPrice;
        }
    }
}
