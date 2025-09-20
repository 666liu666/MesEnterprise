using MesEnterprise.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace MesEnterprise.Infrastructure.Multitenancy;

public class TenantCatalogDbContext : DbContext
{
    public TenantCatalogDbContext(DbContextOptions<TenantCatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
}
