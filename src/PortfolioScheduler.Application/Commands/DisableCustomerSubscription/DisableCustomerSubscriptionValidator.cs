using FluentValidation;

namespace PortfolioScheduler.Application.Commands.DisableCustomerSubscription;

public class DisableCustomerSubscriptionValidator : AbstractValidator<DisableCustomerSubscriptionCommand>
{
    public DisableCustomerSubscriptionValidator()
    {
        RuleFor(x => x.customerId)
            .GreaterThan(0).WithMessage("Customer ID must be greater than 0.");
    }
}
