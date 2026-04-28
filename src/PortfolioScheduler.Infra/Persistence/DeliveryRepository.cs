using FluentResults;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Infra.Data.Context;

namespace PortfolioScheduler.Infra.Persistence;

public class DeliveryRepository : IDeliveryRepository
{
    private readonly AppDbContext _dbContext;

    public DeliveryRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ICollection<Delivery> deliveries, CancellationToken ct)
    {
        await _dbContext.AddRangeAsync(deliveries, ct);
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
            // Log the exception here
            return Result.Fail(ex.Message);
        }
    }
}
