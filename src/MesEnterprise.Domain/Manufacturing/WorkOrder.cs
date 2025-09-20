using MesEnterprise.Domain.Common;
using MesEnterprise.Domain.DataCenter;

namespace MesEnterprise.Domain.Manufacturing;

public class WorkOrder : AuditableEntity
{
    public required string WorkOrderNumber { get; set; }
    public Guid ModelId { get; set; }
    public ModelDefinition? Model { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset PlannedStart { get; set; }
    public DateTimeOffset PlannedEnd { get; set; }
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Draft;
    public ICollection<WorkOrderStep> Steps { get; set; } = new List<WorkOrderStep>();
}

public enum WorkOrderStatus
{
    Draft,
    Released,
    InProgress,
    Completed,
    Closed,
    OnHold
}

public class WorkOrderStep : AuditableEntity
{
    public Guid WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }
    public Guid ProcessId { get; set; }
    public ProcessDefinition? Process { get; set; }
    public int Sequence { get; set; }
    public WorkOrderStepStatus Status { get; set; } = WorkOrderStepStatus.Pending;
}

public enum WorkOrderStepStatus
{
    Pending,
    Running,
    Completed,
    Failed
}
