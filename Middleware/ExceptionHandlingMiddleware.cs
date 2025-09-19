using MesEnterprise.Data;
using MesEnterprise.Domain;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text.Json;

namespace MesEnterprise.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) { _next = next; _logger = logger; }
        public async Task InvokeAsync(HttpContext ctx, AppDbContext db)
        {
            try { await _next(ctx); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                try
                {
                    db.AuditLogs.Add(new AuditLog { Path = ctx.Request.Path, Method = ctx.Request.Method, StatusCode = 500, Exception = ex.ToString(), Timestamp = DateTime.UtcNow });
                    await db.SaveChangesAsync();
                }
                catch { /* swallow */ }
                ctx.Response.StatusCode = 500;
                ctx.Response.ContentType = "application/json";
                await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { code = 500, message = "Internal Server Error" }));
            }
        }
    }
}