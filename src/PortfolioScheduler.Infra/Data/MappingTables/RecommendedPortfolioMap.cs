using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioScheduler.Domain.Models;

namespace PortfolioScheduler.Infra.Data.MappingTables
{
    public class RecommendedPortfolioMap : IEntityTypeConfiguration<RecommendedPortfolio>

    {
        public void Configure(EntityTypeBuilder<RecommendedPortfolio> entity)
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Active).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.TerminationDate).IsRequired(false);

            entity.ToTable("RecommendedPortfolios");
        }
    }
}
