using MediatR;
using UserManagement.Application.Common;

namespace UserManagement.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>;
