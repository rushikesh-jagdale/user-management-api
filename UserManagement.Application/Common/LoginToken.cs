namespace UserManagement.Application.Common;

public sealed record LoginToken(
    string AccessToken,
    DateTime ExpiresAtUtc);
