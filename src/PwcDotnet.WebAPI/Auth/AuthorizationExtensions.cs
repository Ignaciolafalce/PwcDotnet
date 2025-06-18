using PwcDotnet.Application.Common.Auth;

namespace PwcDotnet.WebAPI.Auth;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AppPolicies.OnlyAdmin, policy => policy.RequireRole(RoleConstants.Admin));
            options.AddPolicy(AppPolicies.AboveManagers, policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.Manager));
        });
        return services;
    }
}

public class AppPolicies
{
    public const string OnlyAdmin = "OnlyAdmin";
    public const string AboveManagers = "OnlyAdmin";
}