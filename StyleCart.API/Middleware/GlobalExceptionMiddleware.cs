using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StyleCart.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogWarning(exception, "A business-rule validation error occurred.");

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Invalid request",
                exception.Message);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(exception, "A database concurrency conflict occurred.");

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status409Conflict,
                "Concurrency conflict",
                "The data was changed by another request. Please try again.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred.");

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Server error",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}