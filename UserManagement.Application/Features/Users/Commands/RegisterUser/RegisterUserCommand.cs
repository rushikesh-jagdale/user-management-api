using MediatR;
using UserManagement.Application.Common;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<Result<Guid>>; // ✅ MUST be Result<Guid>