namespace PortfolioScheduler.Domain.Entities;

public class PortfolioItem
{
    public long Id { get; }
    public long RecommendedPortfolioId { get; private set; }
    public string Ticker { get; private set; }
    public decimal Percentage { get; private set; }

    protected PortfolioItem() { }

    public PortfolioItem(long recommendedPortfolioId, string ticker, decimal percentage)
    {
        RecommendedPortfolioId = recommendedPortfolioId;
        Ticker = ticker;
        Percentage = percentage;
    }
}
