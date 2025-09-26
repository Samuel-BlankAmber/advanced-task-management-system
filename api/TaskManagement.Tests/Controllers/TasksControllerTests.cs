using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.Api.Commands;
using TaskManagement.Api.Controllers;
using TaskManagement.Api.Models;
using TaskManagement.Api.Models.Common;
using TaskManagement.Api.Queries;

namespace TaskManagement.Tests.Controllers;

public class TasksControllerTests
{
    private readonly Mock<ITaskCommandHandler> _commandHandlerMock;
    private readonly Mock<ITaskQueryHandler> _queryHandlerMock;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _commandHandlerMock = new Mock<ITaskCommandHandler>();
        _queryHandlerMock = new Mock<ITaskQueryHandler>();
        _controller = new TasksController(_commandHandlerMock.Object, _queryHandlerMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithResult()
    {
        // Arrange
        var paginatedResult = new CursorPaginatedResult<TaskItem> { Items = [] };
        _queryHandlerMock.Setup(q => q.HandleAsync(It.IsAny<GetTasksQuery>())).ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().Be(paginatedResult);
    }

    [Fact]
    public async Task GetOne_ReturnsOk_WhenTaskExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var task = new TaskItem { Id = id };
        _queryHandlerMock.Setup(q => q.HandleAsync(It.Is<GetTaskByIdQuery>(g => g.Id == id))).ReturnsAsync(task);

        // Act
        var result = await _controller.GetOne(id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().Be(task);
    }

    [Fact]
    public async Task GetOne_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _queryHandlerMock.Setup(q => q.HandleAsync(It.Is<GetTaskByIdQuery>(g => g.Id == id))).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _controller.GetOne(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new TaskRequest { Title = "t", Description = "d", Priority = Priority.High, DueDate = DateTime.UtcNow, Status = Status.Pending };
        var createdTask = new TaskItem { Id = Guid.NewGuid(), Title = request.Title };
        _commandHandlerMock.Setup(c => c.HandleAsync(It.IsAny<CreateTaskCommand>())).ReturnsAsync(createdTask);

        // Act
        var result = await _controller.Create(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtAction = result.Result as CreatedAtActionResult;
        createdAtAction!.ActionName.Should().Be(nameof(_controller.GetOne));
        createdAtAction.Value.Should().Be(createdTask);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenTaskUpdated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new TaskRequest { Title = "t", Description = "d", Priority = Priority.High, DueDate = DateTime.UtcNow, Status = Status.Pending };
        var updatedTask = new TaskItem { Id = id, Title = request.Title };
        _commandHandlerMock.Setup(c => c.HandleAsync(It.IsAny<UpdateTaskCommand>())).ReturnsAsync(updatedTask);

        // Act
        var result = await _controller.Update(id, request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().Be(updatedTask);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenTaskNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new TaskRequest { Title = "t", Description = "d", Priority = Priority.High, DueDate = DateTime.UtcNow, Status = Status.Pending };
        _commandHandlerMock.Setup(c => c.HandleAsync(It.IsAny<UpdateTaskCommand>())).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _controller.Update(id, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _commandHandlerMock.Setup(c => c.HandleAsync(It.IsAny<DeleteTaskCommand>())).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenNotDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _commandHandlerMock.Setup(c => c.HandleAsync(It.IsAny<DeleteTaskCommand>())).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetSummary_ReturnsOk_WithStatusSummary()
    {
        // Arrange
        var summary = new StatusSummary(
            [
                new(Status.Pending, 2),
                new(Status.Completed, 1)
            ],
            total: 3
        );
        _queryHandlerMock.Setup(q => q.HandleAsync(It.IsAny<GetTasksSummaryQuery>())).ReturnsAsync(summary);

        // Act
        var result = await _controller.GetSummary();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var ok = result.Result as OkObjectResult;
        ok!.Value.Should().Be(summary);
    }
}
