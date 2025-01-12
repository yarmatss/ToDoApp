using System.ComponentModel.DataAnnotations;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.DTOs.Todo;

public class UpdateTodoDto
{
    public Guid Id { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(1000)]
    public string Description { get; set; } = null!;
    public DateTime? Deadline { get; set; }

    [Required]
    public Priority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public Guid? CategoryId { get; set; }
}