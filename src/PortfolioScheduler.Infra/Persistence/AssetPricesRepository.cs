using FluentResults;
using Microsoft.EntityFrameworkCore;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Infra.Data.Context;

namespace PortfolioScheduler.Infra.Persistence;

public class AssetPricesRepository : IAssetPricesRepository
{
    private readonly AppDbContext _dbContext;

    public AssetPricesRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(IEnumerable<AssetPrices> assetPrices)
    {
        await _dbContext.AssetPrices.AddRangeAsync(assetPrices);
    }

    public async Task<IEnumerable<AssetPrices>> GetAssetPricesByDateAsync(DateTime tradingDate, CancellationToken ct)
    {
        return await _dbContext.AssetPrices
            .Where(x => x.TradingDate == tradingDate)
            .ToListAsync(ct);
    }

    /// <summary>
    /// If the trading date already has registered asset prices, it will not save changes to avoid duplicates. 
    /// Otherwise, it will save the new asset prices to the database.
    /// </summary>
    public async Task<Result> SaveChangesAsync(IEnumerable<AssetPrices> assetPrices, CancellationToken ct)
    {
        try
        {
            var tradingDate = assetPrices.First().TradingDate;

            var alreadyRegistered = await _dbContext.AssetPrices.AnyAsync(a => a.TradingDate == tradingDate, ct);
            if (alreadyRegistered)
                return Result.Ok();

            await _dbContext.SaveChangesAsync(ct);
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail("An error occurred while saving changes to the database.");
        }
    }
}
