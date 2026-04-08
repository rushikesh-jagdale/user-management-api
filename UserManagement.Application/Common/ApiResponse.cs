namespace UserManagement.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> FailureResponse(string message)
        => new() { Success = false, Message = message };
}
