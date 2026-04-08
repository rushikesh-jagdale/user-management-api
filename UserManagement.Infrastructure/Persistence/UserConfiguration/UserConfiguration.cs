using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
               .IsRequired();

        builder.Property(x => x.Email)
               .IsRequired()
               .HasMaxLength(256);

        // 🔥 Composite unique (Tenant + Email)
        builder.HasIndex(x => new { x.TenantId, x.Email })
               .IsUnique();

        // 🔥 Performance index
        builder.HasIndex(x => x.TenantId);

        builder.Property(x => x.PasswordHash)
               .IsRequired();

        builder.Property(x => x.FirstName)
               .HasMaxLength(100);

        builder.Property(x => x.LastName)
               .HasMaxLength(100);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<int>();

        // ✅ Soft Delete defaults
        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        builder.Property(x => x.DeletedAt);

        builder.Property(x => x.DeletedBy);

        // 🔗 Relationships
        builder.HasMany(u => u.UserRoles)
               .WithOne()
               .HasForeignKey(ur => ur.UserId);

        // ⏱ Audit fields (optional but clean)
        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt);
    }
}