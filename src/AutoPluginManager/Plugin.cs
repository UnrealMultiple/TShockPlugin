using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AutoPluginManager;

[ApiVersion(2, 1)]
// ReSharper disable once UnusedType.Global
public class Plugin : TerrariaPlugin
{
    public override string Name => "AutoPluginManager";

    public override Version Version => new (2, 0, 2, 3);

    public override string Author => "少司命,Cai,LaoSparrow";

    public override string Description => "自动更新你的插件！";


    private readonly System.Timers.Timer _timer = new ();

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new ("AutoUpdatePlugin", this.PluginManager, "apm"));
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
        e.Player.SendSuccessMessage(GetString("[AutoUpdatePlugin]插件配置已重载~"));
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
                using var context = PluginManagementContext.CreateDefault();
                var availableUpdates = context.GetAvailableUpdates();
                CheckDuplicatePlugins(TSPlayer.Server);
                if (!availableUpdates.Any())
                {
                    return;
                }

                TShock.Log.ConsoleInfo(GetString("[以下插件有新的版本更新]\n" + string.Join("\n", availableUpdates.Select(i => $"[{i.Current.Name}] V{i.Current.Version} >>> V{i.Latest.Version}"))));
                if (Config.PluginConfig.AutoUpdate)
                {
                    TShock.Log.ConsoleInfo(GetString("正在自动更新插件..."));
                    UpdateCmd(TSPlayer.Server, Array.Empty<string>());
                }
                else
                {
                    TShock.Log.ConsoleInfo(GetString("你可以使用命令/apm -u 更新插件哦~"));
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleInfo(GetString("[AutoUpdate]无法获取更新:") + ex);
            }
        };
        this._timer.Start();
    }

    private static void CheckDuplicatePlugins(TSPlayer ply)
    {
        if (typeof(ServerApi)
                .GetField(
                    "loadedAssemblies",
                    BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                !.GetValue(null) is not Dictionary<string, Assembly> loadedAssemblies)
        {
            return;
        }

        var duplicates = loadedAssemblies
            .GroupBy(x => x.Value.GetName().FullName)
            .Where(x => x.Count() > 1)
            .SelectMany(x => x)
            .ToArray();
        if (duplicates.Any())
        {
            ply.SendErrorMessage(GetString("[插件重复安装]") + string.Join(" >>> ", duplicates.Select(x => x.Key + ".dll")));
        }
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

            UpdateCmd(args.Player, targets);
        }
        else if (args.Parameters.Count == 2 && (args.Parameters[0].ToLower() == "-i" || args.Parameters[0].ToLower() == "i"))
        {
            var indices = args.Parameters[1]
                .Split(",")
                .Select(x => int.TryParse(x, out var index) ? index : -1)
                .ToArray();
            this.InstallCmd(args.Player, indices);
        }
        else if (args.Parameters.Count == 1 && (args.Parameters[0].ToLower() == "-l" || args.Parameters[0].ToLower() == "l"))
        {
            using var context = PluginManagementContext.CreateDefault();
            var manifest = context.Manifest;
            args.Player.SendInfoMessage(GetString("可安装插件列表:"));
            for (var i = 0; i < manifest.Length; i++)
            {
                args.Player.SendInfoMessage($"{i + 1}.{manifest[i].Name} v{manifest[i].Version} (by {manifest[i].Author}) - {manifest[i].Description}");
            }

            args.Player.SendInfoMessage(GetString("*使用/apm -i <序号> 即可安装哦~"));
        }
        else if (args.Parameters.Count == 2 && (args.Parameters[0].ToLower() == "-b" || args.Parameters[0].ToLower() == "b"))
        {
            var plugins = Utils.InstalledPluginsManifestCache.Values;
            if (plugins.All(p => p.Name != args.Parameters[1]))
            {
                args.Player.SendErrorMessage(GetString("排除失败, 没有在你的插件列表里找到这个插件呢~"));
                return;
            }

            if (Config.PluginConfig.UpdateBlackList.Contains(args.Parameters[1]))
            {
                args.Player.SendErrorMessage(GetString("排除失败, 已经排除过这个插件了呢~"));
                return;
            }

            Config.PluginConfig.UpdateBlackList.Add(args.Parameters[1]);
            Config.PluginConfig.Write();
            args.Player.SendSuccessMessage(GetString("排除成功, 已跳过此插件的更新检查~"));
        }
        else if (args.Parameters.Count == 1 && (args.Parameters[0].ToLower() == "-r" || args.Parameters[0].ToLower() == "r"))
        {
            CheckDuplicatePlugins(args.Player);
        }
        else if (args.Parameters.Count == 2 && (args.Parameters[0].ToLower() == "-rb" || args.Parameters[0].ToLower() == "rb"))
        {
            if (!Config.PluginConfig.UpdateBlackList.Contains(args.Parameters[1]))
            {
                args.Player.SendErrorMessage(GetString("删除失败, 没有在你的插件列表里找到这个插件呢~"));
                return;
            }

            Config.PluginConfig.UpdateBlackList.Remove(args.Parameters[1]);
            Config.PluginConfig.Write();
            args.Player.SendSuccessMessage(GetString("删除成功, 此插件将会被检查更新~"));
        }
        else if (args.Parameters.Count == 1 && (args.Parameters[0].ToLower() == "-lb" || args.Parameters[0].ToLower() == "lb"))
        {
            if (!Config.PluginConfig.UpdateBlackList.Any())
            {
                args.Player.SendSuccessMessage(GetString("当前没有排除任何一个插件哦~"));
                return;
            }

            args.Player.SendErrorMessage(GetString("插件更新排除列表:\n") + string.Join('\n', Config.PluginConfig.UpdateBlackList));
        }
        else
        {
            args.Player.SendInfoMessage(GetString("apm c 检测已安装插件更新"));
            args.Player.SendInfoMessage(GetString("apm u [插件名] 更新所有插件或指定插件"));
            args.Player.SendInfoMessage(GetString("apm i [序号] 安装指定插件"));
            args.Player.SendInfoMessage(GetString("apm l 查看可安装插件表"));
            args.Player.SendInfoMessage(GetString("apm b [插件名字] 更新时跳过指定插件"));
            args.Player.SendInfoMessage(GetString("apm rb [插件名字] 取消更新排除"));
            args.Player.SendInfoMessage(GetString("apm lb 查看更新排除列表"));
        }
    }

    private void InstallCmd(TSPlayer player, params int[] targets)
    {
        if (!targets.Any())
        {
            player.SendErrorMessage(GetString("无效参数，请附带需要安装插件的选择项!"));
            return;
        }

        try
        {
            using var context = PluginManagementContext.CreateDefault();
            var availablePlugins = context.Manifest;
            var pendingPlugins = targets
                .Where(i => i > 0 && i <= availablePlugins.Length)
                .Select(i => availablePlugins[i - 1])
                .ToList();
            if (!pendingPlugins.Any())
            {
                player.SendErrorMessage(GetString("序号无效，请附带需要安装插件的选择项!"));
                return;
            }

            player.SendInfoMessage(GetString("正在下载最新插件包..."));
            context.EnsurePluginArchiveDownloaded();
            player.SendInfoMessage(GetString("正在解压插件包..."));
            context.EnsurePluginArchiveExtracted();
            player.SendInfoMessage(GetString("正在安装插件..."));
            var success = context.InstallOrUpdatePlugins(pendingPlugins.Select(x => x.AssemblyName));

            if (!success.plugins.Any())
            {
                player.SendSuccessMessage(GetString("安装了个寂寞~"));
                return;
            }
            var failedUnload = new List<string>();
            var failedLoad = new List<string>();
            if (Config.PluginConfig.HotReloadPlugin)
            {
                
                failedUnload = Utils.UnLoadPlugins(success.plugins
                    .Where(s => s.Current is not null && s.Current.HotReload)
                    .Select(s => s.Current!.Path));
                failedLoad = Utils.LoadPlugins(success.plugins
                    .Select(s => s.Current is not null && s.Current.HotReload  && !failedUnload.Contains(s.Current.Name) ? s.Current.Path : s.Latest.Path));
            }
            player.SendFormattedServerPluginsModifications(success);

            if (Config.PluginConfig.HotReloadPlugin)
            {
                if (failedUnload.Any())
                {
                    player.SendWarningMessage(GetString($"*卸载失败: {string.Join(',',failedUnload)}"));
                }
                if (failedLoad.Any())
                {
                    player.SendWarningMessage(GetString($"*加载失败: {string.Join(',',failedLoad)}"));
                }
                if (failedLoad.Any() || failedLoad.Any())
                {
                    player.SendWarningMessage(GetString("*热加载失败的插件需要重启服务器后才会生效!"));
                }
                else
                {
                    player.SendSuccessMessage(GetString("*热重载已启用,安装的插件已生效!"));
                }
                
            }
            else
            {
                player.SendSuccessMessage(GetString("*热重载已关闭,插件需要重启服务器后才会生效!"));
            }

            
        }
        catch (Exception ex)
        {
            player.SendErrorMessage(GetString("安装插件出现错误:") + ex);
        }
    }

    private static void UpdateCmd(TSPlayer player, params string[] targets)
    {
        try
        {
            using var context = PluginManagementContext.CreateDefault();
            var updates = context.GetAvailableUpdates();
            if (!updates.Any())
            {
                player.SendSuccessMessage(GetString("你的插件全是最新版本，无需更新哦~"));
                return;
            }

            if (targets.Any())
            {
                updates = updates
                    .Where(i => targets.Contains(i.Current!.Name))
                    .ToArray();
                if (!updates.Any())
                {
                    player.SendErrorMessage($"{string.Join(",", targets)} 无需更新!");
                    return;
                }
            }
            
            player.SendInfoMessage(GetString("正在下载最新插件包..."));
            context.EnsurePluginArchiveDownloaded();
            player.SendInfoMessage(GetString("正在解压插件包..."));
            context.EnsurePluginArchiveExtracted();
            player.SendInfoMessage(GetString("正在升级插件..."));
            var success = context.InstallOrUpdatePlugins(updates.Select(x => x.Latest.AssemblyName));
            if (!success.plugins.Any())
            {
                player.SendSuccessMessage(GetString("更新了个寂寞~"));
                return;
            }

            var failedUnload = new List<string>();
            var failedLoad = new List<string>();
            if (Config.PluginConfig.HotReloadPlugin)
            {
                
                failedUnload = Utils.UnLoadPlugins(success.plugins
                    .Where(s => s.Current is not null && s.Current.HotReload)
                    .Select(s => s.Current!.Path));
                failedLoad = Utils.LoadPlugins(success.plugins
                    .Select(s => s.Current is not null && s.Current.HotReload  && !failedUnload.Contains(s.Current.Name) ? s.Current.Path : s.Latest.Path));
            }
            player.SendFormattedServerPluginsModifications(success);

            if (Config.PluginConfig.HotReloadPlugin)
            {
                if (failedUnload.Any())
                {
                    player.SendWarningMessage(GetString($"*卸载失败: {string.Join(',',failedUnload)}"));
                }
                if (failedLoad.Any())
                {
                    player.SendWarningMessage(GetString($"*加载失败: {string.Join(',',failedLoad)}"));
                }
                if (failedLoad.Any() || failedLoad.Any())
                {
                    player.SendWarningMessage(GetString("*热加载失败的插件需要重启服务器后才会生效!"));
                }
                else
                {
                    player.SendSuccessMessage(GetString("*热重载已启用,安装的插件已生效!"));
                }
                
            }
            else
            {
                player.SendSuccessMessage(GetString("*热重载已关闭,插件需要重启服务器后才会生效!"));
            }
        }
        catch (Exception ex)
        {
            player.SendErrorMessage(GetString("自动更新出现错误:") + ex);
        }
    }

    private void CheckCmd(TSPlayer Player)
    {
        try
        {
            using var context = PluginManagementContext.CreateDefault();
            var updates = context.GetAvailableUpdates();
            if (updates.Length == 0)
            {
                Player.SendSuccessMessage(GetString("你的插件全是最新版本，无需更新哦~"));
                return;
            }

            Player.SendInfoMessage(GetString("[以下插件有新的版本更新]\n") + string.Join("\n", updates.Select(i => $"[{i.Current?.Name ?? i.Latest.Name}] V{i.Current?.Version} >>> V{i.Latest.Version}")));
        }
        catch (Exception ex)
        {
            Player.SendErrorMessage(GetString("无法获取更新:") + ex);
        }
    }
}