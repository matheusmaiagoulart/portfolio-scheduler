namespace PortfolioScheduler.Application.Commands.DisableCustomerSubscription;

public record DisableCustomerSubscriptionResponse(
    long customerId,
    string name,
    bool active, 
    DateTime exitDate)
{
    public string Message { get; init; } = "Customer subscription disabled successfully. Your custodies positions will be keeped.";
}