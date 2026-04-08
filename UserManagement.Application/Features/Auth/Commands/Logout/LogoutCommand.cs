using MediatR;
using UserManagement.Application.Common;

namespace UserManagement.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(
    string RefreshToken
) : IRequest<Result<bool>>;

