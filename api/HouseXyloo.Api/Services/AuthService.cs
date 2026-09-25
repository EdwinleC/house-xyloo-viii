using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HouseXyloo.Api.Data;
using HouseXyloo.Api.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HouseXyloo.Api.Services;

public class AuthService
{
    private readonly HouseXylooDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<string> _passwordHasher = new();

    public AuthService(
        HouseXylooDbContext dbContext,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var admin = await _dbContext.AdminUsers
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (admin is null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(
            admin.Email,
            admin.PasswordHash,
            request.Password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var expiresAt = DateTime.UtcNow.AddHours(8);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            )
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(token),

            ExpiresAt = expiresAt
        };
    }
}