using FluentResults;
using System.Net;

namespace PortfolioScheduler.Domain.DomainErrors;

public static class CustomerErrors
{
    public static Error DuplicatedCpf() => 
        new Error("A Customer with the same CPF already exists.").WithMetadata("statusCode", HttpStatusCode.Conflict);

    public static Error DuplicatedEmail() =>
        new Error("A Customer with the same Email already exists.").WithMetadata("statusCode", HttpStatusCode.Conflict);
}
