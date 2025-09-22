using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Api.Events;
using TaskManagement.Api.Models;

namespace TaskManagement.Tests.Events;

public class HighPriorityTaskEventServiceTests
{
    private readonly Mock<ILogger<HighPriorityTaskEventService>> _loggerMock;
    private readonly HighPriorityTaskEventService _service;
    private readonly string _testLogFilePath;

    public HighPriorityTaskEventServiceTests()
    {
        _loggerMock = new Mock<ILogger<HighPriorityTaskEventService>>();
        _service = new HighPriorityTaskEventService(_loggerMock.Object);
        _testLogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "critical-high-priority-tasks.log");
    }

    [Fact]
    public async Task TriggerHighPriorityTaskEventAsync_ThrowsIfNotHighPriority()
    {
        // Arrange
        var task = new TaskItem { Id = Guid.NewGuid(), Priority = Priority.Low };
        Func<Task> act = async () => await _service.TriggerHighPriorityTaskEventAsync(task, "Created");
        // Act & Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Only high priority tasks can trigger high priority task events.");
    }

    [Fact]
    public async Task TriggerHighPriorityTaskEventAsync_LogsAndWritesFile_ForHighPriority()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Description = "Test Desc",
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.High,
            Status = Status.Pending
        };
        if (File.Exists(_testLogFilePath))
        {
            File.Delete(_testLogFilePath);
        }

        // Act
        await _service.TriggerHighPriorityTaskEventAsync(task, "Created");

        // Assert
        // Check log file written
        File.Exists(_testLogFilePath).Should().BeTrue();
        var logContent = await File.ReadAllTextAsync(_testLogFilePath, Encoding.UTF8);
        logContent.Should().Contain("CRITICAL HIGH PRIORITY TASK CREATED");
        logContent.Should().Contain(task.Title);
        // Check logger called
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("CRITICAL: High priority task")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
