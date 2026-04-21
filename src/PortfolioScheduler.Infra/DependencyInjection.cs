using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Domain.Services.Implementations;
using PortfolioScheduler.Domain.Services.Interfaces;
using PortfolioScheduler.Infra.Data.Context;
using PortfolioScheduler.Infra.ExternalData.B3;
using PortfolioScheduler.Infra.Persistence;

namespace PortfolioScheduler;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PortfolioScheduler-Connection")));

        // Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAssetPricesRepository, AssetPricesRepository>();
        services.AddScoped<IPurchaseOrdersRepository, PurchaseOrdersRepository>();
        services.AddScoped<IRecommendedPortfolioRepository, RecommendedPortfolioRepository>();

        services.AddScoped<ICotahistParser, CotahistParser>();

        // Domain Services
        services.AddScoped<IPurchaseExecutionDateValidator, PurchaseExecutionDateValidator>();
        services.AddScoped<IPurchaseQuotesCalculator, PurchaseQuotesCalculator>();
        services.AddScoped<IPortfolioDistribuition, PortfolioDistribuition>();

        return services;
    }
}


