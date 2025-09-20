namespace MesEnterprise.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    string? UserName { get; }
    bool IsInRole(string role);
    bool HasPermission(string permission);
}
