using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.Tenants;

public class Tenant : AuditableEntity
{
    public required string Name { get; set; }
    public string? Code { get; set; }
    public string? ConnectionString { get; set; }
    public ICollection<TenantFeature> Features { get; set; } = new List<TenantFeature>();
}
