using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Commands;
using TaskManagement.Api.Models;
using TaskManagement.Api.Models.Common;
using TaskManagement.Api.Queries;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController(ITaskCommandHandler commandHandler, ITaskQueryHandler queryHandler) : ControllerBase
{
    private readonly ITaskCommandHandler _commandHandler = commandHandler;
    private readonly ITaskQueryHandler _queryHandler = queryHandler;

    [HttpGet]
    public async Task<ActionResult<CursorPaginatedResult<TaskItem>>> GetAll(
        [FromQuery] Priority? priority = null,
        [FromQuery] Status? status = null,
        [FromQuery] Guid? cursor = null,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = new GetTasksQuery(priority, status, cursor, pageSize);
            var result = await _queryHandler.HandleAsync(query);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskItem>> GetOne(Guid id)
    {
        try
        {
            var query = new GetTaskByIdQuery(id);
            var task = await _queryHandler.HandleAsync(query);
            if (task is not null)
            {
                return Ok(task);
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(TaskRequest request)
    {
        try
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
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskItem>> Update(Guid id, TaskRequest request)
    {
        try
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
            else
            {
                return NotFound();
            }
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeleteTaskCommand(id);
            var deleted = await _commandHandler.HandleAsync(command);
            if (deleted)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurred");
        }
    }
}
