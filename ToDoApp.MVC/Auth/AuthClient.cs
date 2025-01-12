using System.Text;
using System.Text.Json;
using ToDoApp.Application.DTOs.Auth;
using ToDoApp.Shared.Services;

namespace ToDoApp.MVC.Auth;

public class AuthClient : IAuthClient
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AuthClient(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public Task<string> GetAccessTokenAsync()
    {
        var accessToken = _httpContextAccessor.HttpContext.Request.Cookies["AccessToken"];
        return Task.FromResult(accessToken);
    }

    public Task<string> GetRefreshTokenAsync()
    {
        var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["RefreshToken"];
        return Task.FromResult(refreshToken);
    }

    public Task SetTokensAsync(string accessToken, string refreshToken)
    {
        var accessTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        _httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", accessToken, accessTokenOptions);
        _httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, refreshTokenOptions);

        return Task.CompletedTask;
    }

    public Task ClearTokensAsync()
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Delete("AccessToken");
        _httpContextAccessor.HttpContext.Response.Cookies.Delete("RefreshToken");
        return Task.CompletedTask;
    }

    public async Task<bool> IsTokenExpiredAsync()
    {
        var accessToken = await GetAccessTokenAsync();
        if (string.IsNullOrEmpty(accessToken))
            return true;

        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(accessToken);
        return jwtToken.ValidTo <= DateTime.UtcNow;
    }

    public async Task<bool> RefreshTokensAsync()
    {
        var refreshToken = await GetRefreshTokenAsync();
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        // Create a dedicated HttpClient for authentication without JWT handler
        var client = _httpClientFactory.CreateClient("AuthClient");

        var refreshRequest = new RefreshTokenDto { RefreshToken = refreshToken };
        var jsonContent = new StringContent(JsonSerializer.Serialize(refreshRequest), Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync("/api/Auth/refresh", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var newTokens = await JsonSerializer.DeserializeAsync<TokenDto>(responseStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (newTokens != null)
                {
                    await SetTokensAsync(newTokens.AccessToken, newTokens.RefreshToken);
                    return true;
                }
            }
            await ClearTokensAsync();
            return false;
        }
        catch
        {
            await ClearTokensAsync();
            return false;
        }
    }
}
