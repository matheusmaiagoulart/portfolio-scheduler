using FluentResults;
using Microsoft.EntityFrameworkCore;
using PortfolioScheduler.Domain.DomainErrors;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Domain.Services.DTOs;
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
        return await _context.Customers
            .Include(c => c.BrokerageAccount)
            .ThenInclude(b => b.Custodies)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<decimal> GetOneThirdAmountOfAllActiveCustomersAsync(CancellationToken ct)
    {
        var customers = await _context.Customers
        .Where(c => c.Active && c.Id != 1)
        .Select(c => c.MonthlyAmount).SumAsync(ct);

        return Math.Round(customers / 3m, 2);
    }

    public async Task<Dictionary<long, CustodyPurchaseDataDTO>> GetChunkOfCustomerAsync(int chunkSize, long lastId, CancellationToken ct)
    {
        var batch = await _context.Customers
            .Where(c => c.Active && c.BrokerageAccount.Id != 1 && c.Id > lastId)
            .Take(chunkSize)
            .Include(c => c.BrokerageAccount)
            .ThenInclude(b => b.Custodies)
            .ToListAsync(ct);

        var custodiesPerCustomer = batch.ToDictionary(
            keySelector: c => c.BrokerageAccount.Id,
            elementSelector: c => new CustodyPurchaseDataDTO
            {
                CustomerId = c.Id,
                FullName = c.Name,
                BrokerageAccountId = c.BrokerageAccount.Id,
                ThirdPartyBalance = Math.Round(c.MonthlyAmount / 3m, 2),
                CustomerCustodies = c.BrokerageAccount.Custodies
            });

        return custodiesPerCustomer;
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
                return Result.Fail(CustomerErrors.DuplicatedCpf());

            if (errorMessage.Contains("IX_Customers_Email"))
                return Result.Fail(CustomerErrors.DuplicatedEmail());

            return Result.Fail("An error occurred while saving changes to the database.");
        }
    }

    public async Task<Result> UpdateAsync(Customer customer, CancellationToken ct)
    {
        try
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync(ct);
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail("An error occurred while updating the customer in the database.");
        }
    }

    public async Task<Customer> GetMasterAccount(CancellationToken ct)
    {
        return await _context.Customers
            .Include(c => c.BrokerageAccount)
            .ThenInclude(b => b.Custodies)
            .FirstOrDefaultAsync(c => c.BrokerageAccount.AccountType == BrokerageAccountType.MASTER, ct);
    }
}
