using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.UserId)
            .IsRequired();

        builder.Property(rt => rt.TenantId)
            .IsRequired();

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(rt => rt.ExpiresAtUtc)
            .IsRequired();

        builder.Property(rt => rt.IsRevoked)
            .IsRequired();

        // 🔥 Unique token per tenant
        builder.HasIndex(rt => new { rt.TenantId, rt.Token })
            .IsUnique();

        // 🔥 Optional global token index (fast lookup)
        builder.HasIndex(rt => rt.Token);

        // ✅ Soft delete
        builder.Property(rt => rt.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(rt => rt.DeletedAt);

        builder.Property(rt => rt.DeletedBy);

        // 🔗 Relationship
        builder.HasOne<User>()
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);

        // ⏱ Audit
        builder.Property(rt => rt.CreatedAt).IsRequired();
        builder.Property(rt => rt.UpdatedAt);
    }
}