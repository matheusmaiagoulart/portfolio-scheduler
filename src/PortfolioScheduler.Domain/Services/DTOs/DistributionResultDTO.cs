using static PortfolioScheduler.Domain.Services.DTOs.DistributionResultDTO;

namespace PortfolioScheduler.Domain.Services.DTOs;

public record DistributionResultDTO(
    IReadOnlyCollection<PurchaseOrdersPerAsset> purchaseOrders,
    IReadOnlyList<Distribuitions> distribuitions,
    List<ResidualsFromMaster> ResidualsFromMasterAccount)
{
    public record Distribuitions(
        long CustomerId,
        string FullName,
        decimal ThirdPartyBalance,
        IEnumerable<DistribuitionPerAsset> DistribuitionsPerAsset
    );
    public record DistribuitionPerAsset(
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