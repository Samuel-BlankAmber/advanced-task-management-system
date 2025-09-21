using TaskManagement.Api.Models;

namespace TaskManagement.Api.Commands;

public interface ITaskCommandHandler
{
    Task<TaskItem> HandleAsync(CreateTaskCommand command);
    Task<TaskItem?> HandleAsync(UpdateTaskCommand command);
    Task<bool> HandleAsync(DeleteTaskCommand command);
}
