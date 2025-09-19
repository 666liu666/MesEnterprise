using MesEnterprise.Application.Services;
using MesEnterprise.Shared;

namespace MesEnterprise.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITenantResolver _tenantResolver;
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(
        RequestDelegate next,
        ITenantResolver tenantResolver,
        ITenantContextAccessor tenantContextAccessor,
        ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _tenantResolver = tenantResolver;
        _tenantContextAccessor = tenantContextAccessor;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _tenantContextAccessor.Current = null;

        var tenantIdentifier = context.Request.Headers["X-Tenant"].FirstOrDefault()
            ?? context.Request.Query["tenant"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(tenantIdentifier))
        {
            var tenant = await _tenantResolver.ResolveAsync(tenantIdentifier!);
            if (tenant == null)
            {
                _logger.LogWarning("Tenant {Identifier} not found", tenantIdentifier);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(ApiResponse.Fail<string>("Invalid tenant"));
                return;
            }
        }

        await _next(context);
    }
}
