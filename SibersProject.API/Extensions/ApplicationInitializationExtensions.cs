using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SibersProject.DAL.Data;
using SibersProject.DAL.Entities.Identity;

namespace SibersProject.API.Extensions;

// #инициализация_приложения / #application_initialization
// Startup helper: apply migrations, seed roles, and seed default super admin.
public static class ApplicationInitializationExtensions
{
    public static async Task ApplyMigrationsAndSeedIdentityAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await db.Database.MigrateAsync();

        foreach (var role in ApplicationRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var superAdminEmail = Environment.GetEnvironmentVariable("SUPERADMIN_EMAIL")
            ?? throw new InvalidOperationException("SUPERADMIN_EMAIL is required.");
        var superAdminPassword = Environment.GetEnvironmentVariable("SUPERADMIN_PASSWORD")
            ?? throw new InvalidOperationException("SUPERADMIN_PASSWORD is required.");

        var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
        if (superAdmin is not null)
            return;

        superAdmin = new ApplicationUser
        {
            UserName = superAdminEmail,
            Email = superAdminEmail,
            EmailConfirmed = true
        };
        var createResult = await userManager.CreateAsync(superAdmin, superAdminPassword);
        if (createResult.Succeeded)
            await userManager.AddToRoleAsync(superAdmin, ApplicationRoles.Supervisor);
    }
}
