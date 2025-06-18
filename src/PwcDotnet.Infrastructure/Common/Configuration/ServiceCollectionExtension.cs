using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PwcDotnet.Application;
using PwcDotnet.Application.Interfaces;
using PwcDotnet.Domain;
using PwcDotnet.Domain.AggregatesModel.CarAggregate;
using PwcDotnet.Domain.AggregatesModel.CustomerAggregate;
using PwcDotnet.Domain.AggregatesModel.RentalAggregate;
using PwcDotnet.Infrastructure.Auth;
using PwcDotnet.Infrastructure.Data.EF;
using PwcDotnet.Infrastructure.Repositories;
using System.Text;

namespace PwcDotnet.Infrastructure.Common.Configuration;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        // Create a logger for infrastructure startup
        using var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("InfrastructureServiceCollectionExtensions");

        logger.LogInformation("Starting Infrastructure configuration...");

        // EF - Database Context
        var useInMemoryDb = bool.Parse(configuration.GetSection("UseInMemoryDb").Value?.ToLower() ?? "false");
        services.AddDbContext<RentalDbContext>(options =>
        {
            logger.LogInformation("Database - Using in memory?: " + configuration.GetValue<bool>("UseInMemoryDb"));

            if (useInMemoryDb)
            {
                options.UseInMemoryDatabase("RentalChallengeDb");
                logger.LogInformation("Database - InMemory: RentalChallengeDb");
            }

            if (!useInMemoryDb)
            {
                options.UseSqlServer(configuration.GetConnectionString("RentalConnectionString"));
                logger.LogInformation("Database - ConnectionString: " + configuration.GetConnectionString("NativoConnectionString"));
            }
        });

        //services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<RentalDbContext>());

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IDomainMarker).Assembly));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationMarker).Assembly));


        // Register repositories
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();

        // Register application services
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        //  Register Identity services
        services.AddIdentityCore<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<RentalDbContext>()
                .AddDefaultTokenProviders();

        // Configure Identity options
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {

                var jwtSection = configuration.GetSection("Jwt");

                var key = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration");

                var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is missing in configuration");

                var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("JWT Audience is missing in configuration");

                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true
                };

            });

        // Configure authorization policies if needed
        services.AddAuthorization();

        // Add HttpContextAccessor to access the current HTTP context
        services.AddHttpContextAccessor();

        logger.LogInformation("Infrastructure configuration completed successfully.");

        return services;
    }
}
