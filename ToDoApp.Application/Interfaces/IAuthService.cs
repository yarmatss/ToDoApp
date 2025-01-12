using ToDoApp.Application.DTOs.Auth;

namespace ToDoApp.Application.Interfaces;

public interface IAuthService
{
    Task<TokenDto> LoginAsync(LoginDto dto);
    Task<TokenDto> RegisterAsync(RegisterDto dto);
    Task<TokenDto> RefreshTokenAsync(RefreshTokenDto request);
}