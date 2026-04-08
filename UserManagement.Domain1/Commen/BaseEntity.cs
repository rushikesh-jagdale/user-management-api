namespace UserManagement.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }

        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }

        // ✅ Soft Delete
        public bool IsDeleted { get; protected set; } = false;
        public DateTime? DeletedAt { get; protected set; }
        public Guid? DeletedBy { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false; // ✅ default
        }

        // ✅ Update timestamp
        public void SetUpdatedAt()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        // ✅ Soft delete
        public void SoftDelete(Guid? deletedBy) // 🔥 allow null (system delete safe)
        {
            if (IsDeleted) return; // ✅ prevent double delete

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;

            SetUpdatedAt();
        }

        // ✅ Restore (future use)
        public void Restore()
        {
            if (!IsDeleted) return; // ✅ safety

            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;

            SetUpdatedAt();
        }
    }
}