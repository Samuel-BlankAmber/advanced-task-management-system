using TaskManagement.Api.Models;

namespace TaskManagement.Api.Commands;

public record CreateTaskCommand(
    string Title,
    string Description,
    Priority Priority,
    DateTime DueDate,
    Status Status
);

public record UpdateTaskCommand(
    Guid Id,
    string Title,
    string Description,
    Priority Priority,
    DateTime DueDate,
    Status Status
);

public record DeleteTaskCommand(Guid Id);
