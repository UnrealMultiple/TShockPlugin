using Newtonsoft.Json;
using System.IO.Compression;
using System.Timers;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoPluginManager;

public class PluginManagementContext : IDisposable
{
    //定时器
    private readonly System.Timers.Timer _timer = new();
    //更新事件
    public event Action<PluginUpdateInfo[]>? OnPluginUpdate;
    //HTTP实例
    private static readonly HttpClient _httpClient = new();

    //本地插件清单
    public Dictionary<string, PluginVersionInfo> LocalPluginManifests => Utils.GetInstalledPlugins().ToDictionary(i => i.AssemblyName);
    
    //云端插件清单
    private Dictionary<string, PluginVersionInfo>? _clouldPluginManifests = null;
    public Dictionary<string, PluginVersionInfo> ClouldPluginManifests => this._clouldPluginManifests ??= FetchUpstreamManifest().ToDictionary(i => i.AssemblyName);

    private Dictionary<string, byte[]>? _zipPluginCache = null;
    //插件包缓存
    private Dictionary<string, byte[]> ZipPluginCache => this._zipPluginCache ??= this.EnsurePluginArchiveExtracted();

    /// <summary>
    /// 下载压缩包并解压到流中
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, byte[]> EnsurePluginArchiveExtracted()
    {
        var files = new Dictionary<string, byte[]>();
        var buffer = EnsurePluginArchiveDownloaded();
        using var ms = new MemoryStream(buffer);
        using var zip = new ZipArchive(ms);
        foreach(var item in zip.Entries)
        {
            if(item.FullName.StartsWith("Plugins/"))
            {
                var fileName = item.FullName[8..];
                using var stream = item.Open();
                using var ms2 = new MemoryStream();
                stream.CopyTo(ms2);
                files.Add(fileName, ms2.ToArray());
            }
        }
        return files;
    }

    public PluginManagementContext()
    {
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
    /// 解析插件清单
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static PluginVersionInfo[] FetchUpstreamManifest()
    {
        var response = _httpClient.GetAsync(Config.UpstreamPluginManifestUrl).Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(GetString("无法连接服务器"));
        }
        var json = response.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<PluginVersionInfo[]>(json) ?? Array.Empty<PluginVersionInfo>();
    }

    public static byte[] EnsurePluginArchiveDownloaded()
    {
        return _httpClient
            .GetByteArrayAsync(Config.UpstreamPluginArchiveUrl)
            .Result;
    }

    public bool CheckManifestsVariation()
    {
        var manifest = FetchUpstreamManifest();
        var isvariation = false;
        foreach (var item in manifest)
        {
            if (!this.ClouldPluginManifests.TryGetValue(item.AssemblyName, out var cloud))
            {
                isvariation = true;
                break;
            }
            else
            {
                if (item.Version > cloud.Version)
                {
                    isvariation = true;
                    break;
                }
            }
        }
        if (isvariation)
        {
            TShock.Log.ConsoleInfo(GetString("插件清单差异，正在进行新的缓存..."));
            this._zipPluginCache = this.EnsurePluginArchiveExtracted();
            this._clouldPluginManifests = manifest.ToDictionary(i => i.AssemblyName);
        }
        return isvariation;
    }

    /// <summary>
    /// 获取插件更新
    /// </summary>
    /// <returns></returns>
    public PluginUpdateInfo[] GetAvailableUpdates()
    {
        Task.Run(() => this.CheckManifestsVariation());
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
            if (!this.ZipPluginCache.TryGetValue(plugin + ".dll", out var buffer))
            {
                return;
            }
            
            var current = this.LocalPluginManifests.GetValueOrDefault(plugin);
            if (current is not null)
            {
                var path = Path.Combine(ServerApi.ServerPluginsDirectoryPath, current.FileName);
                var pdb = Path.ChangeExtension(path, ".pdb");
                File.WriteAllBytes(path, this.ZipPluginCache[plugin + ".dll"]);
                File.WriteAllBytes(pdb, this.ZipPluginCache[plugin + ".pdb"]);
            }
            else
            {
                var path = Path.Combine(ServerApi.ServerPluginsDirectoryPath, plugin + ".dll");
                var pdb = Path.ChangeExtension(path, ".pdb");
                File.WriteAllBytes(path, this.ZipPluginCache[plugin + ".dll"]);
                if(this.ZipPluginCache.TryGetValue(plugin + ".pdb", out var pdbBuffer))
                {
                    File.WriteAllBytes(pdb, pdbBuffer);
                }
            }
            if (this.ClouldPluginManifests.TryGetValue(plugin, out var info))
            {
                success.Add(new(current, info));
            }
        }

        var pluginAssemblyNames = plugins as string[] ?? plugins.ToArray();
        pluginAssemblyNames.ForEach(InstallPlugin);
        var ed = this.ResolvePluginDependencies(pluginAssemblyNames);
        ed.ForEach(InstallPlugin);
        return (success.ToArray(), ed);
    }

    public string[] ResolvePluginDependencies(IEnumerable<string> pluginAssemblyNames)
    {
        var externalDependencies = new HashSet<string>();
        foreach(var n in pluginAssemblyNames)
        {
            if (!this.ClouldPluginManifests.TryGetValue(n, out var latestPluginInfo))
            {
                throw new Exception($"Plugin with assembly name {n} not found in the manifest.");
            }
            latestPluginInfo.Dependencies.ForEach(d => externalDependencies.Add(d));
        }
        return externalDependencies.ToArray();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
        this._timer.Stop();
        this._timer.Dispose();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}