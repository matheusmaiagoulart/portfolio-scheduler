using FluentResults;

namespace PortfolioScheduler.Domain.DomainErrors;

public static class CotahistErrors
{
    public static Error AssetsPricesIsEmpty()
    {
        return new Error("Nenhuma cotação disponível para execução da compra.")
            .WithMetadata("statusCode", 400);
    }
}
