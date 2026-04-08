using MediatR;
using UserManagement.Application.Common;
using UserManagement.Application.Abstractions.Tenancy;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler
    : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITenantContext _tenantContext;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ITenantContext tenantContext)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tenantContext = tenantContext;
    }

    public async Task<Result<bool>> Handle(
        LogoutCommand command,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var refreshToken = await _refreshTokenRepository.GetAsync(
            tenantId,
            command.RefreshToken,
            cancellationToken);

        if (refreshToken is null)
            return Result<bool>.Failure("Invalid refresh token");

        refreshToken.Revoke();

        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
