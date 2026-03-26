
namespace PortfolioScheduler.Domain.Models
{
    public class BrokerageAccount
    {
        public long Id { get; }
        public long CustomerId { get; private set; }
        public String AccountNumber { get; private set; }
        public BrokerageAccountType AccountType { get; private set; }
        public DateTime CreatedAt { get; }

        public Customer Customer { get; }
        public ICollection<Custody> Custodies { get; }

        protected BrokerageAccount() { }
        public BrokerageAccount(long customerId, string accountNumber, BrokerageAccountType accountType)
        {
            CustomerId = customerId;
            AccountNumber = accountNumber;
            AccountType = accountType;
            CreatedAt = DateTime.Now;
        }
    }

    public enum BrokerageAccountType
    {
        MASTER,
        CLIENT // Conta do cliente (Filhote)
    }
}
