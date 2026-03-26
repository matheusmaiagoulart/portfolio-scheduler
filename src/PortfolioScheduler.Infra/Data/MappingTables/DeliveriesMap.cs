using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioScheduler.Domain.Models;

namespace PortfolioScheduler.Infra.Data.MappingTables
{
    public class DeliveriesMap : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(e => e.Custody)
                .WithMany(c => c.Deliveries)
                .HasForeignKey(c => c.CustodyCustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.PurchaseOrder)
                .WithMany(p => p.Deliveries)
                .HasForeignKey(c => c.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.PurchaseOrderId).IsRequired();
            entity.Property(e => e.CustodyCustomerId).IsRequired();
            entity.Property(e => e.Ticker).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.DeliveryDate).IsRequired().HasColumnType("datetime");

            // Indexes for performance
            entity.HasIndex(e => e.PurchaseOrderId).HasDatabaseName("IX_Deliveries_PurchaseOrderId");
            entity.HasIndex(e => e.CustodyCustomerId).HasDatabaseName("IX_Deliveries_CustodyCustomerId");

            entity.ToTable("Deliveries");
        }
    }
}
