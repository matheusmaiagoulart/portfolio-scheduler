using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioScheduler.Domain.Models;

namespace PortfolioScheduler.Infra.Data.MappingTables
{
    public class AssetPricesMap : IEntityTypeConfiguration<AssetPrices>
    {
        public void Configure(EntityTypeBuilder<AssetPrices> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.TradingDate).IsRequired();
            entity.Property(e => e.Ticker).IsRequired().HasMaxLength(10);
            entity.Property(e => e.OpenPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ClosePrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MaxPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MinPrice).HasColumnType("decimal(18, 4)");

            // Indexes for performance
            entity.HasIndex(e => e.Ticker).HasDatabaseName("IX_AssetPrices_Ticker");

            entity.ToTable("AssetPrices"); 
        }
    }
}
