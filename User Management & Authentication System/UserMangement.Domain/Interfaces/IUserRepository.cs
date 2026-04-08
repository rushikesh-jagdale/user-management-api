using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(Guid tenantId, string email);
        Task<User?> GetByIdAsync(Guid tenantId, Guid userId);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
