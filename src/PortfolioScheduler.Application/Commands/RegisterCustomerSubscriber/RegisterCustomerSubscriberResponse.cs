
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Application.Commands.RegisterCustomerSubscriber;

public class RegisterCustomerSubscriberResponse
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string Message { get; init; }
    public decimal MonthlyAmount { get; init; }

    private RegisterCustomerSubscriberResponse(long id, string name, decimal monthlyAmount)
    {
        Id = id;
        Name = name;
        MonthlyAmount = monthlyAmount;
    }

    public static RegisterCustomerSubscriberResponse SuccessRegister(Customer customer)
    {
        return new RegisterCustomerSubscriberResponse(customer.Id, customer.Name, customer.MonthlyAmount)
        {
            Message = $"Congrats, {customer.Name}! Your subscription has been successfully registered."
        };
    }
}
