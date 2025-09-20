using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MesEnterprise.Plugins;

public interface IPluginModule
{
    string Name { get; }
    string Version { get; }
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}
