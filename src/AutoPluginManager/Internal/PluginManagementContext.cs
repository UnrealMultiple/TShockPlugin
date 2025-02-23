using Newtonsoft.Json;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Timers;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoPluginManager.Internal;

internal class PluginManagementContext
{
    //定时器
    private readonly System.Timers.Timer _timer = new();
    //更新事件
    public event Action<PluginUpdateInfo[]>? OnPluginUpdate;

    //本地插件清单
    public Dictionary<string, PluginVersionInfo> LocalPluginManifests => Utils.GetInstalledPlugins().ToDictionary(i => i.AssemblyName);

    //云端插件清单
    private Dictionary<string, PluginVersionInfo>? _clouldPluginManifests = null;
    public Dictionary<string, PluginVersionInfo> ClouldPluginManifests => this._clouldPluginManifests ??= FetchUpstreamManifest().ToDictionary(i => i.AssemblyName);

    private Dictionary<string, PluginContext>? _cloudPluginCache = null;
    //插件包缓存
    private Dictionary<string, PluginContext> CloudPluginCache => this._cloudPluginCache ??= this.ExtractPluginsFromCloud();

    public static readonly PluginManagementContext Instance = new();

    public readonly CultureInfo CultureInfo;


    /// <summary>
    /// 从压缩包中提取插件文件
    /// </summary>
    /// <param name="zipBuffer"></param>
    /// <returns></returns>
    public Dictionary<string, byte[]> UnZip(byte[] zipBuffer)
    {
        var files = new Dictionary<string, byte[]>(); ;
        using var ms = new MemoryStream(zipBuffer);
        using var zip = new ZipArchive(ms);
        foreach (var item in zip.Entries)
        {
            if (item.FullName.StartsWith("Plugins/"))
            {
                var fileName = item.FullName[8..];
                var ext = Path.GetExtension(fileName);
                using var stream = item.Open();
                using var ms2 = new MemoryStream();
                stream.CopyTo(ms2);
                files.Add(fileName, ms2.ToArray());
            }
        }
        return files;
    }

    /// <summary>
    /// 从api获取插件
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, PluginContext> ExtractPluginsFromCloud()
    {
        var buffer = ApiAdapter.Request(PluginAPIType.All).Result;
        return this.ExtractPluginFromBuffer(buffer);
    }

    /// <summary>
    /// 从byte[]中提取插件
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, PluginContext> ExtractPluginFromBuffer(byte[] zipBuffer)
    {
        var contexts = new Dictionary<string, PluginContext>();
        var fileBuffer = this.UnZip(zipBuffer);
        foreach (var (fileFullName, buffer) in fileBuffer)
        {
            var ext = Path.GetExtension(fileFullName);
            if (ext == ".dll")
            {
                var context = new PluginContext()
                {
                    AssemblyName = Path.GetFileNameWithoutExtension(fileFullName),
                    AssemblyBuffer = buffer,
                    PdbBuffer = fileBuffer.GetValueOrDefault(Path.ChangeExtension(fileFullName, ".pdb"), Array.Empty<byte>()),
                    ReadmdBuffer = fileBuffer.GetValueOrDefault(Path.ChangeExtension(fileFullName, ".md"), Array.Empty<byte>())
                };
                contexts.Add(context.AssemblyName, context);
            }
        }
        return contexts;
    }

