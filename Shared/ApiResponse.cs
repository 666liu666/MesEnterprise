namespace MesEnterprise.Shared;

public record ApiResponse<T>(bool Success, T? Data, string? Message = null, string? ErrorCode = null)
{
    public static ApiResponse<T> Ok(T data, string? message = null) => new(true, data, message);
    public static ApiResponse<T> Fail(string message, string? errorCode = null) => new(false, default, message, errorCode);
}

public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data, string? message = null) => ApiResponse<T>.Ok(data, message);
    public static ApiResponse<T> Fail<T>(string message, string? errorCode = null) => ApiResponse<T>.Fail(message, errorCode);
}
