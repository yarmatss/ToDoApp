﻿using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.DTOs.Category;

public class CategoryDto
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}