using System.ComponentModel.DataAnnotations;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.DTOs.Todo;

public class CreateTodoDto
{
    [MaxLength(200)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }

    [Required]
    public Priority Priority { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}