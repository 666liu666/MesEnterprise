using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class PartMaster : AuditableEntity
{
    public required string PartNumber { get; set; }
    public required string PartName { get; set; }
    public string? Specification { get; set; }
    public string? UnitOfMeasure { get; set; }
    public Guid? VendorId { get; set; }
    public Vendor? Vendor { get; set; }
}
