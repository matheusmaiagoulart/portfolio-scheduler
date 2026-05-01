using PortfolioScheduler.Application.Queries.GetCustomerPortfolio;

namespace PortfolioScheduler.Application.Queries.Contracts;

public interface ICustomerReadRepository
{
    Task<GetCustomerPortfolioResponse> GetCustomerPortfolio(long customerId, CancellationToken ct);
}
