using PwcDotnet.WebAPI.Apis.Services;
using PwcDotnet.WebAPI.Auth;

namespace PwcDotnet.WebAPI.Apis;

public static class DashboardApi
{
    public static RouteGroupBuilder MapDashboardApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/dashboard").WithTags("Dashboard").RequireAuthorization(AppPolicies.AboveManagers);

        group.MapGet("/top-used-cars", GetTopRentedCarsAsync);
        group.MapGet("/top-by-brand", GetTopCarsByBrandModelTypeAsync);
        group.MapGet("/daily-stats", GetDailyStatsAsync);

        return group;
    }

    public static async Task<IResult> GetTopRentedCarsAsync([AsParameters] GetTopRentedCarsQuery query, DashboardServices services)
    {
        services.Logger.LogInformation("Fetching top rented cars from {From} to {To} for location {LocationId}", query.FromDate, query.ToDate, query.LocationId);
        var result = await services.Mediator.Send(query);
        return TypedResults.Ok(result);
    }

    public static async Task<IResult> GetTopCarsByBrandModelTypeAsync([AsParameters] GetTopCarsByBrandModelTypeQuery query, DashboardServices services)
    {
        services.Logger.LogInformation("Fetching top cars by brand and model type from {From} to {To} for location {LocationId}", query.FromDate, query.ToDate, query.LocationId);
        var result = await services.Mediator.Send(query);
        return TypedResults.Ok(result);
    }

    public static async Task<IResult> GetDailyStatsAsync([AsParameters] GetDailyStatsQuery query, DashboardServices services)
    {
        services.Logger.LogInformation("Fetching daily stats from {From} to {To} for location {LocationId}", query.FromDate, query.ToDate, query.LocationId);
        var result = await services.Mediator.Send(query);
        return TypedResults.Ok(result);
    }
}
