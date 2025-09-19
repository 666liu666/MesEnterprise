using MesEnterprise.Data;
using MesEnterprise.Domain;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Net.Http;

namespace MesEnterprise.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;
        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger) { _next = next; _logger = logger; }
        public async Task InvokeAsync(HttpContext ctx, AppDbContext db)
        {
            var sw = Stopwatch.StartNew();
            await _next(ctx);
            sw.Stop();
            try
            {
                db.AuditLogs.Add(new AuditLog { UserId = ctx.User?.FindFirst("sub")?.Value, Path = ctx.Request.Path, Method = ctx.Request.Method, StatusCode = ctx.Response.StatusCode, DurationMs = sw.ElapsedMilliseconds, Timestamp = DateTime.UtcNow });
                await db.SaveChangesAsync();
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Audit write failed"); }
        }
    }
}