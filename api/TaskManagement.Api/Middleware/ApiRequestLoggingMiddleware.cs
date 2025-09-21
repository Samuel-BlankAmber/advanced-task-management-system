using System.Text;

namespace TaskManagement.Api.Middleware;

public class ApiRequestLoggingMiddleware(RequestDelegate next, ILogger<ApiRequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ApiRequestLoggingMiddleware> _logger = logger;
    private readonly string _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "api-requests.log");

    public async Task InvokeAsync(HttpContext context)
    {
        var timestamp = DateTime.UtcNow;
        var method = context.Request.Method;
        var endpoint = context.Request.Path + context.Request.QueryString;
        var userAgent = context.Request.Headers.UserAgent.FirstOrDefault() ?? "Unknown";
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var logEntry =
            $"[{timestamp:yyyy-MM-dd HH:mm:ss.fff} UTC] " +
            $"Method: {method} | " +
            $"Endpoint: {endpoint} | " +
            $"Client IP: {clientIp} | " +
            $"User-Agent: {userAgent}";

        await LogToFileAsync(logEntry);
        _logger.LogInformation("API Request: {Method} {Endpoint} from {ClientIp}",
            method, endpoint, clientIp);

        await _next(context);
    }

    private async Task LogToFileAsync(string logEntry)
    {
        try
        {
            var logDirectory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            await File.AppendAllTextAsync(_logFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write to API request log file: {FilePath}", _logFilePath);
        }
    }
}
