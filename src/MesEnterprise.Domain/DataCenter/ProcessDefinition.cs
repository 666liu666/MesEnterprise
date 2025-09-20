using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class ProcessDefinition : AuditableEntity
{
    public required string ProcessCode { get; set; }
    public required string ProcessName { get; set; }
    public string? Description { get; set; }
    public int Sequence { get; set; }
}
