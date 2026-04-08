namespace UserManagement.Application.Common;

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    public T Value { get; }

    private Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value)
        => new(true, value, string.Empty);

    public static Result<T> Failure(string error)
        => new(false, default!, error);
}
