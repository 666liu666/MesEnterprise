using System.Security.Claims;

namespace MesEnterprise.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetTenantId(this ClaimsPrincipal principal)
    {
        var tenantValue = principal.FindFirstValue("tenant");
        return Guid.TryParse(tenantValue, out var tenantId) ? tenantId : null;
    }

    public static string? GetUserId(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.NameIdentifier);
}
