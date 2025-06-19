using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using PwcDotnet.Application.Common.Exceptions;
using PwcDotnet.Domain.Exceptions;

namespace PwcDotnet.WebAPI.Extensions;

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