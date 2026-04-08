using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure.Persistence.DbContext;

namespace UserManagement.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly UserManagementDbContext _context;

    public UserRepository(UserManagementDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(
        Guid tenantId,
        string email,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(
                u => u.TenantId == tenantId
                  && u.Email == email
                  && !u.IsDeleted, // ✅ SOFT DELETE FILTER
                cancellationToken);
    }

    public async Task<User?> GetByIdAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(
                u => u.TenantId == tenantId
                  && u.Id == userId
                  && !u.IsDeleted, // ✅ SOFT DELETE FILTER
                cancellationToken);
    }

    public async Task<(IReadOnlyCollection<User>, int)> GetPagedAsync(
        Guid tenantId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Where(u => u.TenantId == tenantId && !u.IsDeleted); // ✅ FILTER

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Email)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }

    public async Task DeleteAsync(User user, Guid? deletedBy, CancellationToken cancellationToken)
    {
        user.SoftDelete(deletedBy); // 🔥 THIS LINE

        _context.Users.Update(user); // 🔥 THIS LINE

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task<int> CountAdminsAsync(
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .CountAsync(u =>
                u.TenantId == tenantId &&
                !u.IsDeleted && // ✅ IMPORTANT
                u.UserRoles.Any(ur => ur.Role.Name == "Admin"),
                cancellationToken);
    }
}