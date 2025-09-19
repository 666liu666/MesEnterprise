namespace MesEnterprise.Infrastructure;

public record ApiResponse<T>(bool Success, string Message, T? Data, IDictionary<string, string[]>? Errors = null)
{
    public static ApiResponse<T> Ok(T data, string message) => new(true, message, data);

    public static ApiResponse<T> Ok(string message) => new(true, message, default);

    public static ApiResponse<T> Fail(string message, IDictionary<string, string[]>? errors = null)
        => new(false, message, default, errors);
}
