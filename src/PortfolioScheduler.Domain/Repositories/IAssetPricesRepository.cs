using FluentResults;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Domain.Repositories;

public interface IAssetPricesRepository
{
    Task<IEnumerable<AssetPrices>> GetAssetPricesByDateAsync(DateTime tradingDate, CancellationToken ct);
    Task AddAsync (IEnumerable<AssetPrices> assetPrices);
    Task<Result> SaveChangesAsync(IEnumerable<AssetPrices> assetPrices, CancellationToken ct);
}