using TaskManagement.Api.Models;

namespace TaskManagement.Api.Events;

public interface IHighPriorityTaskEventService
{
    Task TriggerHighPriorityTaskEventAsync(TaskItem task, string action);
}
