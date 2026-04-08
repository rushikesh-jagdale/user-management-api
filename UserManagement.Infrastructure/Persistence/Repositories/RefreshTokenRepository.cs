using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Infrastructure.Persistence.DbContext;

namespace UserManagement.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly UserManagementDbContext _context;

    public RefreshTokenRepository(UserManagementDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken)
    {
        await _context.RefreshTokens.AddAsync(
            refreshToken,
            cancellationToken);
    }

    public async Task<RefreshToken?> GetAsync(
        Guid tenantId,
        string token,
        CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Where(rt =>
                rt.TenantId == tenantId &&
                rt.Token == token &&
                !rt.IsRevoked)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
