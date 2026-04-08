using UserManagement.Domain.Common;

namespace UserManagement.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }

        private RefreshToken() { }

        public RefreshToken(Guid userId, string token, DateTime expiresAt)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            IsRevoked = false;
        }

        public void Revoke()
        {
            IsRevoked = true;
            SetUpdatedAt();
        }
    }
}

