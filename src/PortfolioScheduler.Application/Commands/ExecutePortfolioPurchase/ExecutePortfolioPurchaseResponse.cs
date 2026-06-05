using PortfolioScheduler.Domain.Services.DTOs;
using static PortfolioScheduler.Domain.Services.DTOs.DistributionResultDTO;

namespace PortfolioScheduler.Application.Commands.ExecutePortfolioPurchase;

public record ExecutePortfolioPurchaseResponse(
    DateTime ExecutionDate,
    int TotalCustomers,
    decimal TotalPurchaseAmount,
    IReadOnlyCollection<PurchaseOrdersPerAsset> PurchaseOrder,
    IReadOnlyCollection<Distributions> Distributions,
    IReadOnlyCollection<ResidualsFromMaster> ResidualsUsed,
    string Message)
{

    public static ExecutePortfolioPurchaseResponse BuildResponse(DistributionResultDTO result)
    {
        return new ExecutePortfolioPurchaseResponse(
                ExecutionDate: DateTime.UtcNow,
                TotalCustomers: result.distributions.Count,
                TotalPurchaseAmount: result.purchaseOrders.Sum(p => p.TotalPrice),
                PurchaseOrder: result.purchaseOrders,
                Distributions: result.distributions,
                ResidualsUsed: result.ResidualsFromMasterAccount,
                Message: $"Compra programada executada com sucesso para {result.distributions.Count} clientes.");
    }
};


