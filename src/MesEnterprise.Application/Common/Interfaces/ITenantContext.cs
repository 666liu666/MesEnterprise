using MesEnterprise.Domain.Tenants;

namespace MesEnterprise.Application.Common.Interfaces;

public interface ITenantContext
{
    Tenant? CurrentTenant { get; }
    Task<Tenant?> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken);
}
