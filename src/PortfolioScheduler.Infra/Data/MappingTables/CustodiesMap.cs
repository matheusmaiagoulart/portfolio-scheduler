using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Infra.Data.MappingTables;

public class CustodiesMap : IEntityTypeConfiguration<Custody>
{
    public void Configure(EntityTypeBuilder<Custody> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.HasData(
            new { Id = 1L, BrokerageAccountId = 1L, Ticker = "PETR4", Quantity = 0, AveragePrice = 0m, LastUpdate = new DateTime(2026, 1, 1) },
            new { Id = 2L, BrokerageAccountId = 1L, Ticker = "VALE3", Quantity = 0, AveragePrice = 0m, LastUpdate = new DateTime(2026, 1, 1) },
            new { Id = 3L, BrokerageAccountId = 1L, Ticker = "ITUB4", Quantity = 0, AveragePrice = 0m, LastUpdate = new DateTime(2026, 1, 1) },
            new { Id = 4L, BrokerageAccountId = 1L, Ticker = "BBDC4", Quantity = 0, AveragePrice = 0m, LastUpdate = new DateTime(2026, 1, 1) },
            new { Id = 5L, BrokerageAccountId = 1L, Ticker = "WEGE3", Quantity = 0, AveragePrice = 0m, LastUpdate = new DateTime(2026, 1, 1) }
        );

        entity.Property(e => e.BrokerageAccountId).IsRequired();
        entity.Property(e => e.Ticker).IsRequired().HasMaxLength(10);
        entity.Property(e => e.Quantity).IsRequired().HasColumnType("int");
        entity.Property(e => e.AveragePrice).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.LastUpdate).IsRequired().HasColumnType("datetime");

        // Indexes for performance
        entity.HasIndex(e => e.BrokerageAccountId).HasDatabaseName("IX_Custodies_BrokerageAccountId");
    }
}
