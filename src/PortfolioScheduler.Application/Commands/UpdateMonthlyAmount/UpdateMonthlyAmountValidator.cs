using FluentValidation;

namespace PortfolioScheduler.Application.Commands.UpdateMontlhyAmount;

public class UpdateMonthlyAmountValidator : AbstractValidator<UpdateMonthlyAmountCommand>
{

    public UpdateMonthlyAmountValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(100).WithMessage("Monthly amount must be greater or equal than 100.");
    }
}
