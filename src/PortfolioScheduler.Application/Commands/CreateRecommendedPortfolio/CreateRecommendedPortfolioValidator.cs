using FluentValidation;

namespace PortfolioScheduler.Application.Commands.CreateRecommendedPortfolio;

public class CreateRecommendedPortfolioValidator : AbstractValidator<CreateRecommendedPortfolioCommand>
{
    public CreateRecommendedPortfolioValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.PortfolioItems)
            .NotEmpty().WithMessage("Portfolio items are required.")
            .Must(items => items.All(x => x.Percentage > 0)).WithMessage("All portfolio items must have a percentage greater than 0.")
            .Must(items => items.Sum(x => x.Percentage) == 100).WithMessage("The total percentage of all portfolio items must equal 100%.")
            .Must(items => items.Count <= 5).WithMessage("A recommended portfolio cannot have more than 5 items.");

        RuleFor(x => x.TerminationDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Termination date must be in the future.");
    }
}
