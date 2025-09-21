using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;
using TaskManagement.Api.Models.Common;

namespace TaskManagement.Api.Repositories;

public class TaskRepository(TasksDb context) : ITaskRepository
{
    private readonly TasksDb _context = context;

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<CursorPaginatedResult<TaskItem>> GetCursorPaginatedAsync(Guid? cursor = null, int pageSize = 10, Priority? priority = null, Status? status = null)
    {
        var query = _context.Tasks.AsQueryable();

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (cursor.HasValue)
        {
            query = query.Where(t => t.Id.CompareTo(cursor.Value) > 0);
        }

        query = query.OrderBy(t => t.Id);

        var items = await query
            .Take(pageSize + 1)
            .ToListAsync();

        var hasNextPage = items.Count > pageSize;

        if (hasNextPage)
        {
            items.RemoveAt(items.Count - 1);
        }

        var nextCursor = hasNextPage && items.Count > 0 ? items.Last().Id : (Guid?)null;

        return new CursorPaginatedResult<TaskItem>(
            items,
            pageSize,
            hasNextPage,
            nextCursor
        );
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateAsync(Guid id, TaskItem task)
    {
        var existingTask = await _context.Tasks.FindAsync(id);
        if (existingTask == null) return null;

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.Priority = task.Priority;
        existingTask.DueDate = task.DueDate;
        existingTask.Status = task.Status;

        await _context.SaveChangesAsync();
        return existingTask;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
}
