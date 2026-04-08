using UserManagement.Domain.Common;

namespace UserManagement.Domain.Entities
{
    public class Role : BaseEntity
    {
        public Guid TenantId { get; private set; }
        public string Name { get; private set; }

        private Role() { }

        public Role(Guid tenantId, string name)
        {
            TenantId = tenantId;
            Name = name;
        }
    }
}

