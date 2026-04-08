using UserManagement.Domain.Common;
using UserManagement.Domain.Enums;

namespace UserManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid TenantId { get; private set; }

        public string Email { get; private set; }
        public string PasswordHash { get; private set; }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public UserStatus Status { get; private set; }
        public int FailedLoginAttempts { get; private set; }

        private User() { }

        public User(
            Guid tenantId,
            string email,
            string passwordHash,
            string firstName,
            string lastName)
        {
            TenantId = tenantId;
            Email = email;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            Status = UserStatus.Active;
            FailedLoginAttempts = 0;
        }

        public void IncrementFailedLogin()
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= 5)
            {
                Status = UserStatus.Locked;
            }

            SetUpdatedAt();
        }

        public void ResetFailedLogin()
        {
            FailedLoginAttempts = 0;
            SetUpdatedAt();
        }

        public void Deactivate()
        {
            Status = UserStatus.Inactive;
            SetUpdatedAt();
        }
    }
}

