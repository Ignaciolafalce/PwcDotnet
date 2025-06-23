using Microsoft.Net.Http.Headers;

namespace PwcDotnet.WebAPI.Extensions;

public static class EndpointCachingExtensions
{
    public static RouteHandlerBuilder WithResponseCache(this RouteHandlerBuilder builder, int seconds)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var httpContext = context.HttpContext;
            httpContext.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(seconds)
            };
            return await next(context);
        });
    }
}
