using MediatR;
using UserManagement.Application.Common;

namespace UserManagement.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshTokenCommand
    : IRequest<Result<RefreshTokenResponse>>
{
    public string RefreshToken { get; init; } = default!;
}
