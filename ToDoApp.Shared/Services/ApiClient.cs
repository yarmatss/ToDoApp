using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ToDoApp.Application.Common.Models;
using ToDoApp.Application.DTOs.Auth;
using ToDoApp.Application.DTOs.Category;
using ToDoApp.Application.DTOs.Todo;

namespace ToDoApp.Shared.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task AddJwtTokenAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["JWTToken"];
        if (!string.IsNullOrEmpty(token))
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["RefreshToken"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var newToken = await RefreshTokenAsync(refreshToken);
                    _httpContextAccessor.HttpContext?.Response.Cookies.Append("JWTToken", newToken.AccessToken);
                    _httpContextAccessor.HttpContext?.Response.Cookies.Append("RefreshToken", newToken.RefreshToken);
                    token = newToken.AccessToken;
                }
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    // Auth methods
    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TokenDto>();
    }

    public async Task<TokenDto> RegisterAsync(RegisterDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TokenDto>();
    }

    public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/refresh", new { RefreshToken = refreshToken });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TokenDto>();
    }

    // Todo methods
    public async Task<PagedList<TodoDto>> GetTodosAsync(TodoParameters parameters)
    {
        await AddJwtTokenAsync();
        var response = await _httpClient.GetAsync($"api/Todo?{parameters}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedList<TodoDto>>();
    }

    public async Task<TodoDto> GetTodoByIdAsync(Guid id)
    {
        await AddJwtTokenAsync();
        var response = await _httpClient.GetAsync($"api/Todo/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoDto>();
    }

    public async Task<TodoDto> CreateTodoAsync(CreateTodoDto dto)
    {
        await AddJwtTokenAsync();
        var response = await _httpClient.PostAsJsonAsync("api/Todo", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoDto>();
    }

    public async Task<TodoDto> UpdateTodoAsync(Guid id, UpdateTodoDto dto)
    {
        await AddJwtTokenAsync();
        var response = await _httpClient.PutAsJsonAsync($"api/Todo/{id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoDto>();
    }

    public async Task DeleteTodoAsync(Guid id)
    {
        await AddJwtTokenAsync();
        var response = await _httpClient.DeleteAsync($"api/Todo/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<TodoDto> ToggleCompleteAsync(Guid id)
    {
        await AddJwtTokenAsync();
        var response = await _httpClient.PatchAsync($"api/Todo/{id}/toggle", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoDto>();
    }

    // Category methods
    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        await AddJwtTokenAsync();
        var response = await _httpClient.GetAsync("api/Category");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
    {
        await AddJwtTokenAsync();
        var response = await _httpClient.GetAsync($"api/Category/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryDto>();
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto dto)
    {
        await AddJwtTokenAsync();
        var response = await _httpClient.PostAsJsonAsync("api/Category", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryDto>();
    }

    public async Task<CategoryDto> UpdateCategoryAsync(Guid id, CategoryDto dto)
    {
        await AddJwtTokenAsync();
        var response = await _httpClient.PutAsJsonAsync($"api/Category/{id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryDto>();
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        await AddJwtTokenAsync();
        var response = await _httpClient.DeleteAsync($"api/Category/{id}");
        response.EnsureSuccessStatusCode();
    }
}
