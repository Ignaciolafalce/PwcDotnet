using PwcDotnet.Application.DTOs;
using PwcDotnet.WebAPI.Apis.Services;
using PwcDotnet.WebAPI.Auth;

namespace PwcDotnet.WebAPI.Apis;

public static class RentalApi
{
    public static RouteGroupBuilder MapRentalApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/rentals").WithTags("Rentals").RequireAuthorization(AppPolicies.AboveManagers);

        group.MapGet("/", GetAllAsync);
        group.MapPost("/register", RegisterAsync);
        group.MapPut("/modify", ModifyAsync);
        group.MapPut("/cancel", CancelAsync);

        return group;
    }
    private static async Task<IResult> GetAllAsync(
    [AsParameters] GetAllRentalsQuery query,
    RentalServices services)
    {
        services.Logger.LogInformation("Fetching all rentals");
        var result = await services.Mediator.Send(query);
        return TypedResults.Ok(result);
    }

    public static async Task<IResult> RegisterAsync([FromBody] RegisterRentalCommand command, RentalServices services)
    {
        services.Logger.LogInformation("Register rental command for carId {CarId}", command.CarId);

        var result = await services.Mediator.Send(command);

        if (result <= 0)
        {
            services.Logger.LogWarning("Rental registration failed");
            return TypedResults.Problem("Rental registration failed", statusCode: 500);
        }

        // For the responses we can:
        // Option A: Create WebApi DTOS
        // Option B: Add TResponse types (likes DTOS) to commands - i don't do this because i dont have time
        var rentalDto = new RentalDto { 
            Id = result,
            CustomerId = command.CustomerId,
            CarId = command.CarId,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
        };

        return TypedResults.Ok(rentalDto);
    }

    public static async Task<IResult> ModifyAsync([FromBody] ModifyRentalCommand command, RentalServices services)
    {
        services.Logger.LogInformation("Modifying rental {RentalId}", command.RentalId);

        var result = await services.Mediator.Send(command);

        if (!result)
        {
            services.Logger.LogWarning("Rental modification failed for {RentalId}", command.RentalId);
            return TypedResults.Problem("Rental modification failed", statusCode: 500);
        }

        return TypedResults.Ok(new { rentalId = command.RentalId, command.NewStartDate, command.NewEndDate, command.NewCarId, modified = result });
    }

    public static async Task<IResult> CancelAsync([AsParameters] CancelRentalCommand command, RentalServices services)
    {
        services.Logger.LogInformation("Cancelling rental {RentalId}", command.RentalId);

        var result = await services.Mediator.Send(command);

        if (!result)
        {
            services.Logger.LogWarning("Cancel failed for rental {RentalId}", command.RentalId);
            return TypedResults.Problem("Rental cancellation failed", statusCode: 500);
        }

        return TypedResults.Ok(new { rentalId = command.RentalId, canceled = result });
    }
}
