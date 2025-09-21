using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Data;

public class TasksDb(DbContextOptions<TasksDb> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<TaskItem>().HasKey(x => x.Id);
        b.Entity<TaskItem>().Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Entity<TaskItem>().Property(x => x.Description).IsRequired().HasMaxLength(1000);
        b.Entity<TaskItem>().Property(x => x.Priority).IsRequired();
        b.Entity<TaskItem>().Property(x => x.DueDate).IsRequired();
        b.Entity<TaskItem>().Property(x => x.Status).IsRequired();
    }
}
