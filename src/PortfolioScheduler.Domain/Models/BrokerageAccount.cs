using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioScheduler.Domain.Models
{
    public class BrokerageAccount
    {
        public long Id { get; }
        public long CustomerId { get; private set; }
        public String AccountNumber { get; private set; }
        public BrokarageAccountType AccountType { get; private set; }
        public DateTime CreatedAt { get; }

        public Customer Customer { get; }

        protected BrokerageAccount() { }
        public BrokerageAccount(long customerId, string accountNumber, BrokarageAccountType accountType)
        {
            CustomerId = customerId;
            AccountNumber = accountNumber;
            AccountType = accountType;
            CreatedAt = DateTime.Now;
        }
    }

    public enum BrokarageAccountType
    {
        MASTER,
        CLIENT // Conta do cliente (Filhote)
    }
}
