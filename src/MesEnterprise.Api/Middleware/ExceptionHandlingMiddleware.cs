using System.Net;
using System.Text.Json;
using FluentValidation;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Domain.Audit;
using MesEnterprise.Shared.Responses;

namespace MesEnterprise.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IAuditService _auditService;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IAuditService auditService)
    {
        _next = next;
        _logger = logger;
        _auditService = auditService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, ex);
            await _auditService.RecordAsync(new AuditLog
            {
                Action = "Exception",
                HttpMethod = context.Request.Method,
                RequestPath = context.Request.Path,
                Payload = await ReadRequestBodyAsync(context),
                Result = ex.Message,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers.UserAgent,
                IsException = true
            });
        }
    }

    private static async Task<string?> ReadRequestBodyAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        context.Request.Body.Position = 0;
        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        return body;
    }

    private static async Task WriteErrorAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        var traceId = context.TraceIdentifier;
        var response = ApiResponse<object>.Fail(exception.Message, traceId);
        var payload = JsonSerializer.Serialize(response);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(payload);
    }
}
