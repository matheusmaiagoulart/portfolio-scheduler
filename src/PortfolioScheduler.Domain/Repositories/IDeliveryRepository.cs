using FluentResults;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Domain.Repositories;

public interface IDeliveryRepository
{
    Task AddAsync(ICollection<Delivery> deliveries, CancellationToken ct);
    Task<Result> SaveAsync(CancellationToken ct);
}
