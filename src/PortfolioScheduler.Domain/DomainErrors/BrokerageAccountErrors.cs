using FluentResults;

namespace PortfolioScheduler.Domain.DomainErrors;

public static class BrokerageAccountErrors
{
    public static Error NoMasterAccount()
    {
        return new Error("Nenhuma conta master encontrada.")
            .WithMetadata("statusCode", 404);
    }
}
