using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.DTOs.Auth;

public class TokenDto
{
    public required string AccessToken { get; set; }

    [MaxLength(256)]
    public required string RefreshToken { get; set; }
}
