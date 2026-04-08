using MediatR;
using UserManagement.Application.Abstractions.Tenancy;
using UserManagement.Application.Common;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersHandler
    : IRequestHandler<GetUsersQuery, Result<PagedResult<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantContext _tenantContext;

    public GetUsersHandler(
        IUserRepository userRepository,
        ITenantContext tenantContext)
    {
        _userRepository = userRepository;
        _tenantContext = tenantContext;
    }

    public async Task<Result<PagedResult<UserDto>>> Handle(
        GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var (users, totalCount) = await _userRepository.GetPagedAsync(
            tenantId,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        var userDtos = users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Status = u.Status.ToString()
            })
            .ToList();

        var result = new PagedResult<UserDto>(
            userDtos,
            totalCount,
            query.PageNumber,
            query.PageSize);

        return Result<PagedResult<UserDto>>.Success(result);
    }
}