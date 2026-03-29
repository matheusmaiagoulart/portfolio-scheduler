namespace PortfolioScheduler.Domain.Entities;
public class TaxEvent
{
    public long Id { get; }
    public long CustomerId { get; private set; }
    public TaxEventType Type { get; private set; }
    public decimal BaseValue { get; private set; }
    public decimal TaxValue { get; private set; }
    public bool IsSent { get; private set; }
    public DateTime EventDate { get; private set; }

    protected TaxEvent() { }

    public TaxEvent(long customerId, TaxEventType type, decimal baseValue, decimal taxValue)
    {
        CustomerId = customerId;
        Type = type;
        BaseValue = baseValue;
        TaxValue = taxValue;
        IsSent = false;
        EventDate = DateTime.Now;
    }
}

public enum TaxEventType
{
    WITHHOLDING_TAX, // "Dedo-duro"
    CAPITAL_GAIN_TAX // IR sobre venda
}
