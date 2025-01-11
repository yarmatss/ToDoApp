using ToDoApp.Application.DTOs.Category;

namespace ToDoApp.Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryDto> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<CategoryDto>> GetAllAsync(Guid userId);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto, Guid userId);
    Task<CategoryDto> UpdateAsync(Guid id, CategoryDto dto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
}
