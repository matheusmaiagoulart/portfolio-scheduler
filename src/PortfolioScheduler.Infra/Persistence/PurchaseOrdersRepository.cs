using FluentResults;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Infra.Data.Context;

namespace PortfolioScheduler.Infra.Persistence;

public class PurchaseOrdersRepository : IPurchaseOrdersRepository
{
    private readonly AppDbContext _dbContext;

    public PurchaseOrdersRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(IEnumerable<PurchaseOrder> purchaseOrders, CancellationToken ct)
    {
        await _dbContext.PurchaseOrders.AddRangeAsync(purchaseOrders, ct);
    }

    public async Task<Result> SaveAsync(CancellationToken ct)
    {
        try
        {
            await _dbContext.SaveChangesAsync(ct);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
