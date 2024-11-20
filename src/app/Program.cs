using System.Reflection;
using System.Runtime.Loader;
using core;

namespace app;

public class Program
{
    private static readonly string PluginDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
    private static readonly Dictionary<string, LoadedPluginInfo> LoadedPlugins = [];

    class LoadedPluginInfo
    {
        public Assembly Assembly { get; set; }
        public DateTime LastWriteTime { get; set; }
        public byte[] AssemblyBytes { get; set; }
    }

    static async Task Main(string[] args)
    {
        if (!Directory.Exists(PluginDirectory))
        {
            Directory.CreateDirectory(PluginDirectory);
        }
        
        while(true)
        {
            try
            {
                LoadPlugins();

                ExecutePlugins();

                await Task.Delay(1000);

                Console.WriteLine("Tick...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    
    private static void LoadPlugins()
    {
        var pluginFiles = Directory.GetFiles(PluginDirectory, "*.dll");

        var removedAssemblies = LoadedPlugins.Keys
            .Where(loadedPath => !pluginFiles.Contains(loadedPath))
            .ToList();

        foreach (var removedPath in removedAssemblies)
        {
            LoadedPlugins.Remove(removedPath);
            Console.WriteLine($"Plugin removed: {Path.GetFileName(removedPath)}");            
        }

        foreach (var pluginPath in pluginFiles)
        {
            try
            {
                var lastWriteTime = File.GetLastWriteTime(pluginPath);

                if (!LoadedPlugins.ContainsKey(pluginPath) ||
                    LoadedPlugins[pluginPath].LastWriteTime != lastWriteTime)
                {
                    byte[] assemblyBytes;
                    using (var fs = new FileStream(pluginPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        assemblyBytes = new byte[fs.Length];
                        fs.Read(assemblyBytes, 0, assemblyBytes.Length);
                    }

                    var assembly = Assembly.Load(assemblyBytes);

                    LoadedPlugins[pluginPath] = new LoadedPluginInfo
                    {
                        Assembly = assembly,
                        LastWriteTime = lastWriteTime,
                        AssemblyBytes = assemblyBytes
                    };

                    Console.WriteLine($"Plugin {(LoadedPlugins.ContainsKey(pluginPath) ? "reloaded" : "loaded")}: {Path.GetFileName(pluginPath)}");
                }    
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load plugin {Path.GetFileName(pluginPath)}: {ex.Message}");
            }
        }
    }

    private static void ExecutePlugins()
    {
        foreach (var pluginInfo in LoadedPlugins.Values)
        {
            try
            {
                var widgetTypes = pluginInfo.Assembly.GetTypes()
                    .Where(t => typeof(IWidget).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var widgetType in widgetTypes)
                {
                    if (Activator.CreateInstance(widgetType) is IWidget widget)
                    {
                        widget.Execute();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing plugin from {pluginInfo.Assembly.GetName().Name}: {ex.Message}");
            }
        }
    }
}