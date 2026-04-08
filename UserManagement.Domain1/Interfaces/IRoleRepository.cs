using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(
        string roleName,
        CancellationToken cancellationToken);

    Task<Guid> GetTenantIdAsync(
        CancellationToken cancellationToken);

    Task<Role?> GetByNameAsync(
        Guid tenantId, 
        string roleName, 
        CancellationToken cancellationToken);
}