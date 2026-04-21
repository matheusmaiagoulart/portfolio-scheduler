using FluentResults;
using Microsoft.EntityFrameworkCore;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Infra.Data.Context;

namespace PortfolioScheduler.Infra.Persistence;

public class RecommendedPortfolioRepository : IRecommendedPortfolioRepository
{
    private readonly AppDbContext _context;
    public RecommendedPortfolioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RecommendedPortfolio recommendedPortfolio, CancellationToken ct)
    {
        await _context.RecommendedPortfolios.AddAsync(recommendedPortfolio, ct);
    }

    public async Task<RecommendedPortfolio> GetByIdAsync(long id, CancellationToken ct)
    {
        return await _context.RecommendedPortfolios.FindAsync(id, ct);
    }

    public async Task<RecommendedPortfolio> GetRecommendedPortfolioActive(CancellationToken ct)
    {
        return await _context.RecommendedPortfolios
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Active, ct);
    }

    public async Task<Result> SaveChangesAsync(long portfolioId, CancellationToken ct)
    {
        try
        {
            await _context.RecommendedPortfolios
                .Where(x => x.Active && x.Id != portfolioId)
                .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.Active, false)
                .SetProperty(x => x.TerminationDate, DateTime.UtcNow), ct);

            await _context.SaveChangesAsync(ct);
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail("An error occurred while saving changes to the database.");
        }
    }

    public void Update(RecommendedPortfolio recommendedPortfolio)
    {
        _context.RecommendedPortfolios.Update(recommendedPortfolio);
    }
}
