namespace ToDoApp.Shared.Services;

public interface IAuthClient
{
    Task<string> GetAccessTokenAsync();
    Task<string> GetRefreshTokenAsync();
    Task SetTokensAsync(string accessToken, string refreshToken);
    Task ClearTokensAsync();
    Task<bool> IsTokenExpiredAsync();
    Task<bool> RefreshTokensAsync();
}
