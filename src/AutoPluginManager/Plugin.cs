using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Reflection.Metadata;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AutoPluginManager;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Name => "AutoPluginManager";

    public override Version Version => new(2, 0, 1, 7);

    public override string Author => "少司命，Cai";

    public override string Description => "自动更新你的插件！";

    
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
                var updates = Utils.GetUpdates();
                PluginRepeat(TSPlayer.Server);
                if (updates.Any())
                {
                    TShock.Log.ConsoleInfo(GetString("[以下插件有新的版本更新]\n" + string.Join("\n", updates.Select(i => $"[{i.Name}] V{i.OldVersion} >>> V{i.NewVersion}"))));
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

            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleInfo(GetString("[AutoUpdate]无法获取更新:") + ex.Message);
                return;
            }
        };
        this._timer.Start();
    }

    private static void PluginRepeat(TSPlayer ply)
    {
        if (typeof(ServerApi)
            .GetField("loadedAssemblies", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
            !.GetValue(null) is Dictionary<string, Assembly> loadassemblys)
        {
            var mutexs = loadassemblys
                .GroupBy(x => x.Value.GetName().FullName)
                .Where(x => x.Count() > 1)
                .SelectMany(x => x);
            if (mutexs.Any())
            {
                ply.SendErrorMessage(GetString("[插件重复安装]") + string.Join(" >>> ", mutexs.Select(x => x.Key + ".dll")));
            }
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
            var indexs = args.Parameters[1].Split(",").Select(x => int.TryParse(x, out var index) ? index : -1);
            this.InstallCmd(args.Player, indexs);
        }
        else if (args.Parameters.Count == 1 && (args.Parameters[0].ToLower() == "-l" || args.Parameters[0].ToLower() == "l"))
        {
            var repo = Utils.GetRepoPlugin();
            args.Player.SendInfoMessage(GetString("可安装插件列表:"));
            for (var i = 0; i < repo.Count; i++)
            {
                args.Player.SendInfoMessage($"{i + 1}.{repo[i].Name} v{repo[i].Version} (by {repo[i].Author}) - {repo[i].Description}");
            }

            args.Player.SendInfoMessage(GetString("*使用/apm -i <序号> 即可安装哦~"));
        }
        else if (args.Parameters.Count == 2 && (args.Parameters[0].ToLower() == "-b" || args.Parameters[0].ToLower() == "b"))
        {
            var plugins = Utils.GetPlugins();
            if (!plugins.Exists(p => p.Name == args.Parameters[1]))
            {
                args.Player.SendErrorMessage(GetString("排除失败, 没有在你的插件列表里找到这个插件呢~"));
                return;
            }
            Config.PluginConfig.UpdateBlackList.Add(args.Parameters[1]);
            Config.PluginConfig.Write();
            args.Player.SendSuccessMessage(GetString("排除成功, 已跳过此插件的更新检查~"));
        }
        else if (args.Parameters.Count == 1 && (args.Parameters[0].ToLower() == "-r" || args.Parameters[0].ToLower() == "r"))
        {
            PluginRepeat(args.Player);
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
            if (Config.PluginConfig.UpdateBlackList.Count == 0)
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

    private void InstallCmd(TSPlayer Player, IEnumerable<int> target)
    {
        if (!target.Any())
        {
            Player.SendErrorMessage(GetString("无效参数，请附带需要安装插件的选择项!"));
            return;
        }
        try
        {
            var plugins = Utils.GetRepoPlugin();
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
                Player.SendErrorMessage(GetString("序号无效，请附带需要安装插件的选择项!"));
                return;
            }
            Player.SendInfoMessage(GetString("正在下载最新插件包..."));
            Utils.DownLoadPlugin();
            Player.SendInfoMessage(GetString("正在解压插件包..."));
            Utils.ExtractDirectoryZip();
            Player.SendInfoMessage(GetString("正在安装插件..."));
            Utils.InstallPlugin(installs);
            Player.SendSuccessMessage(GetString("[安装完成]\n") + string.Join("\n", installs.Select(i => $"[{i.Name}] V{i.Version}")));
            Utils.LoadPlugins(installs.Select(x => x.Path));
        }
        catch (Exception ex)
        {
            Player.SendErrorMessage(GetString("安装插件出现错误:") + ex.Message);
        }
    }

    private static void UpdateCmd(TSPlayer Player, string[] target)
    {
        try
        {
            var updates = Utils.GetUpdates();
            if (updates.Count == 0)
            {
                Player.SendSuccessMessage(GetString("你的插件全是最新版本，无需更新哦~"));
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
            Player.SendInfoMessage(GetString("正在下载最新插件包..."));
            Utils.DownLoadPlugin();
            Player.SendInfoMessage(GetString("正在解压插件包..."));
            Utils.ExtractDirectoryZip();
            Player.SendInfoMessage(GetString("正在升级插件..."));
            var success =  Utils.UpdatePlugin(updates);
            if (success.Count == 0)
            {
                Player.SendSuccessMessage(GetString("更新了个寂寞~"));
                return;
            }
            if (Config.PluginConfig.AutoReloadPlugin)
            {
                Utils.UnLoadPlugins(success.Select(x => x.Name));
                Utils.LoadPlugins(success.Select(x => x.LocalPath));
            }

            Player.SendSuccessMessage(GetString("[更新完成]\n") + string.Join("\n", success.Select(i => $"[{i.Name}] V{i.OldVersion} >>> V{i.NewVersion}")));
            Player.SendSuccessMessage(GetString("重启服务器后插件生效!"));
        }
        catch (Exception ex)
        {
            Player.SendErrorMessage(GetString("自动更新出现错误:") + ex.Message);
            return;
        }
    }

    private void CheckCmd(TSPlayer Player)
    {
        try
        {
            var updates = Utils.GetUpdates();
            if (updates.Count == 0)
            {
                Player.SendSuccessMessage(GetString("你的插件全是最新版本，无需更新哦~"));
                return;
            }
            Player.SendInfoMessage(GetString("[以下插件有新的版本更新]\n") + string.Join("\n", updates.Select(i => $"[{i.Name}] V{i.OldVersion} >>> V{i.NewVersion}")));
        }
        catch (Exception ex)
        {
            Player.SendErrorMessage(GetString("无法获取更新:") + ex.Message);
            return;
        }
    }

   
}