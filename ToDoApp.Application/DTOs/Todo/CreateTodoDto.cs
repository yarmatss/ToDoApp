using System.ComponentModel.DataAnnotations;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.DTOs.Todo;

public class CreateTodoDto
{
    [MaxLength(200)]
    public required string Title { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public required Priority Priority { get; set; }
    public required Guid CategoryId { get; set; }
}