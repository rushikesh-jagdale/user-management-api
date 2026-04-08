using MediatR;
using UserManagement.Application.Abstractions.Tenancy;
using UserManagement.Application.Common;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdHandler
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantContext _tenantContext;

    public GetUserByIdHandler(
        IUserRepository userRepository,
        ITenantContext tenantContext)
    {
        _userRepository = userRepository;
        _tenantContext = tenantContext;
    }

    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var user = await _userRepository.GetByIdAsync(
                    tenantId,
                   query.UserId,
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
