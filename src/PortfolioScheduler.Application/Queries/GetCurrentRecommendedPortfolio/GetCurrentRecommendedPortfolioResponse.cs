namespace PortfolioScheduler.Application.Queries.GetCurrentRecommendedPortfolio;

public record GetCurrentRecommendedPortfolioResponse(
    long PortfolioId,
    string Name,
    bool Active,
    DateTime CreatedAt,
    List<ItemsResponse> Items);


public record ItemsResponse(
    string Ticker,
    decimal Percentage,
    decimal ActualPrice);
