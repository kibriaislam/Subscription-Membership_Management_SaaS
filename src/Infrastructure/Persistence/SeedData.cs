using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Only seed if database is empty
        if (await context.Users.AnyAsync())
        {
            return;
        }

        // Create a sample user and business for development
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            FirstName = "Admin",
            LastName = "User",
            Role = "Owner",
            CreatedAt = DateTime.UtcNow
        };

        var business = new Business
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "Sample Business",
            Currency = "USD",
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.Businesses.Add(business);

        // Create sample subscription plan
        var plan = new SubscriptionPlan
        {
            Id = Guid.NewGuid(),
            BusinessId = business.Id,
            Name = "Monthly Plan",
            Description = "Monthly subscription plan",
            Price = 29.99m,
            DurationDays = 30,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.SubscriptionPlans.Add(plan);

        await context.SaveChangesAsync();
    }
}

