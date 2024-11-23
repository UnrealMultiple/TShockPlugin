using Newtonsoft.Json;
using Org.BouncyCastle.Math.EC;
using System.IO.Compression;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoPluginManager;

public class PluginManagementContext : IDisposable
{
    public enum States
    {
        Clean,
        ArchiveDownloaded,
        ArchiveExtracted,
    }

    public string UpstreamManifestUrl { get; init; } = Utils.GithubPluginManifestUrl;
    public string UpstreamPluginArchiveUrl { get; init; } = Utils.GithubPluginArchiveUrl;

    public string CacheDir { get; init; } = "APMCache";
    public string PluginArchiveFileName { get; init; } = "Plugins.zip";

    private PluginVersionInfo[]? _manifest;
    public PluginVersionInfo[] Manifest => this._manifest ??= this.FetchUpstreamManifest();

    private Dictionary<string, PluginVersionInfo>? _manifestCache;
    public Dictionary<string, PluginVersionInfo> ManifestCache => this._manifestCache ??= this.Manifest.ToDictionary(i => i.AssemblyName);

    public States State { get; private set; } = States.Clean;

    public PluginVersionInfo[] FetchUpstreamManifest()
    {
        HttpClient httpClient = new ();
        var response = httpClient.GetAsync(this.UpstreamManifestUrl).Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(GetString("无法连接服务器"));
        }

