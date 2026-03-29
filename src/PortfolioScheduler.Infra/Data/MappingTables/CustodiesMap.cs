using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Infra.Data.MappingTables
{
    public class CustodiesMap : IEntityTypeConfiguration<Custody>
    {
        public void Configure(EntityTypeBuilder<Custody> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.BrokerageAccountId).IsRequired();
            entity.Property(e => e.Ticker).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Quantity).IsRequired().HasColumnType("int");
            entity.Property(e => e.AveragePrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.LastUpdate).IsRequired().HasColumnType("datetime");

            // Indexes for performance
            entity.HasIndex(e => e.BrokerageAccountId).HasDatabaseName("IX_Custodies_BrokerageAccountId");
        }
    }
}
