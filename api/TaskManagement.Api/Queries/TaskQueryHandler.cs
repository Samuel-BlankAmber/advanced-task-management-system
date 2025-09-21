using TaskManagement.Api.Models;
using TaskManagement.Api.Repositories;

namespace TaskManagement.Api.Queries;

public interface ITaskQueryHandler
{
    Task<TaskItem?> HandleAsync(GetTaskByIdQuery query);
    Task<List<TaskItem>> HandleAsync(GetAllTasksQuery query);
}

public class TaskQueryHandler(ITaskRepository repository) : ITaskQueryHandler
{
    private readonly ITaskRepository _repository = repository;

    public async Task<TaskItem?> HandleAsync(GetTaskByIdQuery query)
    {
        if (query.Id == Guid.Empty)
        {
            return null;
        }

        return await _repository.GetByIdAsync(query.Id);
    }

    public async Task<List<TaskItem>> HandleAsync(GetAllTasksQuery query)
    {
        return await _repository.GetAllAsync();
    }
}
