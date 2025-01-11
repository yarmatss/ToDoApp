using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.DTOs.Auth;

public class RegisterDto
{
    [EmailAddress]
    [MaxLength(256)]
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}
