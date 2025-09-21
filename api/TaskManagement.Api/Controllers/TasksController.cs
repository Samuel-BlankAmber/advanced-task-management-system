using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Commands;
using TaskManagement.Api.Models;
using TaskManagement.Api.Models.Common;
using TaskManagement.Api.Queries;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("tasks")]
[Authorize]
public class TasksController(ITaskCommandHandler commandHandler, ITaskQueryHandler queryHandler) : ControllerBase
{
    private readonly ITaskCommandHandler _commandHandler = commandHandler;
    private readonly ITaskQueryHandler _queryHandler = queryHandler;

    [HttpGet]
    [Authorize("read:tasks")]
    public async Task<ActionResult<CursorPaginatedResult<TaskItem>>> GetAll(
        [FromQuery] Priority? priority = null,
        [FromQuery] Status? status = null,
        [FromQuery] Guid? cursor = null,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetTasksQuery(priority, status, cursor, pageSize);
        var result = await _queryHandler.HandleAsync(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize("read:tasks")]
    public async Task<ActionResult<TaskItem>> GetOne(Guid id)
    {
        var query = new GetTaskByIdQuery(id);
        var task = await _queryHandler.HandleAsync(query);
        if (task is not null)
        {
            return Ok(task);
        }
        return NotFound();
    }

    [HttpPost]
    [Authorize("write:tasks")]
    public async Task<ActionResult<TaskItem>> Create(TaskRequest request)
    {
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.Status
        );
        var task = await _commandHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetOne), new { id = task.Id }, task);
    }

    [HttpPut("{id:guid}")]
    [Authorize("write:tasks")]
    public async Task<ActionResult<TaskItem>> Update(Guid id, TaskRequest request)
    {
        var command = new UpdateTaskCommand(
            id,
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.Status
        );
        var task = await _commandHandler.HandleAsync(command);
        if (task is not null)
        {
            return Ok(task);
        }
        return NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize("write:tasks")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var command = new DeleteTaskCommand(id);
        var deleted = await _commandHandler.HandleAsync(command);
        if (deleted)
        {
            return NoContent();
        }
        return NotFound();
    }
}
