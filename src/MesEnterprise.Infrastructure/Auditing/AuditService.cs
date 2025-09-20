using System.Threading;
using System.Threading.Tasks;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Domain.Audit;
using MesEnterprise.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace MesEnterprise.Infrastructure.Auditing;

public class AuditService : IAuditService
{
    private readonly MesDbContext _dbContext;
    private readonly ILogger<AuditService> _logger;

    public AuditService(MesDbContext dbContext, ILogger<AuditService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task RecordAsync(AuditLog log, CancellationToken cancellationToken = default)
    {
        await _dbContext.AuditLogs.AddAsync(log, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Audit log recorded for {Action}", log.Action);
    }
}
