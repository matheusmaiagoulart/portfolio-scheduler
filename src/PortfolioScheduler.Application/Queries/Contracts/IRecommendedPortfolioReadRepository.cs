using PortfolioScheduler.Application.Queries.GetCurrentRecommendedPortfolio;

namespace PortfolioScheduler.Application.Queries.Contracts;

public interface IRecommendedPortfolioReadRepository
{
    Task<GetCurrentRecommendedPortfolioResponse> GetCurrentRecommendedPortfolioAsync(CancellationToken ct);
}
