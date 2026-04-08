namespace UserManagement.Application.Common;

public sealed class ApiErrorResponse
{
    public int StatusCode { get; init; }
    public List<string> Errors { get; init; } = new();
}

