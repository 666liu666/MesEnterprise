using MesEnterprise.Domain.Common;
using MesEnterprise.Domain.DataCenter;

namespace MesEnterprise.Domain.Inventory;

public class InventoryItem : AuditableEntity
{
    public Guid PartId { get; set; }
    public PartMaster? Part { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public Guid LocationId { get; set; }
    public StorageLocation? Location { get; set; }
    public string? LotNumber { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
}

public class StorageLocation : AuditableEntity
{
    public required string LocationCode { get; set; }
    public string? Description { get; set; }
    public Guid? FactoryId { get; set; }
}
