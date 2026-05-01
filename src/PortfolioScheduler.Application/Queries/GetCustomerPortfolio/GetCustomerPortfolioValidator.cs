using FluentValidation;

namespace PortfolioScheduler.Application.Queries.GetCustomerPortfolio;

public class GetCustomerPortfolioValidator : AbstractValidator<GetCustomerPortfolioQuery>
{
    public GetCustomerPortfolioValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("CustomerId must be greater than 0");
    }
}
