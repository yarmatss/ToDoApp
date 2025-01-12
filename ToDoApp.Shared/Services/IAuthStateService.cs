namespace ToDoApp.Shared.Services;

public interface IAuthStateService
{
    string AccessToken { get; }
    string RefreshToken { get; }
    void SetTokens(string accessToken, string refreshToken);
}
