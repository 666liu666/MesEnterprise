using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class Vendor : AuditableEntity
{
    public required string VendorCode { get; set; }
    public required string VendorName { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
}
