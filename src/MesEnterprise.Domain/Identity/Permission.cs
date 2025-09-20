using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.Identity;

public class Permission : AuditableEntity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
    public ICollection<RolePermission> Roles { get; set; } = new List<RolePermission>();
}
