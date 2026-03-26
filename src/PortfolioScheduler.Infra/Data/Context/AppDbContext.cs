using Microsoft.EntityFrameworkCore;
using PortfolioScheduler.Domain.Models;

namespace PortfolioScheduler.Infra.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<BrokerageAccount> BrokerageAccounts { get; set; }
        public DbSet<Custody> Custodies { get; set; }
        public DbSet<AssetPrices> AssetPrices { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<RecommendedPortfolio> RecommendedPortfolios { get; set; }
        public DbSet<PortfolioItem> PortfolioItems { get; set; }
        public DbSet<PortfolioRebalance> PortfolioRebalances { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<TaxEvent> TaxEvents { get; set; }
    }
}
