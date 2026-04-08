using MediatR;
using UserManagement.Application.Abstractions.Security;
using UserManagement.Application.Abstractions.Tenancy;
using UserManagement.Application.Common;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ITenantContext _tenantContext;
    private readonly IPermissionRepository _permissionRepository;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ITenantContext tenantContext,
        IPermissionRepository permissionRepository)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _tenantContext = tenantContext;
        _permissionRepository = permissionRepository;
    }

    public async Task<Result<LoginResponse>> Handle(
     LoginCommand command,
     CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var user = await _userRepository.GetByEmailAsync(
            tenantId,
            command.Email,
            cancellationToken);

        if (user is null)
            return Result<LoginResponse>.Failure("Invalid credentials");

        if (!_passwordHasher.Verify(
            command.Password,
            user.PasswordHash))
            return Result<LoginResponse>.Failure("Invalid credentials");

        // ✅ Roles
        var roles = user.UserRoles
     .Select(r => r.Role?.Name)
     .Where(r => !string.IsNullOrWhiteSpace(r))
     .Select(r => r!) 
     .ToList();

        // ✅ Permissions from DB
        var permissions = await _permissionRepository
            .GetPermissionsByUserIdAsync(
                tenantId,
                user.Id,
                cancellationToken);

        Console.WriteLine("ROLES: " + string.Join(",", roles));
        Console.WriteLine("PERMISSIONS: " + string.Join(",", permissions));

        permissions = permissions ?? new List<string>();

        var accessToken = _tokenService.GenerateAccessToken(
                    user.Id,
                    user.Email,
                    tenantId,
                    roles,
                    permissions); // ✅ FIX

        var refreshTokenValue =
            _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken(
            user.Id,
            tenantId,
            refreshTokenValue,
            DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(
            refreshToken,
            cancellationToken);

        return Result<LoginResponse>.Success(
            new LoginResponse(
                accessToken,
                refreshTokenValue,
                DateTime.UtcNow.AddMinutes(15)));
    }
}