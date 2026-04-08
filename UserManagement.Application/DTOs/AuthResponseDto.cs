namespace UserManagement.Application.DTOs;

public sealed class AuthResponseDto
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
    public DateTime ExpiresAtUtc { get; init; }
}
