using Moq;
using TaskManagement.Api.Models;
using TaskManagement.Api.Queries;
using TaskManagement.Api.Repositories;
using TaskManagement.Api.Models.Common;

namespace TaskManagement.Tests.Queries;

public class TaskQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly TaskQueryHandler _handler;

    public TaskQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new TaskQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_GetTaskByIdQuery_ReturnsTaskItem()
    {
        // Arrange
        var id = Guid.NewGuid();
        var query = new GetTaskByIdQuery(id);
        var expectedTask = new TaskItem { Id = id };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(expectedTask);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTask.Id, result?.Id);
        _repositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_GetTasksQuery_ReturnsPaginatedResult()
    {
        // Arrange
        var query = new GetTasksQuery();
        var paginatedResult = new CursorPaginatedResult<TaskItem>
        {
            Items = [new TaskItem { Id = Guid.NewGuid() }],
            NextCursor = null
        };
        _repositoryMock.Setup(r => r.GetCursorPaginatedAsync(null, 10, null, null)).ReturnsAsync(paginatedResult);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        _repositoryMock.Verify(r => r.GetCursorPaginatedAsync(null, 10, null, null), Times.Once);
    }
}
