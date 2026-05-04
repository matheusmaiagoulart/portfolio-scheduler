namespace PortfolioScheduler.Application.Queries.GetAllRecommendedPortfolio
{
    public record GetAllRecommendedPortfolioResponse(
        IEnumerable<RecommendedPortfolioDTO> portfolios);
}

public record RecommendedPortfolioDTO(
    long PortfolioId,
    string Name,
    bool Active,
    DateTime CreatedAt,
    DateTime? DisabledAt,
    List<ItemsDTO> Items
    );

public record ItemsDTO(
    string Ticker,
    decimal Percentage);
