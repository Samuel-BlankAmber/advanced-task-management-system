using TaskManagement.Api.Models;

namespace TaskManagement.Api.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<List<TaskItem>> GetAllAsync();
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem?> UpdateAsync(Guid id, TaskItem task);
    Task<bool> DeleteAsync(Guid id);
}
