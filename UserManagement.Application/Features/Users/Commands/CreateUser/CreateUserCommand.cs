using MediatR;
using UserManagement.Application.Common;

namespace UserManagement.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommand : IRequest<Result<Guid>>
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
}



