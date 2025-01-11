using ToDoApp.Application.Common.Models;
using ToDoApp.Application.DTOs.Todo;

namespace ToDoApp.Application.Interfaces;

public interface ITodoService
{
    Task<TodoDto> GetByIdAsync(Guid id, Guid userId);
    Task<PagedList<TodoDto>> GetAllAsync(Guid userId, TodoParameters parameters);
    Task<TodoDto> CreateAsync(CreateTodoDto dto, Guid userId);
    Task<TodoDto> UpdateAsync(Guid id, UpdateTodoDto dto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<TodoDto> ToggleCompleteAsync(Guid id, Guid userId);
}