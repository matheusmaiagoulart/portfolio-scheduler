using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Domain.Services.DTOs;

public record PurchaseRoundDataDTO(
    decimal TotalPurchaseAmount,
    Dictionary<long, CustodyPurchaseDataDTO> CustodiesPerCustomer);

public record CustodyPurchaseDataDTO
{
    public long CustomerId { get; init; }
    public string FullName { get; init; }
    public long BrokerageAccountId { get; init; }
    public decimal ThirdPartyBalance { get; init; }
    public IEnumerable<Custody> CustomerCustodies { get; init; }
}