using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.Identity;

public class Role : AuditableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<UserRole> Users { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
}
