using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Application.Commands.CreateRecommendedPortfolio;

public class CreateRecommendedPortfolioResponse
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<PortfolioItemResponse> PortfolioItems { get; private set; }
    public bool RebalanceTriggered { get; private set; }
    public string Message { get; private set; }

    private CreateRecommendedPortfolioResponse(RecommendedPortfolio recommendedPortfolio)
    {
        Id = recommendedPortfolio.Id;
        Name = recommendedPortfolio.Name;
        Active = recommendedPortfolio.Active;
        CreatedAt = recommendedPortfolio.CreatedAt;
        PortfolioItems = recommendedPortfolio.Items.Select(PortfolioItemResponse.PortfolioItemFromDomain).ToList();
        RebalanceTriggered = false;
    }

    public static CreateRecommendedPortfolioResponse SuccessfullyCreated(RecommendedPortfolio recommendedPortfolio)
    {
        return new CreateRecommendedPortfolioResponse(recommendedPortfolio)
        {
            Message = "Recommended portfolio created successfully."
        };
    }

    public class PortfolioItemResponse
    {
        public string Ticker { get; private set; }
        public decimal Percentage { get; private set; }
        public static PortfolioItemResponse PortfolioItemFromDomain(PortfolioItem item)
        {
            return new PortfolioItemResponse
            {
                Ticker = item.Ticker,
                Percentage = item.Percentage
            };
        }
    }
}
