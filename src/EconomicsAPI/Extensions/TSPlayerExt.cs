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

    public static void SendGradientMsg(this TSPlayer player, string text)
    {
        player.SendInfoMessage(Utils.Helper.GetGradientText(text));
    }

    public static void ExecCommand(this TSPlayer player, string cmd)
    {
        player.tempGroup = new SuperAdminGroup();
        try
        {
            Commands.HandleCommand(player, cmd.SFormat(player.Name));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleDebug(GetString($"EconomicAPI 执行命令报错:{ex.Message}"));
        }
        finally
        { 
            player.tempGroup = null;
        }
        
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
    public static void HealLife(this TSPlayer Player, int life)
    {
        if (life > 0)
        {
            NetMessage.SendData((int) PacketTypes.PlayerHealOther, -1, -1, NetworkText.Empty, Player.Index, life);
        }
    }

    public static void HealMana(this TSPlayer player, int mana)
    {
        player.TPlayer.statMana += mana;
        player.SendData(PacketTypes.PlayerMana, null, player.Index);
        NetMessage.TrySendData(43, -1, player.Index, null, player.Index, mana);
    }

    public static void HealAllLife(this TSPlayer Player, int Range, int life)
    {
        Player.GetPlayerInRange(Range).ForEach(x => x.HealLife(life));
    }

    public static void HealAllMana(this TSPlayer Player, int Range, int mana)
    {
        Player.GetPlayerInRange(Range).ForEach(x => x.HealMana(mana));
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
    public static void ExecRangeCommands(this TSPlayer Player, int Range, IEnumerable<string> cmds)
    {
        if (!cmds.Any() || Range <= 0)
        {
            return;
        }

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

    public static void StrikeNpc(this TSPlayer Player, int damage, int Range, HashSet<int> Strike)
    {
        if (damage > 0)
        {
            Player.GetNpcInRange(Range).ForEach(npc =>
            {
                if (!Strike.Contains(npc.netID))
                {
                    npc.StrikeNPC(damage, 0, 0);
                    NetMessage.SendData(28, -1, -1, NetworkText.Empty, npc.whoAmI, damage, 0, 0, Player.Index);
                }
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
    public static void CollectNPC(this TSPlayer Player, int Range, HashSet<int> notNpc, int x = 0, int y = 0)
    {
        Player.GetNpcInRange(Range).ForEach(npc =>
        {
            if (!notNpc.Contains(npc.type))
            {
                npc.Teleport(Player.TPlayer.position + new Vector2(x * Player.TPlayer.direction, y));
            }
        });
    }
}