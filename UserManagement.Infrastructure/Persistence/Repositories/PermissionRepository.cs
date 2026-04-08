using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure.Persistence.DbContext;

public sealed class PermissionRepository : IPermissionRepository
{
    private readonly UserManagementDbContext _context;

    public PermissionRepository(UserManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> GetPermissionsByUserIdAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}