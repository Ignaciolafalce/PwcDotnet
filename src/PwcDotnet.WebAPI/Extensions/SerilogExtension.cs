using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using PwcDotnet.Application.Common.Exceptions;
using PwcDotnet.Domain.Exceptions;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Diagnostics;
using System.Net.WebSockets;

namespace PwcDotnet.WebAPI.Extensions;

public static class SerilogConfigExtension
{
    public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration().MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                              .MinimumLevel.Override("System", LogEventLevel.Warning)
                                              .MinimumLevel.Information()
                                              .Enrich.FromLogContext()
                                              .Enrich.WithEnvironmentName()
                                              .Enrich.WithProcessId()
                                              .Enrich.WithThreadId()
                                              .Enrich.With<ActivityEnricher>() // For TraceId y SpanId
                                              .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                                              .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                                              //.WriteTo.ApplicationInsights() we are not going to use this sink we are going to use opentelemetry later
                                              .CreateLogger();
        Log.Logger = logger;
        builder.Host.UseSerilog();

        // later change this to read from appsettings.json
        //builder.Host.UseSerilog((ctx, lc) =>
        //                            lc.ReadFrom.Configuration(ctx.Configuration)
        //                              .Enrich.FromLogContext()
        //                              .WriteTo.Console()
        //                        );
    }
}

public class ActivityEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));
        }
    }
}


public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        // We can refactor here...
        app.UseExceptionHandler(handler =>
        {
            handler.Run(async context =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<IExceptionHandlerFeature>>();

                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                context.Response.ContentType = "application/json";

                if (exception is ValidationException validationException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    var errors = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                    //var errorsList = errors.SelectMany(e => e.Value).ToList();
                    var res = TypedResults.ValidationProblem(errors);

                    logger.LogError(exception, "Validation error occurred: {Errors}", errors);
                    await context.Response.WriteAsJsonAsync(res);
                    return;
                }

                if (exception is RentalDomainException rentalDomainEx)
                {
                    context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

                    var res = TypedResults.UnprocessableEntity(rentalDomainEx.Message);

                    logger.LogError(exception, "Rental domain error occurred: {Message}", rentalDomainEx.Message);
                    await context.Response.WriteAsJsonAsync(res);
                    return;
                }

                if (exception is ForbiddenAccessException forbiddenEx)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    var res = TypedResults.Forbid();

                    logger.LogWarning(exception, "Forbidden access: {Message}", forbiddenEx.Message);
                    await context.Response.WriteAsJsonAsync(res);
                    return;
                }

                if (exception is NotFoundException notFoundEx)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    var res = TypedResults.NotFound(notFoundEx.Message);

                    logger.LogWarning(exception, "Resource not found: {Message}", notFoundEx.Message);
                    await context.Response.WriteAsJsonAsync(res);
                    return;
                }


                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var response = TypedResults.InternalServerError("An unexpected error occurred. Please try again later.");

                logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);
                await context.Response.WriteAsJsonAsync(response);
            });
        });

        return app;
    }
}