using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Api.Middleware;

namespace TaskManagement.Tests.Middleware;

public class GlobalExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenNoException_CallsNext()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        var nextCalled = false;
        RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new GlobalExceptionHandlingMiddleware(next, loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_HandlesExceptionAndLogs()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
        RequestDelegate next = ctx => throw new InvalidOperationException("Test error");
        var middleware = new GlobalExceptionHandlingMiddleware(next, loggerMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        context.Response.ContentType.Should().Be("application/problem+json");
        context.Response.Body.Position = 0;
        var response = await JsonDocument.ParseAsync(context.Response.Body);
        var root = response.RootElement;
        root.GetProperty("title").GetString().Should().Be("Internal Server Error");
        root.GetProperty("status").GetInt32().Should().Be(500);
        root.GetProperty("detail").GetString().Should().Contain("unexpected error");
        root.GetProperty("traceId").GetString().Should().NotBeNullOrEmpty();
        loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("Test error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
