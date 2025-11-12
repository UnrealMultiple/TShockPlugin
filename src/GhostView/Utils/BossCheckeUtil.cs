using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace GhostView.Utils;

public static class BossCheckUtil
{
    public static bool IsBossNearPlayer(TSPlayer? player, float distance = 250f)
    {
        if (player?.TPlayer is null)
        {
            return false;
        }

        var distanceInPixels = distance * 16f;

        return Main.npc.Any(npc =>
            npc.active &&
            (npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail) &&
            Vector2.Distance(player.TPlayer.position, npc.position) <= distanceInPixels
        );
    }
}