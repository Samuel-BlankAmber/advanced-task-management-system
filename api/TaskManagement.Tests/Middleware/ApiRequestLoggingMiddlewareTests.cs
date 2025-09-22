using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Api.Middleware;

namespace TaskManagement.Tests.Middleware;

public class ApiRequestLoggingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_LogsRequestAndCallsNext()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/test";
        context.Request.QueryString = new QueryString("?foo=bar");
        context.Request.Headers.UserAgent = "UnitTestAgent";
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");

        var loggerMock = new Mock<ILogger<ApiRequestLoggingMiddleware>>();
        var nextCalled = false;
        RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new ApiRequestLoggingMiddleware(next, loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
        loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("POST") && v.ToString()!.Contains("/api/test")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task LogToFileAsync_WhenDirectoryDoesNotExist_CreatesDirectoryAndLogs()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/dirtest";
        context.Request.Headers.UserAgent = "UnitTestAgent";
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");

        var loggerMock = new Mock<ILogger<ApiRequestLoggingMiddleware>>();
        var nextCalled = false;
        RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new ApiRequestLoggingMiddleware(next, loggerMock.Object);

        // Delete the log directory if it exists
        var logDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        if (Directory.Exists(logDir))
        {
            Directory.Delete(logDir, true);
        }

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Directory.Exists(logDir).Should().BeTrue();
        nextCalled.Should().BeTrue();
    }
}
