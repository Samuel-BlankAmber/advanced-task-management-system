using Moq;
using TaskManagement.Api.Commands;
using TaskManagement.Api.Models;
using TaskManagement.Api.Repositories;
using TaskManagement.Api.Events;

namespace TaskManagement.Tests.Commands;

public class TaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<IHighPriorityTaskEventService> _eventServiceMock;
    private readonly TaskCommandHandler _handler;

    public TaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _eventServiceMock = new Mock<IHighPriorityTaskEventService>();
        _handler = new TaskCommandHandler(_repositoryMock.Object, _eventServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_CreateTaskCommand_ReturnsCreatedTask()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Test Title",
            "Test Description",
            Priority.High,
            DateTime.UtcNow.AddDays(1),
            Status.Pending
        );
        var expectedTask = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            Priority = command.Priority,
            DueDate = command.DueDate,
            Status = command.Status
        };
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<TaskItem>())).ReturnsAsync(expectedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.Equal(expectedTask.Id, result.Id);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UpdateTaskCommand_ReturnsUpdatedTask()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new UpdateTaskCommand(
            id,
            "Updated Title",
            "Updated Description",
            Priority.Medium,
            DateTime.UtcNow.AddDays(2),
            Status.InProgress
        );
        var expectedTask = new TaskItem
        {
            Id = id,
            Title = command.Title,
            Description = command.Description,
            Priority = command.Priority,
            DueDate = command.DueDate,
            Status = command.Status
        };
        _repositoryMock.Setup(r => r.UpdateAsync(id, It.IsAny<TaskItem>())).ReturnsAsync(expectedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.Equal(expectedTask.Id, result?.Id);
        _repositoryMock.Verify(r => r.UpdateAsync(id, It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeleteTaskCommand_ReturnsTrueOnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new DeleteTaskCommand(id);
        _repositoryMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }
}
