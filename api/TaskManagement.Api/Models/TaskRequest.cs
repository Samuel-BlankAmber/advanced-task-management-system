using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models;

public class TaskRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description can't exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Priority is required")]
    public Priority Priority { get; set; }

    [Required(ErrorMessage = "DueDate is required")]
    public DateTime DueDate { get; set; }

    public Status Status { get; set; }
}
