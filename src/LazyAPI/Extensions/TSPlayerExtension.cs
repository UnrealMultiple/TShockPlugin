using LazyAPI.Attributes;
using LazyAPI.Enums;
using LazyAPI.Utility;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TShockAPI;

namespace LazyAPI.Extensions;
public static class TSPlayerExtension
{
    private static readonly string combatTextFormat = new string('\n', 20) + "{0}" + new string('\n', 20);
    public static void SendCombatText(this TSPlayer player, string message, Color color, bool visableToOthers = true)
    {
        (visableToOthers ? TSPlayer.All : player).SendData(PacketTypes.CreateCombatTextExtended, message, (int) color.PackedValue, player.X, player.Y);
    }
    public static void SendStatusMessage(this TSPlayer player, string message)
    {
        player.SendData(PacketTypes.Status, string.Format(combatTextFormat, message));
    }

    public static void SendPlayerUpdate(this TSPlayer player)
    {
        player.SendData(PacketTypes.PlayerUpdate, "", player.Index);
    }

    public static void SendPlayerSlot(this TSPlayer player, int slot)
    {
        player.SendData(PacketTypes.PlayerSlot, "", slot);
    }

    public static Dictionary<string, bool> GetProgress(this TSPlayer Player)
    {
        return GameProgress.DefaultProgressNames
            .ToDictionary(p => p.Key, p => p.Value.GetStatus(Player.TPlayer));
    }

    public static bool InProgress(this TSPlayer Player, IEnumerable<string> names)
    {
        var gameProgress = Player.GetProgress();
        foreach (var name in names)
        {
            var anti = false;
            var pn = name;
            if (name.StartsWith('!'))
            {
                anti = true;
                pn = name[1..];
            }
            if (gameProgress.TryGetValue(pn, out var code))
            {
                if (code == anti)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
