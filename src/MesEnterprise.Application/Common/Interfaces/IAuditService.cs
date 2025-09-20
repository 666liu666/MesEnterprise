using MesEnterprise.Domain.Audit;

namespace MesEnterprise.Application.Common.Interfaces;

public interface IAuditService
{
    Task RecordAsync(AuditLog log, CancellationToken cancellationToken = default);
}
