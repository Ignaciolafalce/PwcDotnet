using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Trace;
using PwcDotnet.Application.Common.Auth.IdentityEntities;
using PwcDotnet.Application.Common.Configuration;
using PwcDotnet.Infrastructure.Common.Configuration;
using PwcDotnet.Infrastructure.Data;
using PwcDotnet.Infrastructure.Data.EF;
using PwcDotnet.WebAPI.Apis;
using PwcDotnet.WebAPI.Apis.Services;
using PwcDotnet.WebAPI.Auth;
using PwcDotnet.WebAPI.Extensions;
using Serilog;
using System;

namespace PwcDotnet.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddSerilogConfiguration();

        #region Services 

        Log.Information("Configuring Services");

        // CQRS - Just testing purpose otherwise must be modified
        builder.Services.AddCors(options => 
        {
            options.AddPolicy("AllowAllOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
        // Application and Infrastructure services
        builder.Services.AddApplication(builder.Configuration);
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddAppAuthorization();

        
        // OpenApi and Swagger services
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PwcDotnet Challenge API",
                Version = "v1",
                Description = "PWC Rental System",
                Contact = new OpenApiContact
                {
                    Name = "Ignacio Lafalce",
                    Email = "nacho_lafalce@hotmail.com"
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme
                {
                   Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                },
                new string[] { }
            }
          });

        });

        // Api services
        builder.Services.AddScoped<RentalServices>();
        builder.Services.AddScoped<AuthServices>();
        builder.Services.AddScoped<DashboardServices>();
        builder.Services.AddScoped<CustomerServices>();
        builder.Services.AddScoped<CarServices>();

        // Openlemetry services and configuration for tracing
        builder.Services.AddOpenTelemetry()
                        .WithTracing(tracing =>
                        {
                            tracing.AddAspNetCoreInstrumentation()
                                   .AddHttpClientInstrumentation()
                                   .AddSqlClientInstrumentation()
                                   .AddConsoleExporter();
                        });

        #endregion 


        var app = builder.Build();

        // Seed Task data
        Log.Information("Seeding Task Data");
        SeedDummyData(app);

        #region Http Request Pipeline Configuration - Middlewares

        Log.Information("Configuring Http Pipeline...");
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.UseSwagger();
            //app.UseSwaggerUI();
        }

        app.UseCors("AllowAllOrigins"); // Enable CORS policy just o allow all origins for testing purposes, most be Specific in production

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "PwcDotnet Challenge API v1");
            options.DocumentTitle = "PwcDotnet Challenge";
        });

        app.UseHttpsRedirection();

        app.UseCustomExceptionHandler(); // Custom exception handler middleware

        // Redirect root path to Swagger UI
        app.MapGet("/", () => Results.Redirect("/swagger"))
           .ExcludeFromDescription();

        //app.UseAuthorization();  In Application Layer, we have added the authorization policy

        app.MapAuthApi();
        app.MapRentalApi();
        app.MapDashboardApi();
        app.MapCustomerApi();
        app.MapCarApi();

        Log.Information("Starting App...");
        app.Run();

        #endregion
    }

    #region Private utilities

    // this could be in AddInfrastructe(...)? i prefer here. . .
    private static async void SeedDummyData(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await SeedData.SeedAdminUsersAsync(userManager, roleManager);

            var appDbContext = scope.ServiceProvider.GetRequiredService<RentalDbContext>();
            await SeedData.SeedDummyAsync(appDbContext, userManager, roleManager);

        }
    }
    #endregion

}