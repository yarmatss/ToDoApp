using System.ComponentModel.DataAnnotations;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.DTOs.Todo;

public class TodoDto
{
    public Guid Id { get; set; }

    [MaxLength(200)]
    public required string Title { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; } = null!;
    public DateTime? Deadline { get; set; }
    public required Priority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public required Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
