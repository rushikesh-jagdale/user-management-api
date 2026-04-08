using MediatR;
using UserManagement.Application.Abstractions.Tenancy;
using UserManagement.Application.Common;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserHandler
    : IRequestHandler<UpdateUserCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantContext _tenantContext;

    public UpdateUserHandler(
        IUserRepository userRepository,
        ITenantContext tenantContext)
    {
        _userRepository = userRepository;
        _tenantContext = tenantContext;
    }

    public async Task<Result<bool>> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var user = await _userRepository.GetByIdAsync(
            tenantId,
            command.Id,
            cancellationToken);

        if (user is null)
            return Result<bool>.Failure("User not found");

        // ✅ Update via domain method
        user.Update(command.FirstName, command.LastName);

        await _userRepository.UpdateAsync(user, cancellationToken);

        return Result<bool>.Success(true);
    }
}