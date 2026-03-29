using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Infra.Data.MappingTables;

public class PortfoliorebalancesMap : IEntityTypeConfiguration<PortfolioRebalance>
{
    public void Configure(EntityTypeBuilder<PortfolioRebalance> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.RebalanceType).IsRequired().HasConversion<string>();
        entity.Property(e => e.SoldTicker).IsRequired().HasMaxLength(10);
        entity.Property(e => e.BoughtTicker).IsRequired().HasMaxLength(10);
        entity.Property(e => e.SaleAmount).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.RebalanceDate).IsRequired().HasColumnType("datetime");

        // Indexes for performance
        entity.HasIndex(e => e.CustomerId).HasDatabaseName("IX_PortfolioRebalances_CustomerId");

        entity.ToTable("PortfolioRebalances");
    }
}
