using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;
using TaskManagement.Api.Repositories;

namespace TaskManagement.Tests.Repositories;

public class TaskRepositoryTests : IDisposable
{
    private readonly TasksDb _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryTests()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();

        var options = new DbContextOptionsBuilder<TasksDb>()
            .UseSqlite(conn)
            .EnableSensitiveDataLogging()
            .Options;

        _context = new TasksDb(options);
        _context.Database.EnsureCreated();
        _repository = new TaskRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private TaskItem CreateSampleTask(
        string title = "Test Task",
        string description = "Test Description",
        Priority priority = Priority.Medium,
        Status status = Status.Pending,
        DateTime? dueDate = null)
    {
        return new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Priority = priority,
            Status = status,
            DueDate = dueDate ?? DateTime.UtcNow.Date
        };
    }

    private async Task SeedDatabase(params TaskItem[] tasks)
    {
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnTask()
    {
        // Arrange
        var task = CreateSampleTask();
        await SeedDatabase(task);

        // Act
        var result = await _repository.GetByIdAsync(task.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(task.Id);
        result.Title.Should().Be(task.Title);
        result.Description.Should().Be(task.Description);
        result.Priority.Should().Be(task.Priority);
        result.Status.Should().Be(task.Status);
        result.DueDate.Should().Be(task.DueDate);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidTask_ShouldCreateAndReturnTask()
    {
        // Arrange
        var task = CreateSampleTask();

        // Act
        var result = await _repository.CreateAsync(task);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(task.Id);
        result.Title.Should().Be(task.Title);

        // Verify task was saved to database
        var savedTask = await _context.Tasks.FindAsync(task.Id);
        savedTask.Should().NotBeNull();
        savedTask.Title.Should().Be(task.Title);
        savedTask.Description.Should().Be(task.Description);
    }

    [Fact]
    public async Task CreateAsync_WithMultipleTasks_ShouldCreateAll()
    {
        // Arrange
        var task1 = CreateSampleTask("Task 1");
        var task2 = CreateSampleTask("Task 2");

        // Act
        await _repository.CreateAsync(task1);
        await _repository.CreateAsync(task2);

        // Assert
        var allTasks = await _context.Tasks.ToListAsync();
        allTasks.Should().HaveCount(2);
        allTasks.Select(t => t.Title).Should().Contain(new[] { "Task 1", "Task 2" });
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithExistingTask_ShouldUpdateAndReturnTask()
    {
        // Arrange
        var originalTask = CreateSampleTask("Original Title", "Original Description", Priority.Low, Status.Pending);
        await SeedDatabase(originalTask);

        var updatedTask = CreateSampleTask("Updated Title", "Updated Description", Priority.High, Status.InProgress);

        // Act
        var result = await _repository.UpdateAsync(originalTask.Id, updatedTask);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(originalTask.Id);
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
        result.Priority.Should().Be(Priority.High);
        result.Status.Should().Be(Status.InProgress);

        // Verify changes were persisted
        var savedTask = await _context.Tasks.FindAsync(originalTask.Id);
        savedTask.Should().NotBeNull();
        savedTask.Title.Should().Be("Updated Title");
        savedTask.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingTask_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var updateTask = CreateSampleTask("Updated Title");

        // Act
        var result = await _repository.UpdateAsync(nonExistingId, updateTask);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldPreserveOriginalId()
    {
        // Arrange
        var originalTask = CreateSampleTask();
        await SeedDatabase(originalTask);

        var updateTask = CreateSampleTask("Updated Title");
        updateTask.Id = Guid.NewGuid();

        // Act
        var result = await _repository.UpdateAsync(originalTask.Id, updateTask);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(originalTask.Id);
        result.Id.Should().NotBe(updateTask.Id);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithExistingTask_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var task = CreateSampleTask();
        await SeedDatabase(task);

        // Act
        var result = await _repository.DeleteAsync(task.Id);

        // Assert
        result.Should().BeTrue();

        // Verify task was deleted
        var deletedTask = await _context.Tasks.FindAsync(task.Id);
        deletedTask.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingTask_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistingId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldOnlyDeleteSpecifiedTask()
    {
        // Arrange
        var task1 = CreateSampleTask("Task 1");
        var task2 = CreateSampleTask("Task 2");
        await SeedDatabase(task1, task2);

        // Act
        var result = await _repository.DeleteAsync(task1.Id);

        // Assert
        result.Should().BeTrue();

        // Verify only task1 was deleted
        var remainingTask = await _context.Tasks.FindAsync(task2.Id);
        remainingTask.Should().NotBeNull();
        remainingTask.Title.Should().Be("Task 2");

        var deletedTask = await _context.Tasks.FindAsync(task1.Id);
        deletedTask.Should().BeNull();
    }

    #endregion

    #region GetCursorPaginatedAsync Tests

    [Fact]
    public async Task GetCursorPaginatedAsync_WithNoCursor_ShouldReturnFirstPage()
    {
        // Arrange
        var tasks = Enumerable.Range(1, 5)
            .Select(i => CreateSampleTask($"Task {i}"))
            .OrderBy(t => t.Id)
            .ToArray();
        await SeedDatabase(tasks);

        // Act
        var result = await _repository.GetCursorPaginatedAsync(pageSize: 3);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.PageSize.Should().Be(3);
        result.HasNextPage.Should().BeTrue();
        result.NextCursor.Should().NotBeNull();

        // Items should be ordered by ID
        var orderedIds = tasks.OrderBy(t => t.Id).Take(3).Select(t => t.Id).ToList();
        result.Items.Select(t => t.Id).Should().BeEquivalentTo(orderedIds, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetCursorPaginatedAsync_WithCursor_ShouldReturnNextPage()
    {
        // Arrange
        var tasks = Enumerable.Range(1, 5)
            .Select(i => CreateSampleTask($"Task {i}"))
            .OrderBy(t => t.Id)
            .ToArray();
        await SeedDatabase(tasks);

        var firstCursor = tasks.OrderBy(t => t.Id).First().Id;

        // Act
        var result = await _repository.GetCursorPaginatedAsync(cursor: firstCursor, pageSize: 2);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.HasNextPage.Should().BeTrue();

        // Should return tasks after the cursor
        var expectedIds = tasks.OrderBy(t => t.Id).Skip(1).Take(2).Select(t => t.Id);
        result.Items.Select(t => t.Id).Should().BeEquivalentTo(expectedIds, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetCursorPaginatedAsync_LastPage_ShouldHaveNoNextPage()
    {
        // Arrange
        var tasks = Enumerable.Range(1, 3)
            .Select(i => CreateSampleTask($"Task {i}"))
            .ToArray();
        await SeedDatabase(tasks);

        // Act
        var result = await _repository.GetCursorPaginatedAsync(pageSize: 5);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.HasNextPage.Should().BeFalse();
        result.NextCursor.Should().BeNull();
    }

    [Fact]
    public async Task GetCursorPaginatedAsync_FilterByPriority_ShouldReturnFilteredTasks()
    {
        // Arrange
        var highPriorityTask = CreateSampleTask("High Priority", priority: Priority.High);
        var mediumPriorityTask = CreateSampleTask("Medium Priority", priority: Priority.Medium);
        var lowPriorityTask = CreateSampleTask("Low Priority", priority: Priority.Low);

        await SeedDatabase(highPriorityTask, mediumPriorityTask, lowPriorityTask);

        // Act
        var result = await _repository.GetCursorPaginatedAsync(priority: Priority.High);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Priority.Should().Be(Priority.High);
        result.Items.First().Title.Should().Be("High Priority");
    }

    [Fact]
    public async Task GetCursorPaginatedAsync_FilterByStatus_ShouldReturnFilteredTasks()
    {
        // Arrange
        var pendingTask = CreateSampleTask("Pending Task", status: Status.Pending);
        var inProgressTask = CreateSampleTask("In Progress Task", status: Status.InProgress);
        var completedTask = CreateSampleTask("Completed Task", status: Status.Completed);

        await SeedDatabase(pendingTask, inProgressTask, completedTask);

        // Act
        var result = await _repository.GetCursorPaginatedAsync(status: Status.InProgress);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Status.Should().Be(Status.InProgress);
        result.Items.First().Title.Should().Be("In Progress Task");
    }

    [Fact]
    public async Task GetCursorPaginatedAsync_FilterByPriorityAndStatus_ShouldReturnFilteredTasks()
    {
        // Arrange
        var task1 = CreateSampleTask("Task 1", priority: Priority.High, status: Status.Pending);
        var task2 = CreateSampleTask("Task 2", priority: Priority.High, status: Status.InProgress);
        var task3 = CreateSampleTask("Task 3", priority: Priority.Low, status: Status.Pending);

        await SeedDatabase(task1, task2, task3);

        // Act
        var result = await _repository.GetCursorPaginatedAsync(priority: Priority.High, status: Status.Pending);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Priority.Should().Be(Priority.High);
        result.Items.First().Status.Should().Be(Status.Pending);
        result.Items.First().Title.Should().Be("Task 1");
    }

    [Fact]
    public async Task GetCursorPaginatedAsync_EmptyDatabase_ShouldReturnEmptyResult()
    {
        // Act
        var result = await _repository.GetCursorPaginatedAsync();

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.HasNextPage.Should().BeFalse();
        result.NextCursor.Should().BeNull();
    }

    [Fact]
    public async Task GetCursorPaginatedAsync_NoMatchingFilters_ShouldReturnEmptyResult()
    {
        // Arrange
        var task = CreateSampleTask(priority: Priority.Low, status: Status.Pending);
        await SeedDatabase(task);

        // Act
        var result = await _repository.GetCursorPaginatedAsync(priority: Priority.High, status: Status.Completed);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.HasNextPage.Should().BeFalse();
        result.NextCursor.Should().BeNull();
    }

    #endregion
}
