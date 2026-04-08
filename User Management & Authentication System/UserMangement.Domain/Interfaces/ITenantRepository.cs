using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Interfaces
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetByIdAsync(Guid tenantId);
    }
}

