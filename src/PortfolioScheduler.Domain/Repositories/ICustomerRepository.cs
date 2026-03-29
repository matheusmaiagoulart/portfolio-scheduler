using FluentResults;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Domain.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken ct);
    Task<Customer> GetByIdAsync(long id, CancellationToken ct);
    void Update(Customer customer);
    Task<Result> SaveChangesAsync(CancellationToken ct);
}
