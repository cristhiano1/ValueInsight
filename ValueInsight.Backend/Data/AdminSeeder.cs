using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Models;
using ValueInsight.Backend.Services;

namespace ValueInsight.Backend.Data
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ValueInsightDbContext>();
            var passwordService = scope.ServiceProvider.GetRequiredService<PasswordService>();

            var adminEmail = configuration["SeedAdmin:Email"];
            var adminPassword = configuration["SeedAdmin:Password"];
            var adminName = configuration["SeedAdmin:Name"] ?? "System Admin";

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
                return;

            var existingAdmin = await db.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
            if (existingAdmin != null)
                return;

            var admin = new User
            {
                Name = adminName,
                Email = adminEmail,
                IsAdmin = true
            };

            admin.Password = passwordService.HashPassword(admin, adminPassword);

            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }
    }
}