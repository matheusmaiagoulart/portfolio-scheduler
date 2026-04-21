using static PortfolioScheduler.Domain.Services.DTOs.DistributionResultDTO;

namespace PortfolioScheduler.Application.Commands.ExecutePortfolioPurchase;

public record ExecutePortfolioPurchaseResponse(
    DateTime ExecutionDate,
    int TotalCustomers,
    decimal TotalPurchaseAmount,
    IReadOnlyCollection<PurchaseOrdersPerAsset> PurchaseOrder,
    IReadOnlyCollection<Distribuitions> Distribuitions,
    IReadOnlyCollection<ResidualsFromMaster> ResidualsUsed,
    string Message);

