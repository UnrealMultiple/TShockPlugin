using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace EconomicsAPI.Extensions;

public static class PlayerExt
{
    public static void SendCombatMsg(this Player player, string text, Color color)
    {
        NetMessage.SendData(119, -1, -1, NetworkText.FromLiteral(text), (int)color.packedValue, player.position.X, player.position.Y);
    }

    public static List<Projectile> GetProjectInRange(this Player Player, int range)
    {
        return Main.projectile.Where(p =>
        {
            float dx = p.position.X - Player.position.X;
            float dy = p.position.Y - Player.position.Y;
            return p != null && (dx * dx) + (dy * dy) <= range * range * 256f && p.active && !p.friendly;
        }).ToList();
    }

    public static List<NPC> GetNpcInRange(this Player Player, int range)
    {
        return Main.npc.Where(p =>
        {
            float dx = p.position.X - Player.position.X;
            float dy = p.position.Y - Player.position.Y;
            return p != null && p.active && !p.townNPC && p.netID != NPCID.TargetDummy && (dx * dx) + (dy * dy) <= range * range * 256f;
        }).ToList();
    }

    public static List<Player> GetPlayerInRange(this Player player, int range)
    {
        return Economics.ServerPlayers
            .Select(ply => ply.TPlayer)
            .Where(ply =>
            {
                float dx = ply.position.X - player.position.X;
                float dy = ply.position.Y - player.position.Y;
                return (dx * dx) + (dy * dy) <= range * range * 256f;
            }).ToList();
    }
}
