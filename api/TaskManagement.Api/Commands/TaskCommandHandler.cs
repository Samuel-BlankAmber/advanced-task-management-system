using TaskManagement.Api.Events;
using TaskManagement.Api.Models;
using TaskManagement.Api.Repositories;

namespace TaskManagement.Api.Commands;

public interface ITaskCommandHandler
{
    Task<TaskItem> HandleAsync(CreateTaskCommand command);
    Task<TaskItem?> HandleAsync(UpdateTaskCommand command);
    Task<bool> HandleAsync(DeleteTaskCommand command);
}

public class TaskCommandHandler(ITaskRepository repository, IHighPriorityTaskEventService eventService) : ITaskCommandHandler
{
    private readonly ITaskRepository _repository = repository;
    private readonly IHighPriorityTaskEventService _eventService = eventService;

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

        var createdTask = await _repository.CreateAsync(task);

        if (createdTask.Priority == Priority.High)
        {
            await _eventService.TriggerHighPriorityTaskEventAsync(createdTask, "Created");
        }

        return createdTask;
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

        var result = await _repository.UpdateAsync(command.Id, updatedTask);

        if (result != null && result.Priority == Priority.High)
        {
            await _eventService.TriggerHighPriorityTaskEventAsync(result, "Updated");
        }

        return result;
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
