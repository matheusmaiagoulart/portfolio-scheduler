using FluentResults;

namespace PortfolioScheduler.Domain.Entities;

public class RecommendedPortfolio
{
    public long Id { get; }
    public string Name { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? TerminationDate { get; private set; }

    private readonly List<PortfolioItem> _portfolioItems = new();
    public IReadOnlyCollection<PortfolioItem> Items => _portfolioItems.AsReadOnly();

    protected RecommendedPortfolio() { }

    private RecommendedPortfolio(string name)
    {
        Name = name;
        Active = true;
        CreatedAt = DateTime.Now;
        TerminationDate = null;
    }

    public static Result<RecommendedPortfolio> Create(string name, Dictionary<String, PortfolioItem> items, DateTime? terminationDate = null)
    {
        if (string.IsNullOrEmpty(name))
            return Result.Fail<RecommendedPortfolio>("Name cannot be null or empty.");

        if (items == null || items.Count == 0 || items.Count() != 5)
            return Result.Fail<RecommendedPortfolio>("A recommended portfolio must contain exactly 5 items.");

        if (items.GroupBy(x => x.Key).Any(c => c.Count() > 1))
            return Result.Fail<RecommendedPortfolio>("The list items had duplicated tickers.");

        if (items.All(x => x.Value.Percentage == 0) || items.Sum(x => x.Value.Percentage) != 100)
            return Result.Fail<RecommendedPortfolio>("The percentage of each item must be greater than 0 and the total percentage must be equal 100.");

        var recommendedPortfolio = new RecommendedPortfolio(name);
        recommendedPortfolio._portfolioItems.AddRange(items.Values);
        recommendedPortfolio.TerminationDate = terminationDate;

        return Result.Ok(recommendedPortfolio);
    }
}
