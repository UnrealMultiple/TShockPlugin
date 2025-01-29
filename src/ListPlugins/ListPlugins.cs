using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

[ApiVersion(2, 1)]
public class ListPlugins : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 8);
    public override string Author => "iheart 修改：羽学，肝帝熙恩";
    public override string Description => GetString("用指令查已装插件");

    public ListPlugins(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("ListPlugin", this.ListPluginsCommand, "插件列表", "pllist"));
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.ListPluginsCommand);
        }
        base.Dispose(disposing);
    }

    private void ListPluginsCommand(CommandArgs args)
    {
        try
        {
            var pluginInfos = ServerApi.Plugins.Select(p => new
            {
                p.Plugin.Name,
                p.Plugin.Author,
                p.Plugin.Version,
                p.Plugin.Description
            });

            if (!pluginInfos.Any())
            {
                args.Player.SendInfoMessage(GetString("没有安装任何插件。"));
                return;
            }

            var msgBuilder = new StringBuilder();
            msgBuilder.AppendLine(GetString("插件列表："));
            foreach (var plugin in pluginInfos)
            {
                msgBuilder.AppendLine(this.FormatPluginInfo(plugin));
            }

            args.Player.SendInfoMessage(msgBuilder.ToString());
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            args.Player.SendErrorMessage(GetString("获取插件列表时发生错误。"));
        }
    }

    private string FormatPluginInfo(dynamic plugin)
    {
        var random = new Random();
        var colorTag = $"[c/{random.Next(0, 16777216):X}:";
        string formattedName = colorTag + plugin.Name.Replace("]", "]" + colorTag + "]") + "]";
        return GetString($"{formattedName} - 版本: {plugin.Version} - 作者: {plugin.Author}") +
               (plugin.Description != null
                   ? GetString($", 描述: {plugin.Description}")
                   : "");
    }
}