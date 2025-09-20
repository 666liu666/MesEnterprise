using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class Factory : AuditableEntity
{
    public required string FactoryCode { get; set; }
    public required string FactoryName { get; set; }
    public string? Address { get; set; }
    public string? TimeZone { get; set; }
}
