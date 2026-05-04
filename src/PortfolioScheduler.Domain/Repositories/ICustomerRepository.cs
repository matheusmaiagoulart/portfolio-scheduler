using FluentResults;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Services.DTOs;

namespace PortfolioScheduler.Domain.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken ct);
    Task<Customer> GetByIdAsync(long id, CancellationToken ct);
    Task<Customer> GetMasterAccount(CancellationToken ct);
    Task<decimal> GetOneThirdAmountOfAllActiveCustomersAsync(CancellationToken ct);
    Task<Dictionary<long, CustodyPurchaseDataDTO>> GetChunkOfCustomerAsync(int chunkSize, long lastId, CancellationToken ct);
    Task<Result> UpdateAsync(Customer customer, CancellationToken ct);
    Task<Result> SaveChangesAsync(CancellationToken ct);
}
