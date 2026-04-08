using MediatR;
using UserManagement.Application.Common;

namespace UserManagement.Application.Features.Users.Commands.DeleteUser;

public sealed class DeleteUserCommand : IRequest<Result<bool>>
{
    public Guid UserId { get; set; }
}