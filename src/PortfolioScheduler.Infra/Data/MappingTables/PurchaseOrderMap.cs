using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioScheduler.Domain.Models;

namespace PortfolioScheduler.Infra.Data.MappingTables
{
    public class PurchaseOrderMap : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.MasterAccountId).IsRequired();
            entity.Property(e => e.Ticker).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(18,4)");
            entity.Property(e => e.MarketType).IsRequired().HasConversion<string>();
            entity.Property(e => e.ExecutionDate).IsRequired().HasColumnType("datetime");

            //Indexes for performance
            entity.HasIndex(e => e.Ticker).HasDatabaseName("IX_PurchaseOrder_Ticker");
            entity.HasIndex(e => e.ExecutionDate).HasDatabaseName("IX_PurchaseOrder_ExecutionDate");

            entity.ToTable("PurchaseOrders");
        }
    }
}
