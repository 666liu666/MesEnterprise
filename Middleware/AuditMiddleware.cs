using System.Diagnostics;
using System.IO;
using System.Text;
using MesEnterprise.Application.Services;
using MesEnterprise.Domain;
using MesEnterprise.Shared;

namespace MesEnterprise.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;
    private readonly IAuditService _auditService;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger, IAuditService auditService, ITenantContextAccessor tenantContextAccessor)
    {
        _next = next;
        _logger = logger;
        _auditService = auditService;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        string? requestBody = null;

        if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        var originalBodyStream = context.Response.Body;
        await using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);

            var auditLog = new AuditLog
            {
                UserId = context.User?.Identity?.IsAuthenticated == true
                    ? context.User.FindFirst("sub")?.Value ?? context.User.FindFirst("nameid")?.Value
                    : null,
                Path = context.Request.Path,
                Method = context.Request.Method,
                StatusCode = context.Response.StatusCode,
                DurationMs = stopwatch.ElapsedMilliseconds,
                RequestBody = requestBody,
                ResponseBody = responseBody,
                TenantId = _tenantContextAccessor.Current?.TenantId,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                await _auditService.WriteAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to persist audit log for {Path}", context.Request.Path);
            }
        }
    }
}
