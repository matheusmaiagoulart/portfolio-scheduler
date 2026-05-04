using FluentResults;

namespace PortfolioScheduler.Domain.DomainErrors;

public static class PortfolioErrors
{
    public static Error NoActiveRecommendedPortfolio()
    {
        return new Error("Nenhum portfólio recomendado ativo encontrado.")
            .WithMetadata("statusCode", 404);
    }

    public static Error NoOneRecommendedPortfoliosRegistered()
    {
        return new Error("Nenhum portfólio recomendado registrado encontrado.")
            .WithMetadata("statusCode", 404);
    }
}
