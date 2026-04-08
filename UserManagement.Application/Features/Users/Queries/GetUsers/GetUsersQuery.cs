using MediatR;
using UserManagement.Application.Common;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersQuery : IRequest<Result<PagedResult<UserDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}