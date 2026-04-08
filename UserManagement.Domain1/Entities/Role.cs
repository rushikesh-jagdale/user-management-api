using UserManagement.Domain.Common;

namespace UserManagement.Domain.Entities
{
    public class Role : BaseEntity
    {
        public Guid TenantId { get; private set; }
        public string Name { get; private set; } = default!;

        // 🔗 UserRoles (Many-to-Many)
        private readonly List<UserRole> _userRoles = new();
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

        // 🔗 RolePermissions (Many-to-Many)
        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        private Role() { } // EF Core

        public Role(Guid tenantId, string name)
        {
            TenantId = tenantId;
            Name = name;
        }

        // ✅ Add Permission safely
        public void AddPermission(Guid permissionId)
        {
            if (_rolePermissions.Any(p => p.PermissionId == permissionId))
                return;

            _rolePermissions.Add(new RolePermission(Id, permissionId));
        }

        // ✅ Optional: Remove Permission
        public void RemovePermission(Guid permissionId)
        {
            var permission = _rolePermissions
                .FirstOrDefault(p => p.PermissionId == permissionId);

            if (permission is null)
                return;

            _rolePermissions.Remove(permission);
        }
    }
}