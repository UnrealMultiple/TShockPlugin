using Terraria;
using TShockAPI;

namespace GhostView.Utils;

public static class BuffUtils
{
    public static void ClearDebuffs(TSPlayer? player)
    {
        if (player?.TPlayer is null || !player.Active)
        {
            return;
        }

        var tPlayer = player.TPlayer;

        for (var i = 0; i < Player.maxBuffs; i++)
        {
            var buffType = tPlayer.buffType[i];

            if (buffType > 0 && Main.debuff[buffType])
            {
                tPlayer.ClearBuff(buffType);
            }
        }
    }
}