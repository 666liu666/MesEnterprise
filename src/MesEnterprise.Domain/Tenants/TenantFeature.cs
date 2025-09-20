namespace MesEnterprise.Domain.Tenants;

public class TenantFeature
{
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public required string FeatureName { get; set; }
    public string? Value { get; set; }
}
