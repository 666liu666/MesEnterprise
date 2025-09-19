using Microsoft.EntityFrameworkCore;
using MesEnterprise.Data;
using MesEnterprise.Domain;
using MesEnterprise.Shared;

namespace MesEnterprise.Application.Services;

public interface ITenantResolver
{
    Task<Tenant?> ResolveAsync(string identifier, CancellationToken cancellationToken = default);
}

public sealed class TenantResolver : ITenantResolver
{
    private readonly IDbContextFactory<MesDbContext> _dbContextFactory;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public TenantResolver(IDbContextFactory<MesDbContext> dbContextFactory, ITenantContextAccessor tenantContextAccessor)
    {
        _dbContextFactory = dbContextFactory;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task<Tenant?> ResolveAsync(string identifier, CancellationToken cancellationToken = default)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Identifier == identifier && t.IsActive, cancellationToken);
        if (tenant != null)
        {
            _tenantContextAccessor.Current = new TenantContext
            {
                TenantId = tenant.Id,
                Identifier = tenant.Identifier
            };
        }

        return tenant;
    }
}
