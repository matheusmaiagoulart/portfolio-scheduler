
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Application.Commands.RegisterCustomerSubscriber;

public class RegisterCustomerSubscriberResponse
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string Message { get; init; }
    public decimal MonthlyAmount { get; init; }
    public BrokerageAccountResponse BrokerageAccount { get; init; }

    private RegisterCustomerSubscriberResponse(long id, string name, decimal monthlyAmount, BrokerageAccountResponse brokerageAccount)
    {
        Id = id;
        Name = name;
        MonthlyAmount = monthlyAmount;
        BrokerageAccount = brokerageAccount;
    }

    public static RegisterCustomerSubscriberResponse SuccessRegister(Customer customer)
    {
        return new RegisterCustomerSubscriberResponse(customer.Id, customer.Name, customer.MonthlyAmount, BrokerageAccountResponse.ParseResponse(customer.BrokerageAccount))
        {
            Message = $"Congrats, {customer.Name}! Your subscription has been successfully registered."
        };
    }
}

public class BrokerageAccountResponse
{
    public long Id { get; init; }
    public string AccountNumber { get; init; }
    public string Type { get; init; }
    public DateTime CreatedAt { get; init; }
    private BrokerageAccountResponse(long id, string type, string accountNumber, DateTime createdAt)
    {
        Id = id;
        Type = type;
        AccountNumber = accountNumber;
        CreatedAt = createdAt;
    }
    public static BrokerageAccountResponse ParseResponse(BrokerageAccount brokerageAccount)
    {
        return new BrokerageAccountResponse(brokerageAccount.Id, brokerageAccount.AccountType.ToString(), brokerageAccount.AccountNumber, brokerageAccount.CreatedAt);
    }
}
