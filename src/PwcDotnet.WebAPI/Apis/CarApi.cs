using PwcDotnet.WebAPI.Apis.Services;

namespace PwcDotnet.WebAPI.Apis;

public static class CarApi
{
    public static RouteGroupBuilder MapCarApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/cars").WithTags("Cars").RequireAuthorization();

        group.MapGet("/availability", CheckAvailabilityAsync);
        group.MapGet("/upcoming-services", GetUpcomingServicesAsync);

        return group;
    }

    public static async Task<IResult> CheckAvailabilityAsync(
        [AsParameters] CheckCarAvailabilityQuery query,
        CarServices services)
    {
        var result = await services.Mediator.Send(query);
        return TypedResults.Ok(result);
    }

    public static async Task<IResult> GetUpcomingServicesAsync(
        [AsParameters] GetUpcomingCarServicesQuery query,
        CarServices services)
    {
        var result = await services.Mediator.Send(query);
        return TypedResults.Ok(result);
    }
}
