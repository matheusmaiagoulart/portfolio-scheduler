
namespace PortfolioScheduler.Domain.Models
{
    public class RecommendedPortfolio
    {
        public long Id { get; }
        public string Name { get; private set; }
        public bool Active { get; private set; }
        public DateTime CreatedAt { get; }
        public DateTime? TerminationDate {  get; }

        public ICollection<PortfolioItem> PortfolioItems { get; private set; }

        protected RecommendedPortfolio() { }

        public RecommendedPortfolio(string name)
        {
            Name = name;
            Active = true;
            CreatedAt = DateTime.Now;
            TerminationDate = null;
        }
    }
}
