using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class KpsnRule : AuditableEntity
{
    public required string RuleName { get; set; }
    public required string Expression { get; set; }
    public string? Description { get; set; }
}
