using System;
using System.Threading.Tasks;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Shared.Constants;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MesEnterprise.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class NightlyDataSyncJob : IJob
{
    private readonly ILogger<NightlyDataSyncJob> _logger;
    private readonly ICacheService _cacheService;

    public NightlyDataSyncJob(ILogger<NightlyDataSyncJob> logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting nightly data synchronization at {Timestamp}", DateTimeOffset.UtcNow);
        await _cacheService.RemoveAsync("datacenter:processes");
        await _cacheService.RemoveAsync("inventory:snapshot");
        _logger.LogInformation("Nightly data synchronization completed at {Timestamp}", DateTimeOffset.UtcNow);
    }
}
