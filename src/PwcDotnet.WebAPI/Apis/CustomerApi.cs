using PwcDotnet.WebAPI.Apis.Services;
using PwcDotnet.WebAPI.Auth;

namespace PwcDotnet.WebAPI.Apis;

public static class CustomerApi
{
    public static RouteGroupBuilder MapCustomerApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/customers").WithTags("Customers").RequireAuthorization(AppPolicies.AboveManagers);

        group.MapGet("/", GetAllAsync);
        group.MapPost("/register", RegisterAsync);

        return group;
    }

    private static async Task<IResult> GetAllAsync(
    [AsParameters] GetAllCustomersQuery query,
    CustomerServices services)
    {
        var result = await services.Mediator.Send(query);
        return TypedResults.Ok(result);
    }

    public static async Task<IResult> RegisterAsync(
        [FromBody] RegisterCustomerCommand command,
        CustomerServices services)
    {
        services.Logger.LogInformation("Registering customer {Email}", command.Email);

        var result = await services.Mediator.Send(command);

        if (result <= 0)
        {
            services.Logger.LogWarning("Customer registration failed for {Email}", command.Email);
            return TypedResults.Problem("Customer registration failed", statusCode: 500);
        }

        return TypedResults.Ok(new { customerId = result, command.Email, command.FullName });
    }
}
