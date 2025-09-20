using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class ModelDefinition : AuditableEntity
{
    public required string ModelNumber { get; set; }
    public required string Description { get; set; }
    public Guid? VendorId { get; set; }
    public Vendor? Vendor { get; set; }
    public Guid? RouteId { get; set; }
    public RouteDefinition? Route { get; set; }
}
