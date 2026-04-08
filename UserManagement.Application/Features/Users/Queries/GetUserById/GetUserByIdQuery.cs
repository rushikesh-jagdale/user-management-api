using MediatR;
using UserManagement.Application.Common;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public Guid UserId { get; init; }
}

