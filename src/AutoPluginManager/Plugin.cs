using Newtonsoft.Json;
using System.IO.Compression;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AutoPluginManager;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Name => "AutoPluginManager";

    public override Version Version => new(2, 0, 1, 3);

    public override string Author => "少司命，Cai";

    public override string Description => "自动更新你的插件！";

    private const string GiteeReleaseUrl = "https://gitee.com/kksjsj/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";

    private const string GithubReleaseUrl = "https://github.com/UnrealMultiple/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";

    private const string GiteePluginsUrl = "https://gitee.com/kksjsj/TShockPlugin/raw/master/Plugins.json";

    private const string GithubPluginsUrl = "https://raw.githubusercontent.com/UnrealMultiple/TShockPlugin/master/Plugins.json";

    public static readonly Dictionary<string, Version> HasUpdated = new();

    private const string TempSaveDir = "TempFile";

    private const string TempZipName = "Plugins.zip";

    private readonly System.Timers.Timer _timer = new();

    public Plugin(Main game) : base(game)
    {

    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new("AutoUpdatePlugin", this.PluginManager, "apm"));
        ServerApi.Hooks.GamePostInitialize.Register(this, this.AutoCheckUpdate, int.MinValue);
        Config.Read();
        GeneralHooks.ReloadEvent += this.GeneralHooksOnReloadEvent;

    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.PluginManager);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.AutoCheckUpdate);
            GeneralHooks.ReloadEvent -= this.GeneralHooksOnReloadEvent;
            this._timer.Stop();
            this._timer.Dispose();
        }

        base.Dispose(disposing);
    }
    private void GeneralHooksOnReloadEvent(ReloadEventArgs e)
    {
        Config.Read();
        e.Player.SendSuccessMessage("[AutoUpdatePlugin]插件配置已重载~");
    }
    private void AutoCheckUpdate(EventArgs args)
    {
        this._timer.AutoReset = true;
        this._timer.Enabled = true;
        this._timer.Interval = 60 * 30 * 1000;
        this._timer.Elapsed += (_, _) =>
        {
            try
            {
                var updates = GetUpdates();
                if (updates.Any())
                {
                    TShock.Log.ConsoleInfo("[以下插件有新的版本更新]\n" + string.Join("\n", updates.Select(i => $"[{i.Name}] V{i.OldVersion} >>> V{i.NewVersion}")));
                    if (Config.PluginConfig.AutoUpdate)
                    {
                        TShock.Log.ConsoleInfo("正在自动更新插件...");
                        this.UpdateCmd(TSPlayer.Server, Array.Empty<string>());
                    }
                    else
                    {
                        TShock.Log.ConsoleInfo("你可以使用命令/apm -u 更新插件哦~");
                    }


                }

            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleInfo("[AutoUpdate]无法获取更新:" + ex.Message);
                return;
            }
        };
        this._timer.Start();
    }

    private void PluginManager(CommandArgs args)
    {
        if (args.Parameters.Count == 1 && (args.Parameters[0].ToLower() == "-c" || args.Parameters[0].ToLower() == "c"))
        {
            this.CheckCmd(args.Player);
        }
        else if (args.Parameters.Count >= 1 && (args.Parameters[0].ToLower() == "-u" || args.Parameters[0].ToLower() == "u"))
        {
            var targets = Array.Empty<string>();
            if (args.Parameters.Count > 1)
            {
                targets = args.Parameters[1].Split(",");
            }

            this.UpdateCmd(args.Player, targets);
        }
        else if (args.Parameters.Count == 2 && (args.Parameters[0].ToLower() == "-i" || args.Parameters[0].ToLower() == "i"))
        {
            var indexs = args.Parameters[1].Split(",").Select(x =>
            {
                return int.TryParse(x, out var index) ? index : -1;
            });
            this.InstallCmd(args.Player, indexs);
        }
        else if (args.Parameters.Count == 1 && (args.Parameters[0].ToLower() == "-l" || args.Parameters[0].ToLower() == "l"))
        {
            var repo = GetRepoPlugin();
            args.Player.SendInfoMessage("可安装插件列表:");
            for (var i = 0; i < repo.Count; i++)
            {
                args.Player.SendInfoMessage($"{i + 1}.{repo[i].Name} v{repo[i].Version} (by {repo[i].Author}) - {repo[i].Description}");
            }

            args.Player.SendInfoMessage("*使用/apm -i <序号> 即可安装哦~");
        }
        else if (args.Parameters.Count == 2 && (args.Parameters[0].ToLower() == "-b" || args.Parameters[0].ToLower() == "b"))
        {
            var plugins = GetPlugins();
            if (!plugins.Exists(p => p.Name == args.Parameters[1]))
            {
                args.Player.SendErrorMessage("排除失败, 没有在你的插件列表里找到这个插件呢~");
                return;
            }
            Config.PluginConfig.UpdateBlackList.Add(args.Parameters[1]);
            Config.PluginConfig.Write();
            args.Player.SendSuccessMessage("排除成功, 已跳过此插件的更新检查~");
        }
        else if (args.Parameters.Count == 2 && (args.Parameters[0].ToLower() == "-rb" || args.Parameters[0].ToLower() == "rb"))
        {
            if (!Config.PluginConfig.UpdateBlackList.Contains(args.Parameters[1]))
            {
                args.Player.SendErrorMessage("删除失败, 没有在你的插件列表里找到这个插件呢~");
                return;
            }
            Config.PluginConfig.UpdateBlackList.Remove(args.Parameters[1]);
            Config.PluginConfig.Write();
            args.Player.SendSuccessMessage("删除成功, 此插件将会被检查更新~");
        }
        else if (args.Parameters.Count == 1 && (args.Parameters[0].ToLower() == "-lb" || args.Parameters[0].ToLower() == "lb"))
        {
            if (Config.PluginConfig.UpdateBlackList.Count == 0)
            {
                args.Player.SendSuccessMessage("当前没有排除任何一个插件哦~");
                return;
            }
            args.Player.SendErrorMessage("插件更新排除列表:\n" + string.Join('\n', Config.PluginConfig.UpdateBlackList));
        }
        else
        {
            args.Player.SendInfoMessage("apm c 检测已安装插件更新");
            args.Player.SendInfoMessage("apm u [插件名] 更新所有插件或指定插件");
            args.Player.SendInfoMessage("apm i [序号] 安装指定插件");
            args.Player.SendInfoMessage("apm l 查看可安装插件表");
            args.Player.SendInfoMessage("apm b [插件名字] 更新时跳过指定插件");
            args.Player.SendInfoMessage("apm rb [插件名字] 取消更新排除");
            args.Player.SendInfoMessage("apm lb 查看更新排除列表");
        }
    }

    private void InstallCmd(TSPlayer Player, IEnumerable<int> target)
    {
        if (!target.Any())
        {
            Player.SendErrorMessage("无效参数，请附带需要安装插件的选择项!");
            return;
        }
        try
        {
            var plugins = GetRepoPlugin();
            var installs = new List<PluginVersionInfo>();
            foreach (var index in target)
            {
                if (index > 0 && index <= plugins.Count)
                {
                    installs.Add(plugins[index - 1]);
                }
            }
            if (installs.Count == 0)
            {
                Player.SendErrorMessage("序号无效，请附带需要安装插件的选择项!");
                return;
            }
            Player.SendInfoMessage("正在下载最新插件包...");
            DownLoadPlugin();
            Player.SendInfoMessage("正在解压插件包...");
            ExtractDirectoryZip();
            Player.SendInfoMessage("正在安装插件...");
            InstallPlugin(installs);
            Player.SendSuccessMessage("[安装完成]\n" + string.Join("\n", installs.Select(i => $"[{i.Name}] V{i.Version}")));
            Player.SendSuccessMessage("重启服务器后插件生效!");
        }
        catch (Exception ex)
        {
            Player.SendErrorMessage("安装插件出现错误:" + ex.Message);
        }
    }

    private void UpdateCmd(TSPlayer Player, string[] target)
    {
        try
        {
            var updates = GetUpdates();
            if (updates.Count == 0)
            {
                Player.SendSuccessMessage("你的插件全是最新版本，无需更新哦~");
                return;
            }
            if (target.Length != 0)
            {
                updates = updates.Where(i => target.Contains(i.Name)).ToList();
                if (!updates.Any())
                {
                    Player.SendErrorMessage($"{string.Join(",", target)} 无需更新!");
                    return;
                }
            }
            Player.SendInfoMessage("正在下载最新插件包...");
            DownLoadPlugin();
            Player.SendInfoMessage("正在解压插件包...");
            ExtractDirectoryZip();
            Player.SendInfoMessage("正在升级插件...");
            var success = UpdatePlugin(updates);
            if (success.Count == 0)
            {
                Player.SendSuccessMessage("更新了个寂寞~");
                return;
            }
            Player.SendSuccessMessage("[更新完成]\n" + string.Join("\n", success.Select(i => $"[{i.Name}] V{i.OldVersion} >>> V{i.NewVersion}")));
            Player.SendSuccessMessage("重启服务器后插件生效!");
        }
        catch (Exception ex)
        {
            Player.SendErrorMessage("自动更新出现错误:" + ex.Message);
            return;
        }
    }

    private void CheckCmd(TSPlayer Player)
    {
        try
        {
            var updates = GetUpdates();
            if (updates.Count == 0)
            {
                Player.SendSuccessMessage("你的插件全是最新版本，无需更新哦~");
                return;
            }
            Player.SendInfoMessage("[以下插件有新的版本更新]\n" + string.Join("\n", updates.Select(i => $"[{i.Name}] V{i.OldVersion} >>> V{i.NewVersion}")));
        }
        catch (Exception ex)
        {
            Player.SendErrorMessage("无法获取更新:" + ex.Message);
            return;
        }
    }

    #region 工具方法
    private static List<PluginUpdateInfo> GetUpdates()
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

    private static List<PluginVersionInfo> GetRepoPlugin()
    {
        var plugins = GetPlugins();
        HttpClient httpClient = new();
        var response = httpClient.GetAsync(Config.PluginConfig.UseGithubSource ? GithubPluginsUrl : GiteePluginsUrl).Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("无法连接服务器");
        }

        var json = response.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<List<PluginVersionInfo>>(json) ?? new();
    }

    private static List<PluginVersionInfo> GetPlugins()
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


    private static void DownLoadPlugin()
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

    private static void ExtractDirectoryZip()
    {
        DirectoryInfo directoryInfo = new(TempSaveDir);
        ZipFile.ExtractToDirectory(Path.Combine(directoryInfo.FullName, TempZipName), Path.Combine(directoryInfo.FullName, "Plugins"), true);
    }

    private static void InstallPlugin(List<PluginVersionInfo> plugininfos)
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

    private static List<PluginUpdateInfo> UpdatePlugin(List<PluginUpdateInfo> pluginUpdateInfos)
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
                TShock.Log.ConsoleWarn($"[跳过更新]无法在本地找到插件{currentPluginInfo.Name}({destinationPath}),可能是云加载或使用-additionalplugins加载");
                pluginUpdateInfos.RemoveAt(i);  // 移除元素
            }
        }
        if (Directory.Exists(TempSaveDir))
        {
            Directory.Delete(TempSaveDir, true);
        }

        return pluginUpdateInfos;
    }
    #endregion
}