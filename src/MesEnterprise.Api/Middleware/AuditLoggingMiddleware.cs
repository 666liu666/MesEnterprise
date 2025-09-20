using System.IO;
using System;
using System.Text.Json;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Domain.Audit;

namespace MesEnterprise.Api.Middleware;

public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public AuditLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditService auditService)
    {
        context.Request.EnableBuffering();
        string? requestBody = null;
        if (context.Request.ContentLength is > 0)
        {
            context.Request.Body.Position = 0;
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }
        context.Items["audit-payload"] = requestBody;

        var start = DateTimeOffset.UtcNow;
        await _next(context);
        var elapsed = DateTimeOffset.UtcNow - start;

        var log = new AuditLog
        {
            Action = "API_CALL",
            RequestPath = context.Request.Path,
            HttpMethod = context.Request.Method,
            Payload = requestBody,
            Result = JsonSerializer.Serialize(new { status = context.Response.StatusCode, elapsed = elapsed.TotalMilliseconds }),
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context.Request.Headers.UserAgent,
            IsException = context.Response.StatusCode >= 500
        };

        await auditService.RecordAsync(log);
    }
}
