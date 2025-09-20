using System;
using System.Threading;
using System.Threading.Tasks;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MesEnterprise.Infrastructure.Multitenancy;

public sealed class CurrentTenantContext : ITenantContext, ITenantResolver
{
    private readonly IDbContextFactory<TenantCatalogDbContext> _catalogFactory;
    private readonly IMemoryCache _cache;
    private Tenant? _tenant;

    public CurrentTenantContext(IDbContextFactory<TenantCatalogDbContext> catalogFactory, IMemoryCache cache)
    {
        _catalogFactory = catalogFactory;
        _cache = cache;
    }

    public Tenant? CurrentTenant => _tenant;

    public async Task<Tenant?> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue<Tenant>(tenantId, out var cached))
        {
            _tenant = cached;
            return cached;
        }

        await using var context = await _catalogFactory.CreateDbContextAsync(cancellationToken);
        var tenant = await context.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
        if (tenant is not null)
        {
            _cache.Set(tenantId, tenant, TimeSpan.FromMinutes(5));
            _tenant = tenant;
        }

        return tenant;
    }

    public async Task<bool> TrySwitchTenantAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var tenant = await GetTenantAsync(tenantId, cancellationToken);
        if (tenant is null)
        {
            return false;
        }

        _tenant = tenant;
        return true;
    }
}
