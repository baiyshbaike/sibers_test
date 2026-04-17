using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SibersProject.DAL.Data;
using SibersProject.DAL.Entities;
using SibersProject.DAL.Entities.Identity;

namespace SibersProject.API.Extensions;

// #инициализация_приложения / #application_initialization
// Startup helper: apply migrations, seed roles, and seed default super admin.
public static class ApplicationInitializationExtensions
{
    private const string SuperAdminEmail = "superadmin@sibers.local";
    private const string SuperAdminPassword = "SuperAdmin123!";

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

        await EnsureUserWithRoleAsync(
            userManager,
            email: SuperAdminEmail,
            password: SuperAdminPassword,
            role: ApplicationRoles.Supervisor,
            employeeId: null);

        var defaultEmployees = new[]
        {
            new Employee { FirstName = "Maksim", LastName = "Orlov", Email = "manager1@sibers.local" },
            new Employee { FirstName = "Nina", LastName = "Belova", Email = "manager2@sibers.local" },
            new Employee { FirstName = "Ivan", LastName = "Petrov", Email = "employee1@sibers.local" },
            new Employee { FirstName = "Olga", LastName = "Sidorova", Email = "employee2@sibers.local" },
            new Employee { FirstName = "Denis", LastName = "Kuznetsov", Email = "employee3@sibers.local" }
        };

        foreach (var employee in defaultEmployees)
        {
            var existing = await db.Employees.FirstOrDefaultAsync(e => e.Email == employee.Email);
            if (existing is null)
            {
                db.Employees.Add(employee);
            }
        }

        await db.SaveChangesAsync();

        var manager1 = await db.Employees.FirstAsync(e => e.Email == "manager1@sibers.local");
        var manager2 = await db.Employees.FirstAsync(e => e.Email == "manager2@sibers.local");
        var employee1 = await db.Employees.FirstAsync(e => e.Email == "employee1@sibers.local");
        var employee2 = await db.Employees.FirstAsync(e => e.Email == "employee2@sibers.local");
        var employee3 = await db.Employees.FirstAsync(e => e.Email == "employee3@sibers.local");

        await EnsureUserWithRoleAsync(userManager, "manager1@sibers.local", "Manager123!", ApplicationRoles.ProjectManager, manager1.Id);
        await EnsureUserWithRoleAsync(userManager, "manager2@sibers.local", "Manager123!", ApplicationRoles.ProjectManager, manager2.Id);
        await EnsureUserWithRoleAsync(userManager, "employee1@sibers.local", "Employee123!", ApplicationRoles.Employee, employee1.Id);
        await EnsureUserWithRoleAsync(userManager, "employee2@sibers.local", "Employee123!", ApplicationRoles.Employee, employee2.Id);
        await EnsureUserWithRoleAsync(userManager, "employee3@sibers.local", "Employee123!", ApplicationRoles.Employee, employee3.Id);
    }

    private static async Task EnsureUserWithRoleAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string role,
        Guid? employeeId)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                EmployeeId = employeeId
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                return;
        }

        if (employeeId.HasValue && user.EmployeeId != employeeId)
        {
            user.EmployeeId = employeeId;
            await userManager.UpdateAsync(user);
        }

        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);
    }
}
