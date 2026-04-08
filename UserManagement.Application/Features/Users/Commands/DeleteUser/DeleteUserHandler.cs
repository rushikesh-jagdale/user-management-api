using MediatR;
using UserManagement.Application.Abstractions.Security;
using UserManagement.Application.Abstractions.Tenancy;
using UserManagement.Application.Common;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Features.Users.Commands.DeleteUser;

public sealed class DeleteUserHandler
    : IRequestHandler<DeleteUserCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserHandler(
        IUserRepository userRepository,
        ITenantContext tenantContext,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _tenantContext = tenantContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(
     DeleteUserCommand command,
     CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var currentUserId = _currentUserService.UserId;

        // ❌ Prevent self-delete
        if (currentUserId == command.UserId)
            return Result<bool>.Failure("You cannot delete your own account.");

        var user = await _userRepository.GetByIdAsync(
            tenantId,
            command.UserId,
            cancellationToken);

        if (user is null)
            return Result<bool>.Failure("User not found");

        // ❌ Safety check
        if (!user.UserRoles.Any())
            return Result<bool>.Failure("User roles not loaded.");

        // ❌ Prevent deleting admin
        if (user.IsAdmin())
        {
            var adminCount = await _userRepository
                .CountAdminsAsync(tenantId, cancellationToken);

            if (adminCount <= 1)
                return Result<bool>.Failure("Cannot delete the last admin.");

            return Result<bool>.Failure("Admin users cannot be deleted.");
        }

        // ✅ SOFT DELETE (correct way)
        await _userRepository.DeleteAsync(user, currentUserId, cancellationToken);

        return Result<bool>.Success(true);
    }
}