    private PluginManagementContext()
    {
        this.CultureInfo = (CultureInfo) typeof(TShock).Assembly.GetType("TShockAPI.I18n")!.GetProperty(
            "TranslationCultureInfo",
            BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
        if (string.IsNullOrEmpty(this.CultureInfo.Name))
        {
            this.CultureInfo = new CultureInfo("en-US");
        }
        
        this._timer.AutoReset = true;
        this._timer.Enabled = true;
        this._timer.Interval = 60 * 10 * 1000;
        this._timer.Elapsed += this.OnTimer;
        this._timer.Start();
    }

    private void OnTimer(object? sender, ElapsedEventArgs e)
    {
        var availableUpdates = this.GetAvailableUpdates();
        if (availableUpdates.Any())
        {
            this.OnPluginUpdate?.Invoke(availableUpdates);
        }

    }

    /// <summary>
    /// 根据AssemblyName获取插件
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public PluginContext ExtractPluginsFromCloudAssemblyName(string assemblyName)
    {
        var buffer = ApiAdapter.Request(PluginAPIType.Alone, new Dictionary<string, string>()
        {
            { "assembly_name", assemblyName }
        }).Result;
        return this.ExtractPluginFromBuffer(buffer).Values.First();
    }




    /// <summary>
    /// 解析插件清单
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static PluginVersionInfo[] FetchUpstreamManifest()
    {
        var response = ApiAdapter.Request(PluginAPIType.AllManifests).Result;
        return JsonConvert.DeserializeObject<PluginVersionInfo[]>(Encoding.UTF8.GetString(response)) ?? Array.Empty<PluginVersionInfo>();
    }

    /// <summary>
    /// 对比缓存差异
    /// </summary>
    /// <returns></returns>
    public (string[], PluginVersionInfo[]) CheckManifestsVariation()
    {
        var manifest = FetchUpstreamManifest();
        var variation = manifest.Where(i => !this.ClouldPluginManifests.ContainsKey(i.AssemblyName) || i.Version > this.ClouldPluginManifests[i.AssemblyName].Version).Select(i => i.AssemblyName);
        return (variation.ToArray(), manifest);

    }

    /// <summary>
    /// 获取插件更新
    /// </summary>
    /// <returns></returns>
    public PluginUpdateInfo[] GetAvailableUpdates()
    {
        var (variation, manifest) = this.CheckManifestsVariation();
        if (variation.Length > 0)
        {
            this._clouldPluginManifests = manifest.ToDictionary(i => i.AssemblyName);
            foreach (var i in variation)
            {
                this.CloudPluginCache[i] = this.ExtractPluginsFromCloudAssemblyName(i);
            }
        }

        return this.LocalPluginManifests.Values
            .Join(this.ClouldPluginManifests.Values,
                c => c.AssemblyName, l => l.AssemblyName, // c: current, l: latest
                (c, l) => new PluginUpdateInfo(c, l))
            .Where(i =>
                i.Current!.Version < i.Latest.Version &&
                !Config.Instance.UpdateBlackList.Contains(i.Current.Name)) // use plugin name instead of assembly name for compatibility
            .ToArray();
    }

    /// <summary>
    /// 更新插件
    /// </summary>
    /// <param name="plugins"></param>
    public (PluginUpdateInfo[] plugins, string[] externalDlls) InstallOrUpdatePlugins(IEnumerable<string> plugins)
    {
        var success = new List<PluginUpdateInfo>();
        void InstallPlugin(string plugin)
        {
            if (!this.CloudPluginCache.TryGetValue(plugin, out var context))
            {
                return;
            }
            string path;
            string pdb;
            var current = this.LocalPluginManifests.GetValueOrDefault(plugin);
            if (current is not null)
            {
                path = Path.Combine(ServerApi.ServerPluginsDirectoryPath, current.FileName);
                pdb = Path.ChangeExtension(path, ".pdb");

            }
            else
            {
                path = Path.Combine(ServerApi.ServerPluginsDirectoryPath, plugin + ".dll");
                pdb = Path.ChangeExtension(path, ".pdb");

            }
            File.WriteAllBytes(path, context.AssemblyBuffer);
            File.WriteAllBytes(pdb, context.PdbBuffer);
            if (this.ClouldPluginManifests.TryGetValue(plugin, out var info))
            {
                success.Add(new(current, info));
            }
        }

        var pluginAssemblyNames = plugins.ToArray();
        pluginAssemblyNames.ForEach(InstallPlugin);
        var ed = this.ResolvePluginDependencies(pluginAssemblyNames);
        ed.ForEach(InstallPlugin);
        return (success.ToArray(), ed);
    }

    /// <summary>
    /// 解析插件依赖
    /// </summary>
    /// <param name="pluginAssemblyNames"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public string[] ResolvePluginDependencies(IEnumerable<string> pluginAssemblyNames)
    {
        var externalDependencies = new List<string>();
        foreach (var n in pluginAssemblyNames)
        {
            if(this.ClouldPluginManifests.TryGetValue(n, out var ext))
            {
                externalDependencies.AddRange(ext.Dependencies);
                externalDependencies.AddRange(this.ResolvePluginDependencies(ext.Dependencies));
            }
        }
        return externalDependencies.ToHashSet().ToArray();
    }
}