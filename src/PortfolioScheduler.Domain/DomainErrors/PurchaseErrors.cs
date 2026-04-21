using FluentResults;

namespace PortfolioScheduler.Domain.DomainErrors;

public static class PurchaseErrors
{
    public static Error InvalidPriceForTicker(string ticker)
    {
        return new Error($"Preço inválido para o ticker {ticker}.")
            .WithMetadata("statusCode", 400);
    }
    public static Error PurchaseAmountInvalid(decimal amount)
    {
        return new Error($"Valor de compra inválido: {amount}.")
            .WithMetadata("statusCode", 400);
    }
}
