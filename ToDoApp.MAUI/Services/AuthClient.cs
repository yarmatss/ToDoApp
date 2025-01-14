using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using ToDoApp.Application.DTOs.Auth;
using ToDoApp.Shared.Services;

namespace ToDoApp.MAUI.Services;

public class AuthClient : IAuthClient
{
    private const string AccessTokenKey = "AccessToken";
    private const string RefreshTokenKey = "RefreshToken";
    private readonly HttpClient _httpClient;

    public AuthClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        return await SecureStorage.GetAsync(AccessTokenKey);
    }

    public async Task<string> GetRefreshTokenAsync()
    {
        return await SecureStorage.GetAsync(RefreshTokenKey);
    }

    public async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        await SecureStorage.SetAsync(AccessTokenKey, accessToken);
        await SecureStorage.SetAsync(RefreshTokenKey, refreshToken);
    }

    public async Task ClearTokensAsync()
    {
        SecureStorage.Remove(AccessTokenKey);
        SecureStorage.Remove(RefreshTokenKey);
        await Task.CompletedTask;
    }

    public async Task<bool> IsTokenExpiredAsync()
    {
        var accessToken = await GetAccessTokenAsync();
        if (string.IsNullOrEmpty(accessToken))
            return true;

        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        return jwtToken.ValidTo <= DateTime.UtcNow;
    }

    public async Task<bool> RefreshTokensAsync()
    {
        var refreshToken = await GetRefreshTokenAsync();
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        var response = await _httpClient.PostAsJsonAsync("api/Auth/refresh", new { RefreshToken = refreshToken });
        if (response.IsSuccessStatusCode)
        {
            var tokenDto = await response.Content.ReadFromJsonAsync<TokenDto>();
            if (tokenDto != null)
            {
                await SetTokensAsync(tokenDto.AccessToken, tokenDto.RefreshToken);
                return true;
            }
        }
        return false;
    }
}
