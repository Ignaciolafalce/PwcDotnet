using PwcDotnet.Application.Commands.Auth;
using PwcDotnet.WebAPI.Apis.Services;

namespace PwcDotnet.WebAPI.Apis;

public static class AuthApi
{
    public static RouteGroupBuilder MapAuthApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Authentication"); // version???!

        group.MapPost("/login", LoginAsync);
        group.MapPost("/register", RegisterAsync);
        group.MapGet("/me", MeAsync).RequireAuthorization();

        return group;
    }

    public static async Task<IResult> LoginAsync(
        TokenCommand command,
        AuthServices services)
    {
        services.Logger.LogInformation("Login requested for {Email}", command.Email);
        var token = await services.Mediator.Send(command);
        return TypedResults.Ok(token);
    }

    public static async Task<IResult> RegisterAsync(
        RegisterCommand command,
        AuthServices services)
    {
        services.Logger.LogInformation("Registration requested for {Email}", command.Email);
        var token = await services.Mediator.Send(command);
        return TypedResults.Ok(token);
    }

    public static IResult MeAsync(AuthServices services)
    {
        if (string.IsNullOrEmpty(services.CurrentUser.UserId))
        {
            services.Logger.LogWarning("Unauthorized access to /me endpoint");
            return TypedResults.Unauthorized();
        }


        var userName = services.CurrentUser.UserName;
        var userId = services.CurrentUser.UserId;
        var expiration = services.CurrentUser.Expiration;
        var email = services.CurrentUser.Email;
        services.Logger.LogInformation("Info requested for User: {UserName} Id: {UserId}", userName, userId);
        return TypedResults.Ok(new
        {
            userId,
            userName,
            expiration,
            email
        });
    }
}
