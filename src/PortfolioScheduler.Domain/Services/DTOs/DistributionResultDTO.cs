using PortfolioScheduler.Domain.Entities;
using static PortfolioScheduler.Domain.Services.DTOs.DistributionResultDTO;

namespace PortfolioScheduler.Domain.Services.DTOs;

public record DistributionResultDTO(
    IReadOnlyCollection<PurchaseOrdersPerAsset> purchaseOrders,
    IReadOnlyList<Distributions> distributions,
    List<ResidualsFromMaster> ResidualsFromMasterAccount,
    ICollection<Delivery> deliveries)
{
    public record Distributions(
        long CustomerId,
        string FullName,
        decimal ThirdPartyBalance,
        IEnumerable<DistributionPerAsset> DistributionsPerAsset
    );
    public record DistributionPerAsset(
       string Ticker,
       int Quantity
    );

    public record PurchaseOrdersPerAsset(
    string Ticker,
    int TotalQuantity,
    PurchaseOrdersPerAssetDetails Details,
    decimal UnitPrice)
    {
        public decimal TotalPrice => UnitPrice * TotalQuantity;
    };

    public record PurchaseOrdersPerAssetDetails(
        string Type,
        string Ticker,
        int Quantity
    );

    public record ResidualsFromMaster(string Ticker, int Quantity);
}