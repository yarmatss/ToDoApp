using ToDoApp.Application.Common.Models;
using ToDoApp.Application.DTOs.Auth;
using ToDoApp.Application.DTOs.Category;
using ToDoApp.Application.DTOs.Todo;

namespace ToDoApp.Shared.Services;

public interface IApiClient
{
    // Auth
    Task<TokenDto> LoginAsync(LoginDto dto);
    Task<TokenDto> RegisterAsync(RegisterDto dto);
    Task<TokenDto> RefreshTokenAsync(string refreshToken);

    // Todo
    Task<PagedList<TodoDto>> GetTodosAsync(TodoParameters parameters);
    Task<TodoDto> GetTodoByIdAsync(Guid id);
    Task<TodoDto> CreateTodoAsync(CreateTodoDto dto);
    Task<TodoDto> UpdateTodoAsync(Guid id, UpdateTodoDto dto);
    Task DeleteTodoAsync(Guid id);
    Task<TodoDto> ToggleCompleteAsync(Guid id);

    // Category
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
    Task<CategoryDto> GetCategoryByIdAsync(Guid id);
    Task<CategoryDto> CreateCategoryAsync(CategoryDto dto);
    Task<CategoryDto> UpdateCategoryAsync(Guid id, CategoryDto dto);
    Task DeleteCategoryAsync(Guid id);
}