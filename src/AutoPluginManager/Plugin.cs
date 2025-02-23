using AutoPluginManager.Internal;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AutoPluginManager;

[ApiVersion(2, 1)]
// ReSharper disable once UnusedType.Global
public class Plugin : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 3, 5);

    public override string Author => "少司命,Cai,LaoSparrow";

    public override string Description => GetString("自动更新你的插件！");

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Config.Read();
        Commands.ChatCommands.Add(new("AutoUpdatePlugin", CommandAdapter.Adapter, "apm"));
        PluginManagementContext.Instance.OnPluginUpdate += this.AutoCheckUpdate;
        GeneralHooks.ReloadEvent += this.GeneralHooksOnReloadEvent;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == CommandAdapter.Adapter);
            PluginManagementContext.Instance.OnPluginUpdate -= this.AutoCheckUpdate;
            GeneralHooks.ReloadEvent -= this.GeneralHooksOnReloadEvent;

        }
        base.Dispose(disposing);
    }

    private void GeneralHooksOnReloadEvent(ReloadEventArgs e)
    {
        Config.Read();
        e.Player.SendSuccessMessage(GetString("[AutoUpdatePlugin]插件配置已重载~"));
    }


    private void AutoCheckUpdate(PluginUpdateInfo[] availableUpdates)
    {
        try
        {
            if (!availableUpdates.Any())
            {
                return;
            }

            TShock.Log.ConsoleInfo(GetString("[以下插件有新的版本更新]\n" + string.Join("\n", availableUpdates.Select(i => $"[{i.Current!.Name}] V{i.Current!.Version} >>> V{i.Latest.Version}"))));
            if (Config.Instance.AutoUpdate)
            {
                TShock.Log.ConsoleInfo(GetString("正在自动更新插件..."));
                CommandAdapter.UpdatePlugin(TSPlayer.Server, Array.Empty<string>());
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
    }
}