using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Common.Exceptions;
using ToDoApp.Application.DTOs.Category;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly TodoDbContext _context;

    public CategoryService(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto> GetByIdAsync(Guid id, Guid userId)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (category == null)
            throw new NotFoundException("Category not found");

        return MapToDto(category);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(Guid userId)
    {
        var categories = await _context.Categories
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, Guid userId)
    {
        var category = MapToEntity(dto, userId);
        category.Id = Guid.NewGuid();
        category.UserId = userId;

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, CategoryDto dto, Guid userId)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (category == null)
            throw new NotFoundException("Category not found");

        UpdateEntity(category, dto);
        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (category == null)
            throw new NotFoundException("Category not found");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    private CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    private Category MapToEntity(CreateCategoryDto dto, Guid userId)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            UserId = userId
        };
    }

    private void UpdateEntity(Category entity, CategoryDto dto)
    {
        entity.Name = dto.Name;
        entity.Description = dto.Description;
    }
}