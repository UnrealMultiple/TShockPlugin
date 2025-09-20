using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace GhostView.Utils;

public static class BossCheckUtil
{
    private static readonly int[] BossIds =
    [
        NPCID.KingSlime,
        NPCID.EyeofCthulhu,
        NPCID.EaterofWorldsHead,
        NPCID.BrainofCthulhu,
        NPCID.QueenBee,
        NPCID.Deerclops,
        NPCID.SkeletronHead,
        NPCID.WallofFlesh,
        NPCID.QueenSlimeBoss,
        NPCID.Retinazer,
        NPCID.Spazmatism,
        NPCID.TheDestroyer,
        NPCID.SkeletronPrime,
        NPCID.Plantera,
        NPCID.Golem,
        NPCID.DukeFishron,
        NPCID.HallowBoss,
        NPCID.CultistBoss,
        NPCID.MoonLordCore
    ];


    public static bool IsBossNearPlayer(TSPlayer? player, float distance = 250f)
    {
        if (player?.TPlayer is null)
        {
            return false;
        }

        var distanceInPixels = distance * 16f;

        return Main.npc.Any(npc =>
            npc.active &&
            BossIds.Contains(npc.type) &&
            Vector2.Distance(player.TPlayer.position, npc.position) <= distanceInPixels
        );
    }
}