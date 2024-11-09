using Terraria;
using System.Diagnostics;
using System.Reflection;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoPluginManager;
internal static class Utils
{
    public const string GiteePluginArchiveUrl = "https://gitee.com/kksjsj/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";
    public const string GiteePluginManifestUrl = "https://gitee.com/kksjsj/TShockPlugin/raw/master/Plugins.json";

    public const string GithubPluginArchiveUrl = "https://github.com/UnrealMultiple/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";
    public const string GithubPluginManifestUrl = "https://raw.githubusercontent.com/UnrealMultiple/TShockPlugin/master/Plugins.json";

    public static readonly Dictionary<string, Version> PluginVersionOverrides = new(); // Plugin Assembly Name to Version
    
    public static void UnLoadPlugins(IEnumerable<string> targets)
    {
        var plugins = (List<PluginContainer>) typeof(ServerApi)
            .GetField("plugins", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?
            .GetValue(null)!;

        var targetsArray = targets.ToArray();
        var disposingPluginContainers = plugins
            .Where(c => targetsArray.Contains(c.Plugin.GetType().Assembly.GetName().Name))
            .ToArray();
        foreach (var c in disposingPluginContainers)
        {
            c.Plugin.Dispose();
            plugins.Remove(c);
            TShock.Log.ConsoleInfo($"Plugin {c.Plugin.Name} v{c.Plugin.Version} (by {c.Plugin.Author}) disposed.");
        }
    }

    public static void LoadPlugins(IEnumerable<string> targets) // TODO: change to use plugin assembly name instead of path
    {
        var flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        var loadedAssemblies = (Dictionary<string, Assembly>) typeof(ServerApi).GetField("loadedAssemblies", flag)?.GetValue(null)!;
        var game = (Main) typeof(ServerApi).GetField("game", flag)?.GetValue(null)!;
        var plugins = (List<PluginContainer>) typeof(ServerApi).GetField("plugins", flag)?.GetValue(null)!;
        foreach (var current in targets)
        {
            var tsPluginPath = Path.Combine(AppContext.BaseDirectory, ServerApi.PluginsPath, current);
            if (loadedAssemblies!.TryGetValue(current, out var assemblies))
            {
                if (assemblies.GetName().Equals(AssemblyName.GetAssemblyName(tsPluginPath)))
                {
                    continue;
                }
            }
            if (File.Exists(tsPluginPath))
            {
                var pdb = Path.ChangeExtension(tsPluginPath, ".pdb");
                var symbols = File.Exists(pdb) ? File.ReadAllBytes(pdb) : null;
                var ass = Assembly.Load(File.ReadAllBytes(tsPluginPath), symbols);
                loadedAssemblies[Path.GetFileNameWithoutExtension(current)] = ass;
                foreach (var type in ass.GetExportedTypes())
                {
                    if (!type.IsSubclassOf(typeof(TerrariaPlugin)) || !type.IsPublic || type.IsAbstract)
                    {
                        continue;
                    }
                    var customAttributes = type.GetCustomAttributes(typeof(ApiVersionAttribute), false);
                    if (customAttributes.Length == 0)
                    {
                        continue;
                    }

                    if (!ServerApi.IgnoreVersion)
                    {
                        var apiVersionAttribute = (ApiVersionAttribute) customAttributes[0];
                        var apiVersion = apiVersionAttribute.ApiVersion;
                        if (apiVersion.Major != ServerApi.ApiVersion.Major || apiVersion.Minor != ServerApi.ApiVersion.Minor)
                        {
                            TShock.Log.ConsoleError(
                                string.Format("Plugin \"{0}\" is designed for a different Server API version ({1}) and was ignored.",
                                type.FullName, apiVersion.ToString(2)), TraceLevel.Warning);

                            return;
                        }
                    }

                    try
                    {

                        if (Activator.CreateInstance(type, game) is TerrariaPlugin pluginInstance)
                        {
                            var pc = new PluginContainer(pluginInstance);
                            plugins.Add(pc);
                            pc.Initialize();
                            TShock.Log.ConsoleInfo($"Plugin {pc.Plugin.Name} v{pc.Plugin.Version} (by {pc.Plugin.Author}) initiated.",
                           TraceLevel.Info);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Broken plugins better stop the entire server init.
                        throw new InvalidOperationException(
                            string.Format("Could not create an instance of plugin class \"{0}\".", type.FullName), ex);
                    }
                }
            }

        }
    }

    public static PluginVersionInfo[] GetInstalledPlugins()
    {
        var pluginAssemblyToFileNameMap = ((Dictionary<string, Assembly>?)typeof(ServerApi)
            .GetField("loadedAssemblies", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?
            .GetValue(null))!
            .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        var installedPlugins = ServerApi.Plugins
            .Select(c => new PluginVersionInfo // c: container
            {
                AssemblyName = c.Plugin.GetType().Assembly.GetName().Name!,
                Author = c.Plugin.Author,
                Name = c.Plugin.Name,
                Description = c.Plugin.Description,
                Path = pluginAssemblyToFileNameMap
                    .TryGetValue(c.Plugin.GetType().Assembly, out var fileName) ? fileName + ".dll" : "",
                Version = PluginVersionOverrides.GetValueOrDefault(c.Plugin.GetType().Assembly.GetName().Name!, c.Plugin.Version)
            })
            // .DistinctBy(i => i.AssemblyName)
            .ToArray();

        return installedPlugins;
    }

    public static void SendFormattedServerPluginsModifications(this TSPlayer player, (PluginUpdateInfo[] plugins, string[] externalDlls) success)
    {
        if (success.plugins.Any(p => p.Current is null))
        {
            player.SendSuccessMessage(
                GetString("[安装完成]\n") +
                string.Join('\n', success.plugins
                    .Where(p => p.Current is null)
                    .Select(p => $"[{p.Latest.Name}] V{p.Latest.Version}")));
        }
        if (success.plugins.Any(p => p.Current is not null))
        {
            player.SendSuccessMessage(
                GetString("[更新完成]\n") +
                string.Join('\n', success.plugins
                    .Where(p => p.Current is not null)
                    .Select(p => $"[{p.Current?.Name ?? p.Latest.Name}] V{p.Current?.Version} >>> V{p.Latest.Version}")));
        }
        if (success.externalDlls.Any())
        {
            player.SendSuccessMessage(
                GetString("[外部库安装完成]\n") +
                string.Join('\n', success.externalDlls
                    .Select(d => $"[{d}]")));
        }
    }
}
