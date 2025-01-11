using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Common.Exceptions;
using ToDoApp.Application.Common.Models;
using ToDoApp.Application.DTOs.Todo;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.Application.Services;

public class TodoService : ITodoService
{
    private readonly TodoDbContext _context;

    public TodoService(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<TodoDto> GetByIdAsync(Guid id, Guid userId)
    {
        var todo = await _context.Todos
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (todo == null)
            throw new NotFoundException("Todo not found");

        return MapToDto(todo);
    }

    public async Task<PagedList<TodoDto>> GetAllAsync(Guid userId, TodoParameters parameters)
    {
        var query = _context.Todos
            .Include(x => x.Category)
            .Where(x => x.UserId == userId);

        // Wyszukiwanie
        if (!string.IsNullOrEmpty(parameters.SearchPhrase))
        {
            var searchPhrase = parameters.SearchPhrase.ToLower();
            query = query.Where(x =>
                x.Title.ToLower().Contains(searchPhrase) ||
                x.Description.ToLower().Contains(searchPhrase));
        }

        // Filtrowanie
        if (parameters.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == parameters.CategoryId);

        if (parameters.DeadlineFrom.HasValue)
            query = query.Where(x => x.Deadline >= parameters.DeadlineFrom);

        if (parameters.DeadlineTo.HasValue)
            query = query.Where(x => x.Deadline <= parameters.DeadlineTo);

        if (parameters.IsCompleted.HasValue)
            query = query.Where(x => x.IsCompleted == parameters.IsCompleted);

        // Sortowanie
        query = parameters.SortBy.ToLower() switch
        {
            "deadline" when parameters.SortDescending => query.OrderByDescending(x => x.Deadline),
            "deadline" => query.OrderBy(x => x.Deadline),
            _ when parameters.SortDescending => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.CreatedAt)
        };

        // Paginacja
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        var dtos = items.Select(MapToDto);
        return new PagedList<TodoDto>(dtos, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<TodoDto> CreateAsync(CreateTodoDto dto, Guid userId)
    {
        if (dto.CategoryId != Guid.Empty)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId && c.UserId == userId);

            if (!categoryExists)
                throw new BadRequestException("Selected category does not exist");
        }

        var todo = MapToEntity(dto, userId);
        todo.Id = Guid.NewGuid();
        todo.UserId = userId;
        todo.CreatedAt = DateTime.UtcNow;

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(todo.Id, userId);
    }

    public async Task<TodoDto> UpdateAsync(Guid id, UpdateTodoDto dto, Guid userId)
    {
        var todo = await _context.Todos
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (todo == null)
            throw new NotFoundException("Todo not found");

        UpdateEntity(todo, dto);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(id, userId);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var todo = await _context.Todos
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (todo == null)
            throw new NotFoundException("Todo not found");

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
    }

    public async Task<TodoDto> ToggleCompleteAsync(Guid id, Guid userId)
    {
        var todo = await _context.Todos
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (todo == null)
            throw new NotFoundException("Todo not found");

        todo.IsCompleted = !todo.IsCompleted;
        todo.CompletedAt = todo.IsCompleted ? DateTime.UtcNow : null;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id, userId);
    }

    private TodoDto MapToDto(Todo todo)
    {
        if (todo == null)
            throw new ArgumentNullException(nameof(todo));

        return new TodoDto
        {
            Id = todo.Id,
            Title = todo.Title ?? string.Empty,
            Description = todo.Description ?? string.Empty,
            Deadline = todo.Deadline,
            Priority = todo.Priority,
            IsCompleted = todo.IsCompleted,
            CompletedAt = todo.CompletedAt,
            CreatedAt = todo.CreatedAt,
            CategoryId = todo.CategoryId,
            CategoryName = todo.Category?.Name ?? "Brak kategorii"
        };
    }

    private Todo MapToEntity(CreateTodoDto dto, Guid userId)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        return new Todo
        {
            Id = Guid.NewGuid(),
            Title = dto.Title ?? throw new BadRequestException("Title is required"),
            Description = dto.Description ?? string.Empty,
            Deadline = dto.Deadline,
            Priority = dto.Priority,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            CategoryId = dto.CategoryId,
            UserId = userId
        };
    }

    private void UpdateEntity(Todo entity, UpdateTodoDto dto)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        entity.Title = dto.Title ?? throw new BadRequestException("Title is required");
        entity.Description = dto.Description ?? string.Empty;
        entity.Deadline = dto.Deadline;
        entity.Priority = dto.Priority;
        entity.IsCompleted = dto.IsCompleted;
        entity.CompletedAt = dto.IsCompleted ? DateTime.UtcNow : null;
    }
}
