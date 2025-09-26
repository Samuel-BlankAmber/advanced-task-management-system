using TaskManagement.Api.Models;
using TaskManagement.Api.Models.Common;

namespace TaskManagement.Api.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<CursorPaginatedResult<TaskItem>> GetCursorPaginatedAsync(Guid? cursor = null, int pageSize = 10, Priority? priority = null, Status? status = null);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem?> UpdateAsync(Guid id, TaskItem task);
    Task<bool> DeleteAsync(Guid id);
    Task<StatusSummary> GetStatusSummaryAsync();
}
