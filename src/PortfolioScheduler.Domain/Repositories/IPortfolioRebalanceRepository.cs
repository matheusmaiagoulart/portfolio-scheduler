using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Domain.Repositories;

public interface IPortfolioRebalanceRepository
{
    Task AddRangeAsync(IEnumerable<PortfolioRebalance> records, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
