using LazyAPI.Enums;
using LazyAPI.Utility;
using Microsoft.Xna.Framework;
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

    public static bool ProgressComplete(this TSPlayer Player, IEnumerable<string> names)
    {
        foreach (var name in names)
        {
            var orParts = name.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var orResult = false;
            foreach (var part in orParts)
            {
                var trimmed = part.Trim();
                var negate = trimmed.StartsWith('!');
                var key = negate ? trimmed[1..] : trimmed;

                if (GameProgress.DefaultProgressNames.TryGetValue(key, out var pm))
                {
                    var status = pm.GetStatus(Player.TPlayer);
                    orResult |= negate ? !status : status;
                }
            }
            if (!orResult)
            {
                return false;
            }
        }
        return true;
    }


    public static bool ProgressComplete(this TSPlayer Player, IEnumerable<ProgressType> progresses)
    {
        foreach (var progressGroup in progresses)
        {
            var matched = false;
            foreach (var flag in Enum.GetValues<ProgressType>())
            {
                if (progressGroup.HasFlag(flag))
                {
                    if (GameProgress.DefaultProgressTypes.TryGetValue(flag, out var pm))
                    {
                        if (pm.GetStatus(Player.TPlayer))
                        {
                            matched = true;
                            break;
                        }
                    }
                }
            }
            if (!matched)
            {
                return false;
            }
        }
        return true;
    }
}
