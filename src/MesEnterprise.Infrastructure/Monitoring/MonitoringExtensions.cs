using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MesEnterprise.Infrastructure.Monitoring;

public static class MonitoringExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceName = configuration["Observability:ServiceName"] ?? "MesEnterprise.Api";

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddOtlpExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddPrometheusExporter();
            });

        return services;
    }
}
