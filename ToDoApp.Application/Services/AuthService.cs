using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDoApp.Application.Common.Exceptions;
using ToDoApp.Application.DTOs.Auth;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly TodoDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(TodoDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials");

        return await GenerateTokens(user);
    }

    public async Task<TokenDto> RegisterAsync(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
            throw new BadRequestException("Email already exists");

        if (dto.Password != dto.ConfirmPassword)
            throw new BadRequestException("Passwords do not match");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password)
        };

        // Dodanie domyślnej kategorii
        var defaultCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Tasks",
            Description = "Default category for tasks",
            UserId = user.Id
        };

        _context.Users.Add(user);
        _context.Categories.Add(defaultCategory);
        await _context.SaveChangesAsync();

        return await GenerateTokens(user);
    }

    private async Task<TokenDto> GenerateTokens(User user)
    {
        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new TokenDto
        {
            AccessToken = token,
            RefreshToken = refreshToken
        };
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public async Task<TokenDto> RefreshTokenAsync(RefreshTokenDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedException("Invalid refresh token");

        return await GenerateTokens(user);
    }
}
