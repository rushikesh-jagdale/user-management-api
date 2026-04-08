namespace UserManagement.Domain.Entities;

public class Permission
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public Guid TenantId { get; private set; }

    private Permission() { }

    public Permission(string name, Guid tenantId)
    {
        Id = Guid.NewGuid();
        Name = name;
        TenantId = tenantId;
    }
}