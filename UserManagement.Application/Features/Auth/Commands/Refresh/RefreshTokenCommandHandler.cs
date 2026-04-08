using MediatR;
using UserManagement.Application.Abstractions.Security;
using UserManagement.Application.Abstractions.Tenancy;
using UserManagement.Application.Common;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ITenantContext _tenantContext;
    private readonly IPermissionRepository _permissionRepository; // ✅ NEW

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITokenService tokenService,
        ITenantContext tenantContext,
        IPermissionRepository permissionRepository) // ✅ NEW
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _tenantContext = tenantContext;
        _permissionRepository = permissionRepository; // ✅ NEW
    }

    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var refreshToken = await _refreshTokenRepository.GetAsync(
            tenantId,
            command.RefreshToken,
            cancellationToken);

        if (refreshToken is null || refreshToken.IsExpired())
            return Result<RefreshTokenResponse>.Failure("Invalid refresh token");

        var user = await _userRepository.GetByIdAsync(
            tenantId,
            refreshToken.UserId,
            cancellationToken);

        if (user is null || !user.CanLogin())
            return Result<RefreshTokenResponse>.Failure("User not allowed");

        // 🔐 Rotate old token
        refreshToken.Revoke();

        // =========================
        // ✅ FIX 1: Correct roles
        // =========================
        var roles = user.UserRoles
            .Select(r => r.Role?.Name)
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r!) // remove nullability
            .ToList();

        // =========================
        // ✅ FIX 2: Fetch permissions
        // =========================
        var permissions = await _permissionRepository
            .GetPermissionsByUserIdAsync(
                tenantId,
                user.Id,
                cancellationToken);

        // =========================
        // ✅ FIX 3: Pass permissions
        // =========================
        var accessToken = _tokenService.GenerateAccessToken(
            user.Id,
            user.Email,
            tenantId,
            roles,
            permissions);

        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken(
            user.Id,
            tenantId,
            newRefreshTokenValue,
            DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(
            newRefreshToken,
            cancellationToken);

        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return Result<RefreshTokenResponse>.Success(
            new RefreshTokenResponse(
                accessToken,
                newRefreshTokenValue,
                DateTime.UtcNow.AddMinutes(15)));
    }
}