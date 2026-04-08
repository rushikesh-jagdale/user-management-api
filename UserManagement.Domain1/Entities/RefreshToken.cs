using UserManagement.Domain.Common;

namespace UserManagement.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid TenantId { get; private set; }

    public string Token { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsRevoked { get; private set; }

    // EF Core
    private RefreshToken() { }

    public RefreshToken(
        Guid userId,
        Guid tenantId,
        string token,
        DateTime expiresAtUtc)
    {
        Id = Guid.NewGuid();              // ⭐ important
        UserId = userId;
        TenantId = tenantId;
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
        IsRevoked = false;

        CreatedAt = DateTime.UtcNow;     // ⭐ from BaseEntity
    }

    public bool IsExpired()
        => DateTime.UtcNow >= ExpiresAtUtc;

    public void Revoke()
    {
        if (IsRevoked)
            return;

        IsRevoked = true;
        SetUpdatedAt();
    }
}