using TaskManagement.Api.Models;
using TaskManagement.Api.Models.Common;
using TaskManagement.Api.Repositories;

namespace TaskManagement.Api.Queries;

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

    public async Task<CursorPaginatedResult<TaskItem>> HandleAsync(GetTasksQuery query)
    {
        var pageSize = Math.Max(1, Math.Min(100, query.PageSize));

        return await _repository.GetCursorPaginatedAsync(query.Cursor, pageSize, query.Priority, query.Status);
    }

    public async Task<StatusSummary> HandleAsync(GetTasksSummaryQuery query)
    {
        return await _repository.GetStatusSummaryAsync();
    }
}
