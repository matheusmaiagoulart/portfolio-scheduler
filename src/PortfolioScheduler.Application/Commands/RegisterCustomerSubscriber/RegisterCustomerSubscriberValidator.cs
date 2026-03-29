using FluentValidation;

namespace PortfolioScheduler.Application.Commands.RegisterCustomerSubscriber;

public class RegisterCustomerSubscriberValidator : AbstractValidator<RegisterCustomerSubscriberCommand>
{
    public RegisterCustomerSubscriberValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required. Can't be empty or null.")
            .MaximumLength(200).WithMessage("Name can't exceed 200 characters.");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF is required. Can't be empty or null.")
            .Must(c => c.All(char.IsDigit)).WithMessage("CPF must contain only digits.")
            .MaximumLength(11).WithMessage("CPF can't exceed 11 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required. Can't be empty or null.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(200).WithMessage("Email can't exceed 200 characters.");

        RuleFor(x => x.MonthlyAmount)
            .NotEmpty().WithMessage("Monthly amount is required. Can't be null.")
            .GreaterThan(100).WithMessage("The monthly amount to be invested must be greater than 100.")
            .PrecisionScale(18, 2, false).WithMessage("The monthly amount must have up to 18 digits in total, with up to 2 decimal places.");
    }
}
