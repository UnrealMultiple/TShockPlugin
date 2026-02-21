using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace Economics.Core.Extensions;

public static class PlayerExtension
{
    public static void SendCombatMsg(this Player player, string text, Color color)
    {
        NetMessage.SendData(119, -1, -1, NetworkText.FromLiteral(text), (int) color.packedValue, player.position.X, player.position.Y);
    }

    public static List<Projectile> GetProjectInRange(this Player Player, int range)
    {
        return Player.position.FindRangeProjectiles(range);
    }

    public static double ItemUseAngle(this Player TPlayer)
    {
        double angle = TPlayer.itemRotation;
        if (TPlayer.direction == -1)
        {
            angle += Math.PI;
        }
        return angle;
    }

    public static Vector2 ItemOffSet(this Player player)
    {
        float length = player.HeldItem.type == 0 ? 10 : player.HeldItem.height;
        var offset = new Vector2(length, 0).RotatedBy(player.ItemUseAngle());
        return offset;
    }

    public static List<NPC> GetNpcInRange(this Player Player, int range)
    {
        return Player.position.FindRangeNPCs(range);
    }

    public static NPC? GetNpcInRangeByHp(this Player Player, int range)
    {
        var npcs = Player.GetNpcInRange(range);
        if (npcs.Count == 0)
        {
            return null;
        }

        var boss = npcs.OrderBy(x => x.life);
        return boss.FirstOrDefault(x => x.boss, boss.First());
    }

    public static NPC? GetNpcInRangeByDis(this Player Player, int range)
    {
        var npcs = Player.GetNpcInRange(range);
        if (npcs.Count == 0)
        {
            return null;
        }

        var boss = npcs.OrderBy(x => Math.Abs(x.position.Distance(Player.position)));
        return boss.FirstOrDefault(x => x.boss, boss.First());
    }

    public static List<Player> GetPlayerInRange(this Player player, int range)
    {
        return player.position.FindRangePlayers(range);
    }
}