using MediatR;
using UserManagement.Application.Abstractions.Tenancy;
using UserManagement.Application.Common;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Interfaces;
using UserManagement.Application.Abstractions.Security;

namespace UserManagement.Application.Features.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserHandler
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentUserHandler(
        IUserRepository userRepository,
        ITenantContext tenantContext,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
    }

    public async Task<Result<UserDto>> Handle(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var userId = _currentUser.UserId;

        if (userId == Guid.Empty)
            return Result<UserDto>.Failure("User not authenticated");

        var user = await _userRepository.GetByIdAsync(
            tenantId,
            userId,
            cancellationToken);

        if (user is null)
            return Result<UserDto>.Failure("User not found");

        return Result<UserDto>.Success(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Status = user.Status.ToString()
        });
    }
}