using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Repositories;

public class TaskRepository(TasksDb context) : ITaskRepository
{
    private readonly TasksDb _context = context;

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks.ToListAsync();
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
