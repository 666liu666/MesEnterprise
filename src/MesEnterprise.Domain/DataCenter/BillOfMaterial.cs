using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.DataCenter;

public class BillOfMaterial : AuditableEntity
{
    public required string BomCode { get; set; }
    public Guid ModelId { get; set; }
    public ModelDefinition? Model { get; set; }
    public IList<BillOfMaterialItem> Items { get; set; } = new List<BillOfMaterialItem>();
}

public class BillOfMaterialItem
{
    public Guid BillOfMaterialId { get; set; }
    public BillOfMaterial? BillOfMaterial { get; set; }
    public Guid PartId { get; set; }
    public PartMaster? Part { get; set; }
    public decimal Quantity { get; set; }
}
