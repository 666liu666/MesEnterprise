
// File: Auth/PermissionHandler.cs
using Microsoft.AspNetCore.Authorization;
namespace MesEnterprise.Auth
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var ok = context.User.Claims.Any(c => c.Type == "permissions" && c.Value == requirement.Permission);
            if (ok) context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
