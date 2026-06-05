using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Domain.Services.DTOs;

public record RebalanceResultDTO(
    decimal TotalReleased,
    List<PortfolioRebalance> AuditRecords);
