using MesEnterprise.Domain.Common;
using MesEnterprise.Domain.DataCenter;
using MesEnterprise.Domain.Manufacturing;

namespace MesEnterprise.Domain.Quality;

public class QualityRecord : AuditableEntity
{
    public Guid WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }
    public Guid? DefectCodeId { get; set; }
    public DefectCode? Defect { get; set; }
    public required string Station { get; set; }
    public string? SerialNumber { get; set; }
    public string? Remark { get; set; }
    public QualityRecordStatus Status { get; set; } = QualityRecordStatus.Open;
}

public enum QualityRecordStatus
{
    Open,
    Disposition,
    Closed
}
