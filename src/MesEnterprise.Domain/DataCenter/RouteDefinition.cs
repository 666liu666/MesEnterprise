using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class RouteDefinition : AuditableEntity
{
    public required string RouteCode { get; set; }
    public required string RouteName { get; set; }
    public IList<ProcessDefinition> Processes { get; set; } = new List<ProcessDefinition>();
}
