using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class DefectCode : AuditableEntity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}
