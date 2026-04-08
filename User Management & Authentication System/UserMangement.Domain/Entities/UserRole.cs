namespace UserManagement.Domain.Entities
{
    public class UserRole
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }

        private UserRole() { }

        public UserRole(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}

