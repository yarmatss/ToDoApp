using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using ToDoApp.Application.DTOs.Auth;
using ToDoApp.Shared.Services;

namespace ToDoApp.Blazor.Services;

public class AuthClient : IAuthClient
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthClient(IJSRuntime jsRuntime, HttpClient httpClient, IHttpClientFactory httpClientFactory)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "AccessToken");
    }

    public async Task<string> GetRefreshTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "RefreshToken");
    }

    public async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "AccessToken", accessToken);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "RefreshToken", refreshToken);
    }

    public async Task ClearTokensAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "AccessToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "RefreshToken");
    }

    public async Task<bool> IsTokenExpiredAsync()
    {
        var accessToken = await GetAccessTokenAsync();
        if (string.IsNullOrEmpty(accessToken))
            return true;

        var tokenHandler = new JwtSecurityTokenHandler();
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