        var json = response.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<PluginVersionInfo[]>(json) ?? Array.Empty<PluginVersionInfo>();
    }

    public void EnsurePluginArchiveDownloaded()
    {
        if (this.State >= States.ArchiveDownloaded)
        {
            return;
        }

        if (!Directory.Exists(this.CacheDir))
        {
            Directory.CreateDirectory(this.CacheDir);
        }

        HttpClient httpClient = new ();
        var zipBytes = httpClient
            .GetByteArrayAsync(this.UpstreamPluginArchiveUrl)
            .Result;
        File.WriteAllBytes(
            Path.Combine(this.CacheDir, this.PluginArchiveFileName),
            zipBytes);
        this.State = States.ArchiveDownloaded;
    }

    public void EnsurePluginArchiveExtracted()
    {
        if (this.State >= States.ArchiveExtracted)
        {
            return;
        }

        this.EnsurePluginArchiveDownloaded();
        ZipFile.ExtractToDirectory(
            Path.Combine(this.CacheDir, this.PluginArchiveFileName),
            Path.Combine(this.CacheDir, "Plugins"),
            true);
        this.State = States.ArchiveExtracted;
    }

    public PluginUpdateInfo[] GetAvailableUpdates()
    {
        return Utils.InstalledPluginsManifestCache.Values
            .Join(this.ManifestCache.Values,
                c => c.AssemblyName, l => l.AssemblyName, // c: current, l: latest
                (c, l) => new PluginUpdateInfo(c, l))
            .Where(i =>
                i.Current!.Version < i.Latest.Version &&
                !Config.PluginConfig.UpdateBlackList.Contains(i.Current.Name)) // use plugin name instead of assembly name for compatibility
            .ToArray();
    }

    public (PluginUpdateInfo[] updates, string[] externalDlls) ResolvePluginDependencies(string pluginAssemblyName)
    {
        // use hashset to solve loop dependencies
        var pendingUpdates = new HashSet<PluginUpdateInfo>(new PluginUpdateInfo.AssemblyNameEqualityComparer());
        var externalDependencies = new List<string>();
        var stack = new Stack<PluginUpdateInfo>();
        if (!this.ManifestCache.TryGetValue(pluginAssemblyName, out var latestPluginInfo))
        {
            throw new Exception($"Plugin with assembly name {pluginAssemblyName} not found in the manifest.");
        }

        stack.Push(new PluginUpdateInfo(
            Utils.InstalledPluginsManifestCache.GetValueOrDefault(pluginAssemblyName, null!),
            latestPluginInfo));
        while (stack.Count > 0)
        {
            var updateInfo = stack.Pop();
            pendingUpdates.Add(updateInfo);
            foreach (var d in updateInfo.Latest.Dependencies)
            {
                if (this.ManifestCache.TryGetValue(d, out var info))
                {
                    stack.Push(new PluginUpdateInfo(
                        Utils.InstalledPluginsManifestCache.GetValueOrDefault(d, null!),
                        info));
                }
                else
                {
                    externalDependencies.Add(d + ".dll");
                }
            }
        }

        return (pendingUpdates.ToArray(), externalDependencies.ToArray());
    }

    public (PluginUpdateInfo[] plugins, string[] externalDlls) InstallOrUpdatePlugins(IEnumerable<string> targetPlugins)
    {
        this.EnsurePluginArchiveExtracted();

        var pluginsPassedCheck = new List<(PluginUpdateInfo[] updates, string[] externalDlls)>();
        foreach (var n in targetPlugins)
        {
            var pending = this.ResolvePluginDependencies(n);
            var currentPlugin = Utils.InstalledPluginsManifestCache.GetValueOrDefault(n, this.ManifestCache[n]);

            // filter out those plugins which doesn't need update
            pending.updates = pending.updates
                .Where(u => u.Current is null || u.Current.Version < u.Latest.Version)
                .ToArray();

            var bannedDependencies = pending.updates
                .Where(u => Config.PluginConfig.UpdateBlackList.Contains(u.Current?.Name ?? u.Latest.Name))
                .Select(u => u.Current?.Name ?? u.Latest.Name)
                .ToArray();
            if (bannedDependencies.Any())
            {
                TShock.Log.ConsoleWarn(
                    GetString($"[跳过安装或更新] 插件{currentPlugin.Name}({currentPlugin.Path})的其中一项依赖被禁止安装或更新\n") +
                    string.Join('\n', bannedDependencies));
                continue;
            }

            var installedPluginsWhichDontExistLocally = pending.updates
                .Where(u =>
                    u.Current is not null &&
                    !File.Exists(Path.Combine(ServerApi.ServerPluginsDirectoryPath, u.Current.Path)))
                .Select(u => u.Current!.Name)
                .ToArray();
            if (installedPluginsWhichDontExistLocally.Any())
            {
                TShock.Log.ConsoleWarn(
                    GetString($"[跳过安装或更新] 无法在本地找到插件{currentPlugin.Name}({currentPlugin.Path})或其其中一项依赖,可能是云加载或使用-additionalplugins加载\n") +
                    string.Join('\n', installedPluginsWhichDontExistLocally));
                continue;
            }

            pluginsPassedCheck.Add(pending);
        }

        var successfullyUpdatedPlugins = pluginsPassedCheck
            .SelectMany(x => x.updates)
            .DistinctBy(u => u.Latest.AssemblyName)
            .ToArray();
        foreach (var u in successfullyUpdatedPlugins)
        {
            CopyPluginFile(u.Latest.Path, u.Current?.Path ?? u.Latest.Path);
            Utils.PluginVersionOverrides[u.Current?.AssemblyName ?? u.Latest.AssemblyName] = u.Latest.Version;
        }
        Utils.ClearCache();

        var successfullyUpdatedExternalDlls = pluginsPassedCheck
            .SelectMany(x => x.externalDlls)
            .Distinct()
            .ToArray();
        foreach (var ed in successfullyUpdatedExternalDlls)
        {
            CopyPluginFile(ed, ed);
        }

        return (successfullyUpdatedPlugins, successfullyUpdatedExternalDlls);

        void CopyPluginFile(string src, string dest)
        {
            var dllSrcPath = Path.Combine(this.CacheDir, "Plugins", "Plugins", src);
            var dllDestPath = Path.Combine(ServerApi.ServerPluginsDirectoryPath, dest);
            File.Copy(dllSrcPath, dllDestPath, true);

            var pdbSrcPath = Path.ChangeExtension(dllSrcPath, ".pdb");
            var pdbDestPath = Path.ChangeExtension(dllDestPath, ".pdb");
            if (File.Exists(pdbSrcPath))
            {
                File.Copy(pdbSrcPath, pdbDestPath, true);
            }
        }
    }

    public void CleanUp()
    {
        if (Directory.Exists(this.CacheDir))
        {
            Directory.Delete(this.CacheDir, true);
        }

        this.State = States.Clean;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }

        this.CleanUp();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public static PluginManagementContext CreateDefault()
    {
        if (Config.PluginConfig.UseCustomSource)
        {
            return new PluginManagementContext { UpstreamManifestUrl = Config.PluginConfig.CustomSourceManifestUrl, UpstreamPluginArchiveUrl = Config.PluginConfig.CustomSourceArchiveUrl };
        }

        return new PluginManagementContext { UpstreamManifestUrl = Config.PluginConfig.UseGithubSource ? Utils.GithubPluginManifestUrl : Utils.GiteePluginManifestUrl, UpstreamPluginArchiveUrl = Config.PluginConfig.UseGithubSource ? Utils.GithubPluginArchiveUrl : Utils.GiteePluginArchiveUrl };
    }
}