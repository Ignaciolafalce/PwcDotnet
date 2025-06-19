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
