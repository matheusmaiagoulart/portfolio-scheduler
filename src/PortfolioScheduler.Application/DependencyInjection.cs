using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using PortfolioScheduler.Application.Behaviors;
using PortfolioScheduler.Application.Services.Interfaces;
using PortfolioScheduler.Application.Services;
using PortfolioScheduler.Domain.Services.Interfaces;
using PortfolioScheduler.Domain.Services.Implementations;

namespace PortfolioScheduler.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Add all classes that implement IValidator<T> as transient services
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        // Fluent Validation
        services.AddValidatorsFromAssembly(assembly);

        // Service
        services.AddScoped<IProcessDistributionInBatch, ProcessDistributionInBatch>();
        services.AddScoped<IPortfolioRebalancer, PortfolioRebalancer>();

        return services;
    }
}
