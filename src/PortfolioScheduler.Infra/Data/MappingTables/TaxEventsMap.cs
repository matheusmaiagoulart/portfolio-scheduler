using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Infra.Data.MappingTables
{
    public class TaxEventsMap : IEntityTypeConfiguration<TaxEvent>
    {
        public void Configure(EntityTypeBuilder<TaxEvent> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.Type).IsRequired().HasConversion<string>();
            entity.Property(e => e.BaseValue).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.TaxValue).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.IsSent).IsRequired();
            entity.Property(e => e.EventDate).IsRequired().HasColumnType("datetime");

            // Indexes for performance
            entity.HasIndex(e => e.IsSent).HasFilter("[IsSent] = 0").HasDatabaseName("IX_TaxEvents_NotSent"); // Index for unsent tax events
            entity.HasIndex(e => e.CustomerId).HasDatabaseName("IX_TaxEvents_CustomerId");

            entity.ToTable("TaxEvents");
        }
    }
}
