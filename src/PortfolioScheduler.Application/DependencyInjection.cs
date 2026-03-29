using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using PortfolioScheduler.Application.Behaviors;

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

        return services;
    }
}
