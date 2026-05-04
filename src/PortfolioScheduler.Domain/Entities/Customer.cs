using FluentResults;

namespace PortfolioScheduler.Domain.Entities;

public class Customer
{
    public long Id { get; }
    public string Name { get; private set; }
    public string Cpf { get; private set; }
    public string Email { get; private set; }
    public decimal MonthlyAmount { get; private set; }
    public bool Active { get; private set; }
    public DateTime JoiningDate { get; private set; }

    public BrokerageAccount BrokerageAccount { get; private set; }

    protected Customer() { }

    private Customer(string name, string cpf, string email, decimal monthlyAmount)
    {
        Name = name;
        Cpf = cpf;
        Email = email;
        MonthlyAmount = monthlyAmount;
        Active = true;
        JoiningDate = DateTime.Now;
        BrokerageAccount = BrokerageAccount.Create();
    }

    public static Result<Customer> Create(string name, string cpf, string email, decimal monthlyAmount)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Fail("Name can't be null.");

        if (string.IsNullOrWhiteSpace(cpf)) return Result.Fail("CPF can't be null.");

        if (monthlyAmount.CompareTo(new Decimal(100)) < 0) return Result.Fail("Monthly amount must be greater than 100.");

        return Result.Ok(new Customer(name, cpf, email, monthlyAmount));
    }

    public Result Disable()
    {
        if (!Active) return Result.Fail("Customer is already disabled.");

        Active = false;
        return Result.Ok();
    }
}
