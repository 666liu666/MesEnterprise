using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class Employee : AuditableEntity
{
    public required string EmployeeNumber { get; set; }
    public required string FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public Guid? FactoryId { get; set; }
    public Factory? Factory { get; set; }
    public Guid? ShiftId { get; set; }
    public ShiftDefinition? Shift { get; set; }
}
