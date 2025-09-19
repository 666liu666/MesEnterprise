namespace MesEnterprise.Shared;

public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int TotalCount, int PageNumber, int PageSize);
