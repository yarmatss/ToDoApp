using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.DTOs.Auth;

public class LoginDto
{
    [EmailAddress(ErrorMessage = "Incorrect email format")]
    [MaxLength(256)]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}