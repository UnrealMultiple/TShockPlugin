using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace EconomicsAPI.Extensions;

public static class TSPlayerExt
{
    public static void SendCombatMsg(this TSPlayer player, string text, Color color)
    {
        player.TPlayer.SendCombatMsg(text, color);
    }

    public static void ExecCommand(this TSPlayer player, string cmd)
    {
        player.tempGroup = new SuperAdminGroup();
        Commands.HandleCommand(player, cmd.SFormat(player.Name));
        player.tempGroup = null;
    }

    public static void ExecCommand(this TSPlayer player, IEnumerable<string> cmds)
    {
        foreach (var cmd in cmds)
        {
            player.ExecCommand(cmd);
        }
    }

    public static void GiveItems(this TSPlayer player, IEnumerable<Model.Item> items)
    {
        foreach (var item in items)
        {
            player.GiveItem(item.netID, item.Stack, item.Prefix);
        }
    }

    public static List<TSPlayer> GetPlayerInRange(this TSPlayer Player, int range)
    {
        return Player.TPlayer.GetPlayerInRange(range).Select(x => TShock.Players[x.whoAmI]).ToList();
    }

    public static List<Projectile> GetProjectileInRange(this TSPlayer Player, int range)
    {
        return Player.TPlayer.GetProjectInRange(range);
    }

    /// <summary>
    /// 清理弹幕
    /// </summary>
    /// <param name="Player">玩家对象</param>
    /// <param name="Range">范围</param>
    public static void ClearProj(this TSPlayer Player, int Range)
    {
        Player.GetProjectileInRange(Range).ForEach(x =>
        {
            x.active = false;
            x.type = 0;
            TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", x.whoAmI);
        });
    }

    /// <summary>
    /// 回复玩家生命值
    /// </summary>
    /// <param name="Player">玩家对象</param>
    /// <param name="life">生命值</param>
    public static void ReLife(this TSPlayer Player, int life)
    {
        if (life > 0)
            NetMessage.SendData((int)PacketTypes.PlayerHealOther, -1, -1, NetworkText.Empty, Player.Index, life);
    }


    public static void AllReLife(this TSPlayer Player, int Range, int life)
    {
        if (life > 0)
            Player.GetPlayerInRange(Range).ForEach(x => x.ReLife(life));
    }

    public static List<NPC> GetNpcInRange(this TSPlayer Player, int range)
    {
        return Player.TPlayer.GetNpcInRange(range);
    }
    /// <summary>
    /// 范围执行命令
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="Range"></param>
    /// <param name="cmds"></param>
    public static void ExecRangeCommands(this TSPlayer Player, int Range, HashSet<string> cmds)
    {
        if (cmds.Count == 0 || Range <= 0)
            return;
        Player.GetPlayerInRange(Range).ForEach(x => x.ExecCommand(cmds));
    }


    /// <summary>
    /// 对范围内NPC赵成伤害
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="damage"></param>
    /// <param name="Range"></param>
    public static void StrikeNpc(this TSPlayer Player, int damage, int Range)
    {
        if (damage > 0)
        {
            Player.GetNpcInRange(Range).ForEach(npc =>
            {
                npc.StrikeNPC(damage, 0, 0);
                NetMessage.SendData(28, -1, -1, NetworkText.Empty, npc.whoAmI, damage, 0, 0, Player.Index);
            });
        }
    }

    /// <summary>
    /// 重生
    /// </summary>
    /// <param name="Player">玩家对象</param>
    public static void ReSpawn(this TSPlayer Player)
    {
        Player.Spawn(PlayerSpawnContext.ReviveFromDeath);
    }


    /// <summary>
    /// 将敌怪拉到身边
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="Range"></param>
    /// <param name="code"></param>
    public static void CollectNPC(this TSPlayer Player, int Range)
    {
        Player.GetNpcInRange(Range).ForEach(npc =>
        {
            npc.Teleport(Player.TPlayer.position);
        });
    }
}
