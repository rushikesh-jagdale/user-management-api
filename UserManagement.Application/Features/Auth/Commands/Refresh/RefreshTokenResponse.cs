namespace UserManagement.Application.Features.Auth.Commands.Refresh;

public sealed record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc);

