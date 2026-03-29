namespace PortfolioScheduler.Domain.Entities;
public class PortfolioRebalance
{
    public long Id { get; }
    public long CustomerId { get; private set; }
    public RebalanceType RebalanceType { get; private set; }
    public string SoldTicker { get; private set; }
    public string BoughtTicker { get; private set; }
    public decimal SaleAmount { get; private set; }
    public DateTime RebalanceDate { get; private set; }

    protected PortfolioRebalance() { }

    public PortfolioRebalance(long customerId, RebalanceType rebalanceType, string soldTicker, string boughtTicker, decimal saleAmount)
    {
        CustomerId = customerId;
        RebalanceType = rebalanceType;
        SoldTicker = soldTicker;
        BoughtTicker = boughtTicker;
        SaleAmount = saleAmount;
        RebalanceDate = DateTime.Now;
    }
}

public enum RebalanceType
{
    PORTFOLIO_CHANGE, // Mudança de estratégica (MUDANCA_CESTA)
    DRIFT_ADJUSTMENT // ajuste por desvio (DESVIO)
}
