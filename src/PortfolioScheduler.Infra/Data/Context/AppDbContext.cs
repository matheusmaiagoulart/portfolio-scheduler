using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PortfolioScheduler.Domain.Entities;
using PortfolioScheduler.Domain.Repositories;

namespace PortfolioScheduler.Infra.Data.Context
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        private IDbContextTransaction? _currentTransaction;
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

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            _currentTransaction = await Database.BeginTransactionAsync(ct);
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            await _currentTransaction.CommitAsync(ct);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            await _currentTransaction.RollbackAsync(ct);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
}
