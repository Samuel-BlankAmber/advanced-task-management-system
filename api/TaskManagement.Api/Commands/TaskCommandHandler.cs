using TaskManagement.Api.Models;
using TaskManagement.Api.Repositories;

namespace TaskManagement.Api.Commands;

public interface ITaskCommandHandler
{
    Task<TaskItem> HandleAsync(CreateTaskCommand command);
    Task<TaskItem?> HandleAsync(UpdateTaskCommand command);
    Task<bool> HandleAsync(DeleteTaskCommand command);
}

public class TaskCommandHandler(ITaskRepository repository) : ITaskCommandHandler
{
    private readonly ITaskRepository _repository = repository;

    public async Task<TaskItem> HandleAsync(CreateTaskCommand command)
    {
        var task = new TaskItem
        {
            Title = command.Title,
            Description = command.Description,
            Priority = command.Priority,
            DueDate = command.DueDate,
            Status = command.Status,
        };

        return await _repository.CreateAsync(task);
    }

    public async Task<TaskItem?> HandleAsync(UpdateTaskCommand command)
    {
        if (command.Id == Guid.Empty)
        {
            return null;
        }

        var updatedTask = new TaskItem
        {
            Id = command.Id,
            Title = command.Title,
            Description = command.Description,
            Priority = command.Priority,
            DueDate = command.DueDate,
            Status = command.Status,
        };

        return await _repository.UpdateAsync(command.Id, updatedTask);
    }

    public async Task<bool> HandleAsync(DeleteTaskCommand command)
    {
        if (command.Id == Guid.Empty)
        {
            return false;
        }

        return await _repository.DeleteAsync(command.Id);
    }
}
