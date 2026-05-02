using Microsoft.EntityFrameworkCore;
using PortfolioScheduler.Application.Queries.Contracts;
using PortfolioScheduler.Application.Queries.GetCurrentRecommendedPortfolio;
using PortfolioScheduler.Infra.Data.Context;

namespace PortfolioScheduler.Infra.Queries;

public class RecommendedPortfolioReadRepository : IRecommendedPortfolioReadRepository
{
    private readonly AppDbContext _dbContext;

    public RecommendedPortfolioReadRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetCurrentRecommendedPortfolioResponse> GetCurrentRecommendedPortfolioAsync(CancellationToken ct)
    {
        var portfolio = await _dbContext.RecommendedPortfolios
            .AsNoTracking()
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Active, ct);

        if (portfolio == null)
            return null;

        var latestTradingDate = await _dbContext.AssetPrices.MaxAsync(p => p.TradingDate, ct);

        var assetsPrice = await _dbContext.AssetPrices
            .AsNoTracking()
            .Where(x => x.TradingDate == latestTradingDate)
            .ToDictionaryAsync(x => x.Ticker, x => x.ClosePrice, ct);

        return new GetCurrentRecommendedPortfolioResponse(
            PortfolioId: portfolio.Id,
            Name: portfolio.Name,
            Active: portfolio.Active,
            CreatedAt: portfolio.CreatedAt,
            Items: portfolio.Items.Select(x => new ItemsResponse(
                Ticker: x.Ticker,
                Percentage: x.Percentage,
                ActualPrice: assetsPrice.TryGetValue(x.Ticker, out var actualPrice) ? actualPrice : 0
                )).ToList());
    }
}
