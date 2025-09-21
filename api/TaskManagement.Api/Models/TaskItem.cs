namespace TaskManagement.Api.Models;

public enum Priority { None = 0, Low = 1, Medium = 2, High = 3 }
public enum Status { Pending = 0, InProgress = 1, Completed = 2, Archived = 3 }

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.None;
    public DateTime DueDate { get; set; } = DateTime.UtcNow.Date;
    public Status Status { get; set; } = Status.Pending;
}
