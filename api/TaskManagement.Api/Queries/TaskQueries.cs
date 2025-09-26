using TaskManagement.Api.Models;

namespace TaskManagement.Api.Queries;

public record GetTaskByIdQuery(Guid Id);

public record GetTasksQuery(
    Priority? Priority = null, 
    Status? Status = null,
    Guid? Cursor = null,
    int PageSize = 10
);

public record GetTasksSummaryQuery();
