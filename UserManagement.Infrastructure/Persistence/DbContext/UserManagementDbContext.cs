using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Common;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistence.DbContext;

public sealed class UserManagementDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // =========================
        // ✅ TABLE NAME FIX
        // =========================
        builder.Entity<Role>().ToTable("Role");

        // =========================
        // ✅ GLOBAL QUERY FILTER (🔥 IMPORTANT)
        // =========================
        builder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Role>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<RefreshToken>().HasQueryFilter(x => !x.IsDeleted);

        // ✅ ADD THESE (VERY IMPORTANT)
        builder.Entity<UserRole>().HasQueryFilter(ur => !ur.Role.IsDeleted);
        builder.Entity<RolePermission>().HasQueryFilter(rp => !rp.Role.IsDeleted);

        // =========================
        // USER ROLE CONFIG
        // =========================
        builder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(ur => ur.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(ur => ur.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // =========================
        // ROLE PERMISSION CONFIG
        // =========================
        builder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        builder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId);

        base.OnModelCreating(builder);
    }
}