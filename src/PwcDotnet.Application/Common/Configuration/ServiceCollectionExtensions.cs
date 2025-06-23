using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PwcDotnet.Application.Common.Behaviors;

namespace PwcDotnet.Application.Common.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {

        // Create a logger for Application startup
        using var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("ApplicationServiceCollectionExtensions");

        logger.LogInformation("Starting Application configuration...");

        services.AddMemoryCache();

        // CQRS con MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(IApplicationMarker).Assembly));

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(IApplicationMarker).Assembly);

        // Validation pipeline with FluentValidation + MediatR
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

        // Logging pipeline with FluentValidation + MediatR
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        logger.LogInformation("Application configuration completed successfully.");

        return services;
    }
}