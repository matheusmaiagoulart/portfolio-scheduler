using FluentResults;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Domain.Repositories;

public interface IPurchaseOrdersRepository
{
    Task AddAsync(IEnumerable<PurchaseOrder> purchaseOrders, CancellationToken ct);
    Task<Result> SaveAsync(CancellationToken ct);
}