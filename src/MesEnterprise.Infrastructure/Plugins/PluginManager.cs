using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Loader;
using MesEnterprise.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace MesEnterprise.Infrastructure.Plugins;

public sealed class PluginManager
{
    private readonly string _pluginFolder;
    private readonly IList<IPluginModule> _loadedModules = new List<IPluginModule>();

    public PluginManager(IConfiguration configuration)
    {
        _pluginFolder = configuration["Plugins:Path"] ?? Path.Combine(AppContext.BaseDirectory, "plugins");
        Directory.CreateDirectory(_pluginFolder);
    }

    public IReadOnlyCollection<IPluginModule> Modules => _loadedModules.ToList();

    public async Task LoadPluginsAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        var provider = new PhysicalFileProvider(_pluginFolder);
        foreach (var file in provider.GetDirectoryContents(string.Empty).Where(f => !f.IsDirectory && f.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var context = new AssemblyLoadContext(Path.GetFileNameWithoutExtension(file.Name), isCollectible: true);
            await using var stream = file.CreateReadStream();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            var assembly = context.LoadFromStream(ms);
            foreach (var moduleType in assembly.GetTypes().Where(t => typeof(IPluginModule).IsAssignableFrom(t) && !t.IsAbstract))
            {
                if (Activator.CreateInstance(moduleType) is IPluginModule module)
                {
                    module.ConfigureServices(services, configuration);
                    _loadedModules.Add(module);
                }
            }
        }
    }
}
