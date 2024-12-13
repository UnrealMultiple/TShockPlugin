﻿using Terraria;
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

    private static Dictionary<string, PluginVersionInfo>? _installedPluginsManifestCache;
    public static Dictionary<string, PluginVersionInfo> InstalledPluginsManifestCache => _installedPluginsManifestCache ??= GetInstalledPlugins().ToDictionary(i => i.AssemblyName);
    public static readonly Dictionary<string, Version> PluginVersionOverrides = new ();
    
    public static List<string> UnLoadPlugins(IEnumerable<string> targetPaths)
    {
        var failedUnload = new List<string>();
        _installedPluginsManifestCache = null;
        var loadedAssemblies = (Dictionary<string, Assembly>) typeof(ServerApi)
            .GetField("loadedAssemblies", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?
            .GetValue(null)!;
        var plugins = (List<PluginContainer>) typeof(ServerApi)
            .GetField("plugins", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)?
            .GetValue(null)!;

        string[] targetPathsArray = targetPaths
            .Select(Path.GetFileNameWithoutExtension)
            .ToArray()!;
        foreach (var p in targetPathsArray)
        {
            foreach (var c in plugins
                         .Where(c => c.Plugin.GetType().Assembly == loadedAssemblies[p])
                         .ToArray())
            {
                try
                {
                    c.Plugin.Dispose();
                    plugins.Remove(c);
                    TShock.Log.ConsoleInfo($"Plugin {c.Plugin.Name} v{c.Plugin.Version} (by {c.Plugin.Author}) disposed.");
                }
                catch (Exception ex)
                {
                    failedUnload.Add(c.Plugin.GetType().Assembly.GetName().Name!);
                    if (Config.PluginConfig.ConinueHotReloadWhenError)
                    {
                        TShock.Log.ConsoleError($"卸载 \"{c.Plugin.Name}\" 时出错.\n" +
                                                $"{ex}");
                    }
                    else
                    {
                        throw new Exception(
                            string.Format("卸载 \"{0}\" 时出错.", c.Plugin.Name), ex);
                    }
                    
                }
                
            }

            loadedAssemblies.Remove(p);
        }

        return failedUnload;
    }

    public static List<string> LoadPlugins(IEnumerable<string> targetPaths)
    {
        var failedLoad = new List<string>();
        _installedPluginsManifestCache = null;
        var flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        var loadedAssemblies = (Dictionary<string, Assembly>) typeof(ServerApi).GetField("loadedAssemblies", flag)?.GetValue(null)!;
        var game = (Main) typeof(ServerApi).GetField("game", flag)?.GetValue(null)!;
        var plugins = (List<PluginContainer>) typeof(ServerApi).GetField("plugins", flag)?.GetValue(null)!;
        foreach (var current in targetPaths)
        {
            var tsPluginPath = Path.Combine(AppContext.BaseDirectory, ServerApi.PluginsPath, current);
            if (loadedAssemblies.TryGetValue(current, out var asm))
            {
                if (asm.GetName().Equals(AssemblyName.GetAssemblyName(tsPluginPath)))
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
                            continue;
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
                        failedLoad.Add(type.FullName!);
                        if (Config.PluginConfig.ConinueHotReloadWhenError)
                        {
                            TShock.Log.ConsoleError($"Could not create an instance of plugin class \"{type.FullName}\".\n" +
                                                    $"{ex}");
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                string.Format("Could not create an instance of plugin class \"{0}\".", type.FullName), ex);
                        }
                    }
                }
            }
        }
        return failedLoad;
    }

    public static PluginVersionInfo[] GetInstalledPlugins()
    {
        var pluginAssemblyToFileNameMap = ((Dictionary<string, Assembly>?) typeof(ServerApi)
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
                    .TryGetValue(c.Plugin.GetType().Assembly, out var fileName)
                    ? fileName + ".dll"
                    : "",
                Version = PluginVersionOverrides.GetValueOrDefault(c.Plugin.GetType().Assembly.GetName().Name!, c.Plugin.Version)
            })
            .DistinctBy(i => i.AssemblyName) // to prevent from being crashed by some ClassLibrary1 ...
            .ToArray();

        return installedPlugins;
    }
    
    public static void ClearCache()
    {
        _installedPluginsManifestCache = null;
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