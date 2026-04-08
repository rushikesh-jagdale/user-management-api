using UserManagement.Domain.Common;

namespace UserManagement.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; private set; }
        public bool IsActive { get; private set; }

        private Tenant() { } // For ORM (future use)

        public Tenant(string name)
        {
            Name = name;
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
            SetUpdatedAt();
        }
    }
}

