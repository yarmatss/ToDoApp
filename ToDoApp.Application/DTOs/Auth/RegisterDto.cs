using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.DTOs.Auth;

public class RegisterDto
{
    [EmailAddress(ErrorMessage = "Incorrect email format")]
    [MaxLength(256)]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
