using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken);

    Task<RefreshToken?> GetAsync(
        Guid tenantId,
        string token,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
