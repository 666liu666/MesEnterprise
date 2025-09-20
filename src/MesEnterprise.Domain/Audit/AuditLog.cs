using MesEnterprise.Domain.Common;

namespace MesEnterprise.Domain.Audit;

public class AuditLog : AuditableEntity
{
    public required string Action { get; set; }
    public string? Resource { get; set; }
    public string? RequestPath { get; set; }
    public string? HttpMethod { get; set; }
    public string? Payload { get; set; }
    public string? Result { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsException { get; set; }
}
