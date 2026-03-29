using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioScheduler.Domain.Repositories;
using PortfolioScheduler.Infra.Data.Context;
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

        return services;
    }
}


