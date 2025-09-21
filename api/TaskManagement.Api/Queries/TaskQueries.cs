using TaskManagement.Api.Models;

namespace TaskManagement.Api.Queries;

public record GetTaskByIdQuery(Guid Id);

public record GetAllTasksQuery(Priority? Priority = null, Status? Status = null);
