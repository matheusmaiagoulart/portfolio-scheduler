using PortfolioScheduler.Application.Queries.GetAllRecommendedPortfolio;
using PortfolioScheduler.Application.Queries.GetCurrentRecommendedPortfolio;

namespace PortfolioScheduler.Application.Queries.Contracts;

public interface IRecommendedPortfolioReadRepository
{
    Task<GetCurrentRecommendedPortfolioResponse> GetCurrentRecommendedPortfolioAsync(CancellationToken ct);
    Task<GetAllRecommendedPortfolioResponse> GetAllRecommendedPortfolioAsync(CancellationToken ct);
}
