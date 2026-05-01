using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioScheduler.Application.Queries.Contracts;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Domain.Services.Implementations;
using PortfolioScheduler.Domain.Services.Interfaces;
using PortfolioScheduler.Infra.Data.Context;
using PortfolioScheduler.Infra.ExternalData.B3;
using PortfolioScheduler.Infra.Persistence;
using PortfolioScheduler.Infra.Queries;

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

        // Repositories Persistence
        services.AddScoped<IAppDbContext, AppDbContext>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<IAssetPricesRepository, AssetPricesRepository>();
        services.AddScoped<IPurchaseOrdersRepository, PurchaseOrdersRepository>();
        services.AddScoped<IRecommendedPortfolioRepository, RecommendedPortfolioRepository>();

        // Repositories Queries
        services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();

        services.AddScoped<ICotahistParser, CotahistParser>();

        // Domain Services
        services.AddScoped<IPurchaseExecutionDateValidator, PurchaseExecutionDateValidator>();
        services.AddScoped<IPurchaseQuotesCalculator, PurchaseQuotesCalculator>();
        services.AddScoped<IPortfolioDistribution, PortfolioDistribution>();

        return services;
    }
}


