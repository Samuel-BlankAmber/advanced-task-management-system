using System.Net;
using System.Text.Json;

namespace TaskManagement.Api.Middleware;

public class GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);
            await HandleExceptionAsync(context);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            title = "Internal Server Error",
            status = 500,
            detail = "An unexpected error occurred while processing the request.",
            traceId = context.TraceIdentifier,
        };

        var jsonResponse = JsonSerializer.Serialize(problemDetails, JsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }
}
