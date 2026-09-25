using HouseXyloo.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseXyloo.Api.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(
        HouseXylooDbContext dbContext,
        IConfiguration configuration)
    {
        var email = configuration["Admin:Email"];
        var password = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var adminExists = await dbContext.AdminUsers
            .AnyAsync(x => x.Email == email);

        if (adminExists)
        {
            return;
        }

        var passwordHasher = new PasswordHasher<string>();

        var admin = new AdminUser
        {
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        admin.PasswordHash = passwordHasher.HashPassword(
            admin.Email,
            password
        );

        dbContext.AdminUsers.Add(admin);

        await dbContext.SaveChangesAsync();
    }
}