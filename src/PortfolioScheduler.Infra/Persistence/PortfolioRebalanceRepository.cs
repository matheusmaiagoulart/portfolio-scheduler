using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Infra.Data.Context;

namespace PortfolioScheduler.Infra.Persistence;

public class PortfolioRebalanceRepository : IPortfolioRebalanceRepository
{
    private readonly AppDbContext _dbContext;

    public PortfolioRebalanceRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRangeAsync(IEnumerable<PortfolioRebalance> records, CancellationToken ct)
    {
        await _dbContext.PortfolioRebalances.AddRangeAsync(records, ct);
    }

    public async Task SaveAsync(CancellationToken ct)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}
