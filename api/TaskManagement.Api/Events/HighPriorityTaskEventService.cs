using TaskManagement.Api.Models;

namespace TaskManagement.Api.Events;

public class HighPriorityTaskEvent
{
    public Guid TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TaskItem Task { get; set; } = null!;
}

public class HighPriorityTaskEventService(ILogger<HighPriorityTaskEventService> logger) : IHighPriorityTaskEventService
{
    private readonly ILogger<HighPriorityTaskEventService> _logger = logger;
    private readonly string _criticalLogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", "critical-high-priority-tasks.log");

    public async Task TriggerHighPriorityTaskEventAsync(TaskItem task, string action)
    {
        if (task.Priority != Priority.High)
        {
            throw new InvalidOperationException("Only high priority tasks can trigger high priority task events.");
        }

        var highPriorityEvent = new HighPriorityTaskEvent
        {
            TaskId = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Action = action,
            Task = task
        };

        await LogCriticalUpdateAsync(highPriorityEvent);
        _logger.LogWarning("CRITICAL: High priority task {Action}: {TaskId} - {Title} (Due: {DueDate})",
            action, task.Id, task.Title, task.DueDate);
    }

    private async Task LogCriticalUpdateAsync(HighPriorityTaskEvent eventData)
    {
        try
        {
            var logDirectory = Path.GetDirectoryName(_criticalLogFilePath);
            if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var logEntry =
                $"[{eventData.Timestamp:yyyy-MM-dd HH:mm:ss.fff} UTC] " +
                $"*** CRITICAL HIGH PRIORITY TASK {eventData.Action.ToUpper()} ***" + Environment.NewLine +
                $"Task ID: {eventData.TaskId}" + Environment.NewLine +
                $"Title: {eventData.Title}" + Environment.NewLine +
                $"Description: {eventData.Description}" + Environment.NewLine +
                $"Due Date: {eventData.DueDate:yyyy-MM-dd HH:mm:ss} UTC" + Environment.NewLine +
                $"Status: {eventData.Task.Status}" + Environment.NewLine +
                $"Priority: {eventData.Task.Priority}" + Environment.NewLine +
                $"Action: {eventData.Action}" + Environment.NewLine +
                new string('=', 80) + Environment.NewLine;

            await File.AppendAllTextAsync(_criticalLogFilePath, logEntry, System.Text.Encoding.UTF8);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write critical high priority task event to log file: {FilePath}", _criticalLogFilePath);
        }
    }
}
