using MediatR;
using UserManagement.Application.Common;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Features.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQuery : IRequest<Result<UserDto>>
{
}