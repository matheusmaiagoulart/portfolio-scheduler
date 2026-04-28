namespace PortfolioScheduler.Domain.Repositories;

public interface IAppDbContext
{
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}