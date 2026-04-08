using UserManagement.Domain.Common;
using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Entities;

public class User : BaseEntity
{
    public Guid TenantId { get; private set; }

    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;

    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;

    public UserStatus Status { get; private set; }
    public int FailedLoginAttempts { get; private set; }

    // 🔐 Roles (Many-to-Many)
    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    // 🔐 Refresh Tokens
    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() { } // EF Core

    public User(
        Guid tenantId,
        string email,
        string passwordHash,
        string firstName,
        string lastName)
    {
        TenantId = tenantId;
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;

        Status = UserStatus.Active;
        FailedLoginAttempts = 0;
    }

    public void Update(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
    public bool CanLogin()
        => Status == UserStatus.Active;

    public void AssignRole(Guid roleId)
    {
        if (_userRoles.Any(r => r.RoleId == roleId))
            return;

        _userRoles.Add(new UserRole(Id, roleId));
        SetUpdatedAt();
    }

    public void RemoveRole(Guid roleId)
    {
        var role = _userRoles.FirstOrDefault(r => r.RoleId == roleId);
        if (role is null)
            return;

        _userRoles.Remove(role);
        SetUpdatedAt();
    }

    public void IncrementFailedLogin()
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= 5)
            Status = UserStatus.Locked;

        SetUpdatedAt();
    }

    public void ResetFailedLogin()
    {
        FailedLoginAttempts = 0;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        SetUpdatedAt();
    }
    public bool IsAdmin()
    {
        return UserRoles.Any(ur => ur.Role.Name == "Admin");
    }
    public void MarkAsDeleted(Guid deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}