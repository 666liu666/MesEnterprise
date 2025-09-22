using System.Security.Claims;


namespace MesEnterprise.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetTenantId(this ClaimsPrincipal principal)
    {
        //var tenantValue = principal.FindFirstValue("tenant");
        var tenantValue = principal.FindFirst("tenant")?.Value;

        return Guid.TryParse(tenantValue, out var tenantId) ? tenantId : null;
    }

    public static string? GetUserId(this ClaimsPrincipal principal) => principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
