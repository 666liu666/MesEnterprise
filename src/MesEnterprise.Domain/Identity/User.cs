using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.Identity;

public class User : AuditableEntity
{
    public required string UserName { get; set; }
    public required string PasswordHash { get; set; }
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public bool IsLocked { get; set; }
    public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
}
