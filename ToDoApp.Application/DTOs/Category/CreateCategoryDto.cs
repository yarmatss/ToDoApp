using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.DTOs.Category;

public class CreateCategoryDto
{
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}
