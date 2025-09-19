using Microsoft.AspNetCore.Identity;

namespace MesEnterprise.Domain;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }
    public Guid TenantId { get; set; }
}

public class ApplicationRole : IdentityRole<Guid>
{
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class Permission
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class RolePermission
{
    public Guid RoleId { get; set; }
    public ApplicationRole? Role { get; set; }
    public Guid PermissionId { get; set; }
    public Permission? Permission { get; set; }
}

public interface ITenantEntity
{
    Guid TenantId { get; set; }
}

public class Tenant
{
    public Guid Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string? UserId { get; set; }
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public long DurationMs { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public string? Exception { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class WorkOrder : ITenantEntity
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Status { get; set; } = "Created";
    public Guid TenantId { get; set; }
}

public class Machine : ITenantEntity
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Offline";
    public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;
    public Guid TenantId { get; set; }
}

public class WorkOrderHistory : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid WorkOrderId { get; set; }
    public string Operation { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Guid TenantId { get; set; }
}
