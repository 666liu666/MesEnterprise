namespace MesEnterprise.Domain
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? DisplayName { get; set; }
    }

    public class ApplicationRole : IdentityRole<Guid>
    {
        public ICollection<RolePermission>? RolePermissions { get; set; }
    }

    public class Permission
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public ICollection<RolePermission>? RolePermissions { get; set; }
    }

    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public ApplicationRole? Role { get; set; }
        public Guid PermissionId { get; set; }
        public Permission? Permission { get; set; }
    }

    public class Tenant
    {
        public Guid Id { get; set; }
        public string Identifier { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class AuditLog
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string Path { get; set; } = null!;
        public string Method { get; set; } = null!;
        public int StatusCode { get; set; }
        public long DurationMs { get; set; }
        public string? Exception { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class WorkOrder
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Product { get; set; } = null!;
        public int Quantity { get; set; }
        public string Status { get; set; } = "Created";
    }

    public class Machine
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Status { get; set; } = "Offline";
        public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;
    }
}
