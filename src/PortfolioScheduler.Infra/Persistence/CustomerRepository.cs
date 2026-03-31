using FluentResults;
using Microsoft.EntityFrameworkCore;
using PortfolioScheduler.Domain.DomainErrors;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Infra.Data.Context;

namespace PortfolioScheduler.Infra.Persistence;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;
    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        await _context.Customers.AddAsync(customer, ct);
    }

    public async Task<Customer> GetByIdAsync(long id, CancellationToken ct)
    {
        return await _context.Customers.FindAsync(id, ct);
    }

    public async Task<Result> SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            await _context.SaveChangesAsync(ct);
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            var errorMessage = ex.InnerException?.Message;

            if (errorMessage.Contains("IX_Customers_Cpf"))
                return Result.Fail(CustomerError.DuplicatedCpf());

            if (errorMessage.Contains("IX_Customers_Email"))
                return Result.Fail(CustomerError.DuplicatedEmail());

            return Result.Fail("An error occurred while saving changes to the database.");
        }
    }

    public void Update(Customer customer)
    {
        _context.Customers.Update(customer);
    }
}
