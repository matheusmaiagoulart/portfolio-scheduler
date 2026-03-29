using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Infra.Data.MappingTables
{
    public class PortfolioItemsMap : IEntityTypeConfiguration<PortfolioItem>
    {
        public void Configure(EntityTypeBuilder<PortfolioItem> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Ticker).IsRequired();
            entity.Property(e => e.Percentage).IsRequired().HasColumnType("decimal(5,2)");

            entity.ToTable("PortfolioItems");
        }
    }
}
