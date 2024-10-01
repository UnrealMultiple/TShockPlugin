using Terraria;
using System.Diagnostics;
using System.Reflection;
using TerrariaApi.Server;
using TShockAPI;
using Newtonsoft.Json;
using System.IO.Compression;

namespace AutoPluginManager;
internal class Utils
{
    private const string GiteeReleaseUrl = "https://gitee.com/kksjsj/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";

    private const string GithubReleaseUrl = "https://github.com/UnrealMultiple/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";

    private const string GiteePluginsUrl = "https://gitee.com/kksjsj/TShockPlugin/raw/master/Plugins.json";

    private const string GithubPluginsUrl = "https://raw.githubusercontent.com/UnrealMultiple/TShockPlugin/master/Plugins.json";

    public static readonly Dictionary<string, Version> HasUpdated = new();

    private const string TempSaveDir = "TempFile";

    private const string TempZipName = "Plugins.zip";

    public static void UnLoadPlugins(IEnumerable<string> Plugins)
    {
        foreach (var plugin in ServerApi.Plugins)
        {
            if (Plugins.Contains(plugin.Plugin.Name))
            { 
                plugin.Plugin.Dispose();
            }
        }
    }

    public static void LoadPlugins(IEnumerable<string> Plugins)
    {
        var flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        var loadedAssemblies = (Dictionary<string, Assembly>) typeof(ServerApi).GetField("loadedAssemblies", flag)?.GetValue(null)!;
        var game = (Main) typeof(ServerApi).GetField("game", flag)?.GetValue(null)!;
        var plugins = (List<PluginContainer>) typeof(ServerApi).GetField("plugins", flag)?.GetValue(null)!;
        foreach (var current in Plugins)
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
                loadedAssemblies?.Add(current, ass);
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
                            TShock.Log.ConsoleInfo(string.Format(
                           "Plugin {0} v{1} (by {2}) initiated.", pc.Plugin.Name, pc.Plugin.Version, pc.Plugin.Author),
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

    public static List<PluginUpdateInfo> GetUpdates()
    {
        var plugins = GetPlugins();
        var latestPluginList = GetRepoPlugin();
        List<PluginUpdateInfo> pluginUpdateList = new();
        foreach (var latestPluginInfo in latestPluginList)
        {
            foreach (var plugin in plugins)
            {
                if (plugin.Name == latestPluginInfo.Name && plugin.Version < latestPluginInfo.Version)
                {
                    pluginUpdateList.Add(new PluginUpdateInfo(plugin.Name, plugin.Author, latestPluginInfo.Version, plugin.Version, plugin.Path, latestPluginInfo.Path));
                }
            }
        }

        pluginUpdateList.RemoveAll(x => Config.PluginConfig.UpdateBlackList.Contains(x.Name));
        return pluginUpdateList;
    }

    private static List<PluginUpdateInfo> GetUpdates(List<PluginVersionInfo> latestPluginList)
    {
        List<PluginUpdateInfo> pluginUpdateList = new();
        var plugins = GetPlugins();
        foreach (var latestPluginInfo in latestPluginList)
        {
            foreach (var plugin in plugins)
            {
                if (plugin.Name == latestPluginInfo.Name && plugin.Version < latestPluginInfo.Version)
                {
                    pluginUpdateList.Add(new PluginUpdateInfo(plugin.Name, plugin.Author, latestPluginInfo.Version, plugin.Version, plugin.Path, latestPluginInfo.Path));
                }
            }
        }

        return pluginUpdateList;
    }

    public static List<PluginVersionInfo> GetRepoPlugin()
    {
        var plugins = GetPlugins();
        HttpClient httpClient = new();
        var response = httpClient.GetAsync(Config.PluginConfig.UseGithubSource ? GithubPluginsUrl : GiteePluginsUrl).Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(GetString("无法连接服务器"));
        }

        var json = response.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<List<PluginVersionInfo>>(json) ?? new();
    }

    public static List<PluginVersionInfo> GetPlugins()
    {
        List<PluginVersionInfo> plugins = new();
        //获取已安装的插件，并且读取插件信息和AssemblyName
        foreach (var plugin in ServerApi.Plugins)
        {
            var version = HasUpdated.ContainsKey(plugin.Plugin.Name) //将插件版本设为上次更新的新版本
                ? HasUpdated[plugin.Plugin.Name]
                : plugin.Plugin.Version;
            plugins.Add(new PluginVersionInfo()
            {
                AssemblyName = plugin.Plugin.GetType().Assembly.GetName().Name!,
                Author = plugin.Plugin.Author,
                Name = plugin.Plugin.Name,
                Description = plugin.Plugin.Description,
                Version = version
            });
        }
        var type = typeof(ServerApi);
        var field = type.GetField("loadedAssemblies", BindingFlags.NonPublic | BindingFlags.Static)!;
        if (field.GetValue(null) is Dictionary<string, Assembly> loadedAssemblies)
        {
            foreach (var (fileName, assembly) in loadedAssemblies)
            {
                for (var i = 0; i < plugins.Count; i++)
                {
                    if (plugins[i].AssemblyName == assembly.GetName().Name)
                    {
                        plugins[i].Path = fileName + ".dll";
                    }
                }
            }
        }

        return plugins;
    }


    public static void DownLoadPlugin()
    {
        DirectoryInfo directoryInfo = new(TempSaveDir);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        HttpClient httpClient = new();
        var zipBytes = httpClient.GetByteArrayAsync(Config.PluginConfig.UseGithubSource ? GithubReleaseUrl : GiteeReleaseUrl).Result;
        File.WriteAllBytes(Path.Combine(directoryInfo.FullName, TempZipName), zipBytes);
    }

    public static void ExtractDirectoryZip()
    {
        DirectoryInfo directoryInfo = new(TempSaveDir);
        ZipFile.ExtractToDirectory(Path.Combine(directoryInfo.FullName, TempZipName), Path.Combine(directoryInfo.FullName, "Plugins"), true);
    }

    public static void InstallPlugin(List<PluginVersionInfo> plugininfos)
    {
        foreach (var info in plugininfos)
        {
            var sourcePath = Path.Combine(TempSaveDir, "Plugins", "Plugins", info.Path);
            var destinationPath = Path.Combine(ServerApi.ServerPluginsDirectoryPath, info.Path);
            File.Copy(sourcePath, destinationPath, true);
            //热添加插件emmm
            //var ass = Assembly.Load(File.ReadAllBytes(destinationPath));
        }
        if (Directory.Exists(TempSaveDir))
        {
            Directory.Delete(TempSaveDir, true);
        }
    }

    public static List<PluginUpdateInfo> UpdatePlugin(List<PluginUpdateInfo> pluginUpdateInfos)
    {
        for (var i = pluginUpdateInfos.Count - 1; i >= 0; i--)
        {
            var currentPluginInfo = pluginUpdateInfos[i];
            var sourcePath = Path.Combine(TempSaveDir, "Plugins", "Plugins", currentPluginInfo.RemotePath);
            var destinationPath = Path.Combine(ServerApi.ServerPluginsDirectoryPath, currentPluginInfo.LocalPath);
            // 确保目标目录存在
            var destinationDirectory = Path.GetDirectoryName(destinationPath)!;
            if (File.Exists(destinationPath))
            {
                File.Copy(sourcePath, destinationPath, true);
                if (HasUpdated.ContainsKey(currentPluginInfo.Name))
                {
                    HasUpdated[currentPluginInfo.Name] = currentPluginInfo.NewVersion;
                }
                else
                {
                    HasUpdated.Add(currentPluginInfo.Name, currentPluginInfo.NewVersion);
                }
            }
            else
            {
                TShock.Log.ConsoleWarn(GetString($"[跳过更新]无法在本地找到插件{currentPluginInfo.Name}({destinationPath}),可能是云加载或使用-additionalplugins加载"));
                pluginUpdateInfos.RemoveAt(i);  // 移除元素
            }
        }
        if (Directory.Exists(TempSaveDir))
        {
            Directory.Delete(TempSaveDir, true);
        }

        return pluginUpdateInfos;
    }
}
