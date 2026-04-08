using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles"); // ✅ keep consistent naming

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TenantId)
            .IsRequired();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        // 🔥 Unique per tenant (IMPORTANT for multi-tenant)
        builder.HasIndex(r => new { r.TenantId, r.Name })
            .IsUnique();

        // 🔥 Performance
        builder.HasIndex(r => r.TenantId);

        // ✅ Soft delete
        builder.Property(r => r.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(r => r.DeletedAt);

        builder.Property(r => r.DeletedBy);

        // ⏱ Audit
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.UpdatedAt);
    }
}