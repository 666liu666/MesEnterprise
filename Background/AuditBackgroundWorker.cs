using Microsoft.Extensions.Hosting;
namespace MesEnterprise
{
    public class AuditBackgroundWorker : BackgroundService
    {
        private readonly ILogger<AuditBackgroundWorker> _logger;
        public AuditBackgroundWorker(ILogger<AuditBackgroundWorker> logger) { _logger = logger; }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuditBackgroundWorker started");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                // placeholder: batch upload AuditLogs to external store, or compress, etc.
            }
        }
    }
}