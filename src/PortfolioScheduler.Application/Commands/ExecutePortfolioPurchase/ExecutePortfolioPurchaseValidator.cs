using FluentValidation;

namespace PortfolioScheduler.Application.Commands.ExecutePortfolioPurchase;

public class ExecutePortfolioPurchaseValidator : AbstractValidator<ExecutePortfolioPurchaseCommand>
{
    public ExecutePortfolioPurchaseValidator()
    {
        RuleFor(x => x.LastCotahist)
            .NotEmpty()
            .WithMessage("A lista de cotações não pode ser vazia.");

        RuleForEach(x => x.LastCotahist)
            .ChildRules(quote =>
            {
                quote.RuleFor(q => q.Ticker)
                    .NotEmpty()
                    .WithMessage("Ticker não pode ser nulo ou vazio.");

                quote.RuleFor(q => q.ClosePrice)
                    .GreaterThan(0)
                    .WithMessage(q => $"Preço de fechamento inválido para o ticker {q.Ticker}.");

                quote.RuleFor(q => q.DataPregao)
                    .NotEmpty()
                    .WithMessage(q => $"Data de pregão inválida para o ticker {q.Ticker}.");
            });
    }
}
