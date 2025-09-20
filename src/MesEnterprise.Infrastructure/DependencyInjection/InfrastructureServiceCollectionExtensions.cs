using System;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Application.DependencyInjection;
using MesEnterprise.Application.Features.DataCenter.Processes;
using MesEnterprise.Infrastructure.Auditing;
using MesEnterprise.Infrastructure.Caching;
using MesEnterprise.Infrastructure.Identity;
using MesEnterprise.Infrastructure.Jobs;
using MesEnterprise.Infrastructure.Monitoring;
using MesEnterprise.Infrastructure.Multitenancy;
using MesEnterprise.Infrastructure.Persistence;
using MesEnterprise.Infrastructure.Plugins;
using MesEnterprise.Infrastructure.Realtime;
using MesEnterprise.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace MesEnterprise.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        });

        services.AddDbContextFactory<TenantCatalogDbContext>(options =>
        {
            options.UseOracle(configuration.GetConnectionString("TenantCatalog") ?? configuration.GetConnectionString("Oracle"));
        });

        services.AddDbContext<MesDbContext>((sp, options) =>
        {
            var tenant = sp.GetRequiredService<ITenantContext>().CurrentTenant;
            var connectionString = tenant?.ConnectionString ?? configuration.GetConnectionString("Oracle");
            options.UseOracle(connectionString, oracle =>
            {
                oracle.UseOracleSQLCompatibility("11");
            });
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<ITenantContext, CurrentTenantContext>();
        services.AddScoped<ITenantResolver, CurrentTenantContext>();
        services.AddSingleton<ICacheService, HybridCacheService>();

        var pluginManager = new PluginManager(configuration);
        pluginManager.LoadPluginsAsync(services, configuration).GetAwaiter().GetResult();
        services.AddSingleton(pluginManager);

        services.AddApplicationLayer();
        services.AddObservability(configuration);
        services.AddJwtAuthentication(configuration);

        services.AddSignalR().AddStackExchangeRedis(configuration.GetConnectionString("Redis") ?? "localhost:6379");

        services.AddQuartz(q =>
        {
            var jobKey = JobKey.Create(nameof(NightlyDataSyncJob));
            q.AddJob<NightlyDataSyncJob>(opts => opts.WithIdentity(jobKey));
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("NightlyDataSyncJob-trigger")
                .WithCronSchedule(configuration["Scheduler:NightlySyncCron"] ?? "0 0 3 * * ?"));
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("oracle", tags: new[] { "ready" })
            .AddRedis(configuration.GetConnectionString("Redis") ?? "localhost:6379", name: "redis");

        return services;
    }
}

public class DatabaseHealthCheck : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
{
    private readonly MesDbContext _dbContext;

    public DatabaseHealthCheck(MesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1 FROM DUAL", cancellationToken);
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Database unavailable", ex);
        }
    }
}
