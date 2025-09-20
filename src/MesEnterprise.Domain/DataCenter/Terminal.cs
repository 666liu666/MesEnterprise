using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class Terminal : AuditableEntity
{
    public required string TerminalCode { get; set; }
    public required string Location { get; set; }
    public string? Description { get; set; }
    public Guid? FactoryId { get; set; }
    public Factory? Factory { get; set; }
}
