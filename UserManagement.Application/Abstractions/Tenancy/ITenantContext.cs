namespace UserManagement.Application.Abstractions.Tenancy;

public interface ITenantContext
{
    Guid TenantId { get; }
    void SetTenantId(Guid tenantId);
}