using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MesEnterprise.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var typesWithInterface = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new { Implementation = t, Interface = t.GetInterfaces().FirstOrDefault() })
            .Where(t => t.Interface is not null);

        foreach (var type in typesWithInterface)
        {
            services.Add(new ServiceDescriptor(type.Interface!, type.Implementation, lifetime));
        }

        return services;
    }
}
