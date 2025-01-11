namespace ToDoApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public ICollection<Todo> Todos { get; set; } = null!;
    public ICollection<Category> Categories { get; set; } = null!;
}