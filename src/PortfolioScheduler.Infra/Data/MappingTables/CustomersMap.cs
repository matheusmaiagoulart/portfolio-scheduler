using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioScheduler.Domain.Entities;

namespace PortfolioScheduler.Infra.Data.MappingTables;

public class CustomersMap : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> entity)
    {
        entity.HasKey(e => e.Id); // Primary Key
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.HasOne(e => e.BrokerageAccount)
            .WithOne()
            .HasForeignKey<BrokerageAccount>(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasData(new
        {
            Id = 1L,
            Name = "CORRETORA - CONTA MASTER",
            Cpf = "00000000000",
            Email = "master@itaucorretora.com.br",
            MonthlyAmount = 0m,
            Active = true,
            JoiningDate = new DateTime(2026, 1, 1)
        });

        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Cpf).IsRequired().HasMaxLength(11).IsUnicode(true);
        entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
        entity.Property(e => e.MonthlyAmount).IsRequired().HasPrecision(18, 2);
        entity.Property(e => e.Active).IsRequired();
        entity.Property(e => e.JoiningDate).IsRequired();

        // Indexes for performance
        entity.HasIndex(e => e.Cpf).IsUnique().HasDatabaseName("IX_Customers_Cpf");
        entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Customers_Email");
        entity.HasIndex(e => e.Active).HasDatabaseName("IX_Customers_Active");

        entity.ToTable("Customers");
    }
}
