using EconomicsAPI.Model;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace EconomicsAPI.Extensions;

public static class PlayerExt
{
    public static void SendCombatMsg(this Player player, string text, Color color)
    {
        NetMessage.SendData(119, -1, -1, NetworkText.FromLiteral(text), (int)color.packedValue, player.position.X, player.position.Y);
    }

    public static void SendCombatMsg(this TSPlayer player, string text, Color color)
    { 
        SendCombatMsg(player.TPlayer, text, color);
    }

    public static void ExecCommand(this TSPlayer player, string cmd)
    {
        player.tempGroup = new SuperAdminGroup();
        Commands.HandleCommand(player, cmd);
        player.tempGroup = null;
    }

    public static void ExecCommand(this TSPlayer player, IEnumerable<string> cmds)
    {
        foreach(var cmd in cmds)
        {
            player.ExecCommand(cmd);
        }
    }

    public static void GiveItems(this TSPlayer player, IEnumerable<Model.Item> items)
    {
        foreach(var item in items)
        {
            player.GiveItem(item.netID, item.Stack, item.Prefix);
        }
    }
}
