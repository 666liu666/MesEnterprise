using System.Runtime.Loader;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MesEnterprise.Plugins
{
    public interface IPlugin { string Name { get; } void Configure(IServiceProvider provider); }
    public static class PluginLoader
    {
        public static void LoadPlugins(string folder, IServiceCollection services)
        {
            if (!Directory.Exists(folder)) return;
            foreach (var dll in Directory.GetFiles(folder, "*.dll"))
            {
                var alc = new AssemblyLoadContext(Path.GetFileNameWithoutExtension(dll), true);
                using var fs = File.OpenRead(dll);
                var asm = alc.LoadFromStream(fs);
                var types = asm.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                foreach (var t in types) services.AddSingleton(typeof(IPlugin), t);
            }
        }
    }
}
