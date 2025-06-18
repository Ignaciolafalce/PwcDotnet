using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PwcDotnet.Application.Common.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwcDotnet.Application.Common.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();

        // CQRS con MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(executingAssembly));

        // FluentValidation
        services.AddValidatorsFromAssembly(executingAssembly);

        // Validation pipeline with FluentValidation + MediatR
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));


        return services;
    }
}