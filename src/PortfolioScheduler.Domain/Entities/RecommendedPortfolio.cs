namespace PortfolioScheduler.Domain.Entities;
public class RecommendedPortfolio
{
    public long Id { get; }
    public string Name { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? TerminationDate { get; }

    private readonly List<PortfolioItem> _portfolioItems = new();
    public IReadOnlyCollection<PortfolioItem> Items => _portfolioItems.AsReadOnly();

    protected RecommendedPortfolio() { }

    public RecommendedPortfolio(string name)
    {
        Name = name;
        Active = true;
        CreatedAt = DateTime.Now;
        TerminationDate = null;
    }
}
