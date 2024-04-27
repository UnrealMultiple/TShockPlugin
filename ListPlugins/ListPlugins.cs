using Microsoft.Xna.Framework;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

[ApiVersion(2, 1)]
public class ListPlugins : TerrariaPlugin
{
    public override string Name => "查已装插件";
    public override Version Version => new Version(1, 0, 3);
    public override string Author => "iheart 修改：羽学，肝帝熙恩";
    public override string Description => "用指令查已装插件";

    public ListPlugins(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("ListPlugin", ListPluginsCommand, "插件列表", "pllist"));
    }

    private void ListPluginsCommand(CommandArgs args)
    {
        Random random = new Random();
        var pluginInfos = ServerApi.Plugins.Select(p => new
        {
            Name = p.Plugin.Name,
            Author = p.Plugin.Author,
            Description = p.Plugin.Description
        });

        StringBuilder msgBuilder = new StringBuilder();
        msgBuilder.AppendLine("插件列表：");
        foreach (var plugin in pluginInfos)
        {
            string colorTag = $"[c/{random.Next(0, 16777216):X}:";
            string formattedName = colorTag + plugin.Name.Replace("]", "]" + colorTag + "]") + "]";
            msgBuilder.AppendFormat("{0} - 作者: {1}{2}\n", formattedName, plugin.Author,
                plugin.Description != null ? $", 描述: {plugin.Description}" : "");
        }

        args.Player.SendInfoMessage(msgBuilder.ToString());
    }
}
