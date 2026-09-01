using Microsoft.Xna.Framework;
using System.Text;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace Economics.Core.Extensions;

public static class TSPlayerExtension
{

    public static void SendCombatMsg(this TSPlayer player, string text, Color color)
    {
        player.TPlayer.SendCombatMsg(text, color);
    }

    public static void SendGradientMsg(this TSPlayer player, string text)
    {
        player.SendInfoMessage(Utils.Helper.GetGradientText(text));
    }

    public static string GetFormattedBiomesList(this TSPlayer plr)
    {
        StringBuilder stringBuilder = new();
        var envInfo = plr.GetBiomesInfo();
        var colorHexCode = envInfo.Contains(GetString("空岛")) ? "00BFFF"
            : envInfo.Contains(GetString("地下")) ? "FF8C00"
            : envInfo.Contains(GetString("洞穴")) ? "A0522D"
            : envInfo.Contains(GetString("地狱")) ? "FF0000"
            : "008000";
        stringBuilder.Append($"[c/{colorHexCode}:{string.Join(',', envInfo)}]");
        return stringBuilder.ToString();
    }
    public static List<string> GetBiomesInfo(this TSPlayer plr)
    {
        var index = plr.Index;
        var list = new List<string>();
        if (Main.player[index].ZoneDungeon)
        {
            list.Add(GetString("地牢"));
        }
        if (Main.player[index].ZoneCorrupt)
        {
            list.Add(GetString("腐化"));
        }
        if (Main.player[index].ZoneHallow)
        {
            list.Add(GetString("神圣"));
        }
        if (Main.player[index].ZoneMeteor)
        {
            list.Add(GetString("陨石"));
        }
        if (Main.player[index].ZoneJungle)
        {
            list.Add(GetString("丛林"));
        }
        if (Main.player[index].ZoneSnow)
        {
            list.Add(GetString("雪原"));
        }
        if (Main.player[index].ZoneCrimson)
        {
            list.Add(GetString("猩红"));
        }
        if (Main.player[index].ZoneWaterCandle)
        {
            list.Add(GetString("水蜡烛"));
        }
        if (Main.player[index].ZonePeaceCandle)
        {
            list.Add(GetString("和平蜡烛"));
        }
        if (Main.player[index].ZoneDesert)
        {
            list.Add(GetString("沙漠"));
        }
        if (Main.player[index].ZoneGlowshroom)
        {
            list.Add(GetString("发光蘑菇"));
        }
        if (Main.player[index].ZoneUndergroundDesert)
        {
            list.Add(GetString("地下沙漠"));
        }
        if (Main.player[index].ZoneSkyHeight)
        {
            list.Add(GetString("空岛"));
        }
        if (Main.player[index].ZoneDirtLayerHeight)
        {
            list.Add(GetString("地下"));
        }
        if (Main.player[index].ZoneRockLayerHeight)
        {
            list.Add(GetString("洞穴"));
        }
        if (Main.player[index].ZoneUnderworldHeight)
        {
            list.Add(GetString("地狱"));
        }
        if (Main.player[index].ZoneBeach)
        {
            list.Add(GetString("海滩"));
        }
        if (Main.player[index].ZoneRain)
        {
            list.Add(GetString("雨天"));
        }
        if (Main.player[index].ZoneSandstorm)
        {
            list.Add(GetString("沙尘暴"));
        }
        if (Main.player[index].ZoneGranite)
        {
            list.Add(GetString("花岗岩"));
        }
        if (Main.player[index].ZoneMarble)
        {
            list.Add(GetString("大理石"));
        }
        if (Main.player[index].ZoneHive)
        {
            list.Add(GetString("蜂巢"));
        }
        if (Main.player[index].ZoneGemCave)
        {
            list.Add(GetString("宝石洞窟"));
        }
        if (Main.player[index].ZoneLihzhardTemple)
        {
            list.Add(GetString("神庙"));
        }
        if (Main.player[index].ZoneGraveyard)
        {
            list.Add(GetString("墓地"));
        }
        if (Main.player[index].ZoneShadowCandle)
        {
            list.Add(GetString("阴影蜡烛"));
        }
        if (Main.player[index].ZoneShimmer)
        {
            list.Add(GetString("微光"));
        }
        if (Main.player[index].ShoppingZone_Forest)
        {
            list.Add(GetString("森林"));
        }
        return list;
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