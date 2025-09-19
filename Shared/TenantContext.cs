using System;
using System.Threading;

namespace MesEnterprise.Shared;

public sealed class TenantContext
{
    public Guid? TenantId { get; init; }
    public string? Identifier { get; init; }
}

public interface ITenantContextAccessor
{
    TenantContext? Current { get; set; }
}

public sealed class TenantContextAccessor : ITenantContextAccessor
{
    private static readonly AsyncLocal<TenantContext?> TenantCurrent = new();

    public TenantContext? Current
    {
        get => TenantCurrent.Value;
        set => TenantCurrent.Value = value;
    }
}
