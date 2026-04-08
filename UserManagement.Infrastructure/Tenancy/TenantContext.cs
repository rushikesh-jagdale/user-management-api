using UserManagement.Application.Abstractions.Tenancy;

namespace UserManagement.Infrastructure.Tenancy;

public class TenantContext : ITenantContext
{
    private Guid _tenantId;

    public Guid TenantId => _tenantId;

    public void SetTenantId(Guid tenantId)
    {
        _tenantId = tenantId;
    }
}