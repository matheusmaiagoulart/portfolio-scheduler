namespace PortfolioScheduler.Domain.Entities;

public class BrokerageAccount
{
    public long Id { get; }
    public long CustomerId { get; private set; }
    public String AccountNumber { get; private set; }
    public BrokerageAccountType AccountType { get; private set; }
    public DateTime CreatedAt { get; }

    private readonly List<Custody> _custodies = new();
    public IReadOnlyCollection<Custody> Custodies => _custodies.AsReadOnly();

    protected BrokerageAccount() { }

    public BrokerageAccount(BrokerageAccountType accountType)
    {
        AccountNumber = GeneratedAccountNumber();
        AccountType = accountType;
        CreatedAt = DateTime.Now;
    }

    private string GeneratedAccountNumber()
    {
        return Guid.NewGuid().ToString().Substring(0, 20).ToUpper();
    }
}

public enum BrokerageAccountType
{
    MASTER,
    CLIENT // Conta do cliente (Filhote)
}
