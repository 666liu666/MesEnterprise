using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class ShiftDefinition : AuditableEntity
{
    public required string ShiftName { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsOvernight { get; set; }
}
