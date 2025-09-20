namespace MesEnterprise.Shared.Responses;

public record ApiResponse<T>(bool Success, T? Data, string? Message = null, string? TraceId = null)
{
    public static ApiResponse<T> Ok(T data, string? message = null, string? traceId = null) => new(true, data, message, traceId);
    public static ApiResponse<T> Fail(string message, string? traceId = null) => new(false, default, message, traceId);
}

public record PagedResponse<T>(IEnumerable<T> Items, long TotalCount, int PageNumber, int PageSize);
