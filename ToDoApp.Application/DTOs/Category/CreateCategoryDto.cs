using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.DTOs.Category;

public class CreateCategoryDto
{
    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}
