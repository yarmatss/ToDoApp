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
    private readonly IAuthStateService _authState;

    public ApiClient(HttpClient httpClient, IAuthStateService authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    private async Task AddJwtTokenAsync()
    {
        if (!string.IsNullOrEmpty(_authState.AccessToken))
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(_authState.AccessToken);
            if (jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(-5) && !string.IsNullOrEmpty(_authState.RefreshToken))
            {
                var newToken = await RefreshTokenAsync(_authState.RefreshToken);
                _authState.SetTokens(newToken.AccessToken, newToken.RefreshToken);
            }
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.AccessToken);
        }
    }

    // Auth methods
    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", dto);
        response.EnsureSuccessStatusCode();
        var tokenDto = await response.Content.ReadFromJsonAsync<TokenDto>();
        if (tokenDto is not null)
            _authState.SetTokens(tokenDto.AccessToken, tokenDto.RefreshToken);
        return tokenDto;
    }

    public async Task<TokenDto> RegisterAsync(RegisterDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", dto);
        response.EnsureSuccessStatusCode();
        var tokenDto = await response.Content.ReadFromJsonAsync<TokenDto>();
        if (tokenDto is not null)
            _authState.SetTokens(tokenDto.AccessToken, tokenDto.RefreshToken);
        return tokenDto;
    }

    public async Task<TokenDto> RefreshTokenAsync(string refreshToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/refresh", new RefreshTokenDto { RefreshToken = refreshToken });
        response.EnsureSuccessStatusCode();
        var tokenDto = await response.Content.ReadFromJsonAsync<TokenDto>();
        if (tokenDto is not null)
            _authState.SetTokens(tokenDto.AccessToken, tokenDto.RefreshToken);
        return tokenDto;
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
