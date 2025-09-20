using System.Security.Claims;
using MesEnterprise.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MesEnterprise.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId => Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    public Guid? TenantId => Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue("tenant"), out var id) ? id : null;
    public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    public bool IsInRole(string role) => _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;

    public bool HasPermission(string permission) => _httpContextAccessor.HttpContext?.User?.Claims.Any(c => c.Type == "permission" && c.Value == permission) ?? false;
}
