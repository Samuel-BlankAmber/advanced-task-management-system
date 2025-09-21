using TaskManagement.Api.Models;
using TaskManagement.Api.Models.Common;

namespace TaskManagement.Api.Queries;

public interface ITaskQueryHandler
{
    Task<TaskItem?> HandleAsync(GetTaskByIdQuery query);
    Task<CursorPaginatedResult<TaskItem>> HandleAsync(GetTasksQuery query);
}
