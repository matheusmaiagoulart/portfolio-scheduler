using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Infra.Data.MappingTables;

public class BrokerageAccountsMap : IEntityTypeConfiguration<BrokerageAccount>
{
    public void Configure(EntityTypeBuilder<BrokerageAccount> entity)
    {
        entity.HasKey(e => e.Id); // Primary Key
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.HasMany(e => e.Custodies)
            .WithOne()
            .HasForeignKey(e => e.BrokerageAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasData(new
        {
            Id = 1L,
            CustomerId = 1L,
            AccountNumber = "MASTER-ACCOUNT-0001",
            AccountType = BrokerageAccountType.MASTER,
            CreatedAt = new DateTime(2026, 1, 1)
        });

        entity.Property(e => e.CustomerId).IsRequired();
        entity.Property(e => e.AccountNumber).IsRequired().IsUnicode(true);
        entity.Property(e => e.AccountType).IsRequired().HasConversion<string>();
        entity.Property(e => e.CreatedAt).IsRequired();

        // Indexes for performance
        entity.HasIndex(e => e.CustomerId).HasDatabaseName("IX_BrokerageAccounts_CustomerId");
        entity.HasIndex(e => e.AccountNumber).HasDatabaseName("IX_BrokerageAccounts_AccountNumber");

        entity.ToTable("BrokerageAccounts");
    }
}
