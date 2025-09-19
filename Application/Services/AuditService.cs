using Microsoft.EntityFrameworkCore;
using MesEnterprise.Data;
using MesEnterprise.Domain;

namespace MesEnterprise.Application.Services;

public interface IAuditService
{
    Task WriteAsync(AuditLog log, CancellationToken cancellationToken = default);
}

public sealed class AuditService : IAuditService
{
    private readonly IDbContextFactory<MesDbContext> _dbContextFactory;

    public AuditService(IDbContextFactory<MesDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task WriteAsync(AuditLog log, CancellationToken cancellationToken = default)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await db.AuditLogs.AddAsync(log, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
