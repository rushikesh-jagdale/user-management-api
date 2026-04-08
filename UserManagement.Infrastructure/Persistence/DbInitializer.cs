using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Persistence.DbContext;
using UserManagement.Application.Abstractions.Security;

namespace UserManagement.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<UserManagementDbContext>();

        // =========================
        // ✅ Apply migrations
        // =========================
        await context.Database.MigrateAsync();

        // =========================
        // 🔐 Tenant (STATIC for now)
        // =========================
        var tenantId = Guid.Parse("a6cb0542-3de3-4a3c-8dc4-32bd9a93059c");

        // =========================
        // ✅ Seed Permissions
        // =========================
        if (!await context.Permissions.AnyAsync())
        {
            var permissions = new List<Permission>
            {
                new Permission("create:user", tenantId),
                new Permission("view:user", tenantId),
                new Permission("update:user", tenantId),
                new Permission("delete:user", tenantId)
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync(); // 🔥 IMPORTANT
        }

        // =========================
        // ✅ Seed Roles
        // =========================
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new Role(tenantId, "Admin"),
                new Role(tenantId, "User")
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync(); // 🔥 IMPORTANT
        }

        // =========================
        // 🔄 Fetch Roles & Permissions
        // =========================
        var adminRole = await context.Roles
            .FirstAsync(r => r.Name == "Admin");

        var userRole = await context.Roles
            .FirstAsync(r => r.Name == "User");

        var permissionsList = await context.Permissions.ToListAsync();

        // =========================
        // ✅ Role → Permission Mapping
        // =========================
        if (!await context.RolePermissions.AnyAsync())
        {
            var rolePermissions = new List<RolePermission>();

            // 🔥 Admin → ALL permissions
            foreach (var permission in permissionsList)
            {
                rolePermissions.Add(new RolePermission(adminRole.Id, permission.Id));
            }

            // 👤 User → only view
            var viewPermission = permissionsList
                .First(p => p.Name == "view:user");

            rolePermissions.Add(new RolePermission(userRole.Id, viewPermission.Id));

            await context.RolePermissions.AddRangeAsync(rolePermissions);
            await context.SaveChangesAsync();
        }

        // =========================
        // ✅ Create Admin User
        // =========================
        if (!await context.Users.AnyAsync(u => u.Email == "admin@gmail.com"))
        {
            var passwordHasher = scope.ServiceProvider
                .GetRequiredService<IPasswordHasher>();

            var adminUser = new User(
                tenantId,
                "admin@gmail.com",
                passwordHasher.Hash("Admin@123"),
                "System",
                "Admin");

            adminUser.AssignRole(adminRole.Id);

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }
    }
}