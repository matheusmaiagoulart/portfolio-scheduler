using FluentResults;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Domain.Repositories;

public interface IRecommendedPortfolioRepository
{
    Task AddAsync(RecommendedPortfolio recommendedPortfolio, CancellationToken ct);
    Task<RecommendedPortfolio> GetByIdAsync(long id, CancellationToken ct);
    void Update(RecommendedPortfolio recommendedPortfolio);
    Task<Result> SaveChangesAsync(long portfolioId, CancellationToken ct);
}
