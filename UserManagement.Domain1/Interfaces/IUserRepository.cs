using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(
            Guid tenantId,
            string email,
            CancellationToken cancellationToken);

        Task<User?> GetByIdAsync(
            Guid tenantId,
            Guid userId,
            CancellationToken cancellationToken);

        Task<(IReadOnlyCollection<User> Users, int TotalCount)> GetPagedAsync(
               Guid tenantId,
               int pageNumber,
              int pageSize,
              CancellationToken cancellationToken);

        Task DeleteAsync(User user, Guid? deletedBy, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);

        Task<int> CountAdminsAsync(
             Guid tenantId,
             CancellationToken cancellationToken);
    }
}
