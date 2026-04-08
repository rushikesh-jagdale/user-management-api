using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure.Persistence.DbContext;

namespace UserManagement.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly UserManagementDbContext _context;

    public RoleRepository(UserManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByNameAsync(
        string roleName,
        CancellationToken cancellationToken)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(
                r => r.Name == roleName,
                cancellationToken);
    }

    public async Task<Guid> GetTenantIdAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Roles
            .Select(r => r.TenantId)
            .FirstAsync(cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(
       Guid tenantId,
       string roleName,
       CancellationToken cancellationToken)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r =>
                r.TenantId == tenantId &&
                r.Name == roleName,
                cancellationToken);
    }
}