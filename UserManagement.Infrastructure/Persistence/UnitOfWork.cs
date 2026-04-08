using UserManagement.Application.Abstractions.Persistence;
using UserManagement.Infrastructure.Persistence.DbContext;

namespace UserManagement.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly UserManagementDbContext _context;

    public UnitOfWork(UserManagementDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
