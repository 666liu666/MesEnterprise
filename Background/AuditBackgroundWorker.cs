using Hangfire;
using Microsoft.Extensions.Hosting;

namespace MesEnterprise.Background;

public class AuditBackgroundWorker : IHostedService
{
    private readonly ILogger<AuditBackgroundWorker> _logger;
    private readonly IRecurringJobManager _recurringJobManager;

    public AuditBackgroundWorker(ILogger<AuditBackgroundWorker> logger, IRecurringJobManager recurringJobManager)
    {
        _logger = logger;
        _recurringJobManager = recurringJobManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Configuring recurring background jobs");
        _recurringJobManager.AddOrUpdate("audit-log-cleanup", () => PerformAuditCleanup(), Cron.Daily);
        _recurringJobManager.AddOrUpdate("machine-heartbeat", () => PublishHeartbeat(), Cron.Minutely);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping background worker");
        return Task.CompletedTask;
    }

    public Task PerformAuditCleanup()
    {
        _logger.LogInformation("Executing audit log cleanup job at {Time}", DateTimeOffset.UtcNow);
        return Task.CompletedTask;
    }

    public Task PublishHeartbeat()
    {
        _logger.LogDebug("Publishing heartbeat at {Time}", DateTimeOffset.UtcNow);
        return Task.CompletedTask;
    }
}
