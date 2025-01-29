using LazyAPI;
using Microsoft.Xna.Framework;
using On.OTAPI;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace AnnouncementBoxPlus;

[ApiVersion(2, 1)]
// ReSharper disable once UnusedType.Global
public class AnnouncementBoxPlus : LazyPlugin
{
    //定义插件的作者名称
    public override string Author => "Cai";

    //插件的一句话描述
    public override string Description => GetString("优化广播盒");

    //插件的名称
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;

    //插件的版本
    public override Version Version => new Version(1, 0, 5);

    //插件的构造器
    public AnnouncementBoxPlus(Main game) : base(game)
    {
    }

    //插件加载时执行的代码
    public override void Initialize()
    {
        On.OTAPI.Hooks.Wiring.InvokeAnnouncementBox += this.OnAnnouncementBox;
        GetDataHandlers.SignRead.Register(this.OnSignRead);
        GetDataHandlers.Sign.Register(this.OnSign);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.SignRead.UnRegister(this.OnSignRead);
            GetDataHandlers.Sign.UnRegister(this.OnSign);
            On.OTAPI.Hooks.Wiring.InvokeAnnouncementBox -= this.OnAnnouncementBox;
        }

        base.Dispose(disposing);
    }

    private void OnSign(object? sender, GetDataHandlers.SignEventArgs e)
    {
        var tile = Main.tile[e.X, e.Y];
        if (tile.type == 425 && Config.Instance.usePerm)
        {
            if (!e.Player.HasPermission("AnnouncementBoxPlus.Edit"))
            {
                e.Player.SendErrorMessage(GetString("[i:3617]你没有权限修改广播盒(AnnouncementBoxPlus.Edit)"));
                e.Handled = true;
            }
        }
    }

    private void OnSignRead(object? sender, GetDataHandlers.SignReadEventArgs e)
    {
        var tile = Main.tile[e.X, e.Y];
        if (tile.type == 425 && Config.Instance.usePerm)
        {
            if (!e.Player.HasPermission("AnnouncementBoxPlus.Edit"))
            {
                e.Player.SendErrorMessage(GetString("[i:3617]你没有权限修改广播盒(AnnouncementBoxPlus.Edit)"));
                e.Handled = true;
            }
        }
    }

    private bool OnAnnouncementBox(Hooks.Wiring.orig_InvokeAnnouncementBox orig, int x, int y, int signId)
    {
        try
        {
            if (!Config.Instance.Enable)
            {
                return false;
            }

            var num37 = Sign.ReadSign(x, y, CreateIfMissing: false);
            if (num37 == -1 || Main.sign[num37] == null || string.IsNullOrWhiteSpace(Main.sign[num37].text))
            {
                return false;
            }

            var text = Main.sign[num37].text;


            if (Config.Instance.justWho && Wiring.CurrentUser != 255 && TShock.Players[Wiring.CurrentUser] != null)
            {
                if (Config.Instance.range <= 0)
                {
                    NetMessage.SendData(107, Wiring.CurrentUser, -1, NetworkText.FromLiteral(FormatBox(Main.sign[num37].text, Wiring.CurrentUser)), 255, Color.White.R, Color.White.G, Color.White.B, 460);
                }
                else
                {
                    if (Main.player[Wiring.CurrentUser].active && Main.player[Wiring.CurrentUser].Distance(new Vector2((x * 16) + 16, (y * 16) + 16)) <= Config.Instance.range)
                    {
                        NetMessage.SendData(107, Wiring.CurrentUser, -1, NetworkText.FromLiteral(FormatBox(Main.sign[num37].text, Wiring.CurrentUser)), 255, Color.White.R, Color.White.G, Color.White.B, 460);
                    }
                }
            }
            else
            {
                if (Config.Instance.range <= 0)
                {
                    for (var i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active)
                        {
                            NetMessage.SendData(107, i, -1, NetworkText.FromLiteral(FormatBox(Main.sign[num37].text, i)), 255, Color.White.R, Color.White.G, Color.White.B, 460);
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active && Main.player[i].Distance(new Vector2((x * 16) + 16, (y * 16) + 16)) <= Config.Instance.range)
                        {
                            NetMessage.SendData(107, i, -1, NetworkText.FromLiteral(FormatBox(Main.sign[num37].text, i)), 255, Color.White.R, Color.White.G, Color.White.B, 460);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(ex.ToString());
        }

        return false;
    }

    public static string FormatBox(string text, int index)
    {
        var online = "";
        if (TShock.Utils.GetActivePlayerCount() == 0)
        {
            online = GetString("服务器中没有玩家在线");
        }

        var players = new List<string>();

        foreach (var ply in TShock.Players)
        {
            if (ply != null && ply.Active)
            {
                players.Add(ply.Name);
            }
        }

        online = string.Join(",", players);
        if (index >= Main.maxPlayers || TShock.Players[index] == null)
        {
            if (Config.Instance.useFormat)
            {
                text = Config.Instance.formation
                    .Replace("%玩家组名%", "")
                    .Replace("%玩家名%", GetString("[服务器]"))
                    .Replace("%当前时间%", DateTime.Now.ToString("HH:mm"))
                    .Replace("%内容%", text);
            }

            if (Config.Instance.usePlaceholder)
            {
                text = text.Replace("%玩家组名%", "")
                    .Replace("%玩家名%", GetString("[服务器]"))
                    .Replace("%当前时间%", DateTime.Now.ToString("HH:mm"))
                    .Replace("%当前服务器在线人数%", TShock.Utils.GetActivePlayerCount().ToString())
                    .Replace("%渔夫任务鱼名称%", Tools.GetFishMissionName())
                    .Replace("%渔夫任务鱼ID%", Tools.GetFishId().ToString())
                    .Replace("%渔夫任务鱼地点%", Tools.GetFisheMissionPlace())
                    .Replace("%地图名称%", Main.worldName)
                    .Replace("%玩家血量%", GetString("无法获取"))
                    .Replace("%玩家魔力%", GetString("无法获取"))
                    .Replace("%玩家血量最大值%", GetString("无法获取"))
                    .Replace("%玩家魔力最大值%", GetString("无法获取"))
                    .Replace("%玩家幸运值%", GetString("无法获取"))
                    .Replace("%玩家X坐标%", GetString("无法获取"))
                    .Replace("%玩家Y坐标%", GetString("无法获取"))
                    .Replace("%玩家所处区域%", GetString("无法获取"))
                    .Replace("%玩家死亡状态%", GetString("无法获取"))
                    .Replace("%重生倒计时%", GetString("无法获取"))
                    .Replace("%当前环境%", GetString("无法获取"))
                    .Replace("%服务器在线列表%", online)
                    .Replace("%渔夫任务鱼完成%", GetString("未完成"));
            }

            return text;
        }

        var plr = TShock.Players[index];
        if (Config.Instance.useFormat)
        {
            text = Config.Instance.formation
                .Replace("%玩家组名%", plr.Group.Name)
                .Replace("%玩家名%", plr.Name)
                .Replace("%当前时间%", DateTime.Now.ToString("HH:mm"))
                .Replace("%内容%", text);
        }

        if (Config.Instance.usePlaceholder)
        {
            text = text.Replace("%玩家组名%", plr.Group.Name)
                .Replace("%玩家名%", plr.Name)
                .Replace("%当前时间%", DateTime.Now.ToString("HH:mm"))
                .Replace("%当前服务器在线人数%", TShock.Utils.GetActivePlayerCount().ToString())
                .Replace("%渔夫任务鱼名称%", Tools.GetFishMissionName())
                .Replace("%渔夫任务鱼ID%", Tools.GetFishId().ToString())
                .Replace("%渔夫任务鱼地点%", Tools.GetFisheMissionPlace())
                .Replace("%地图名称%", Main.worldName)
                .Replace("%玩家血量%", plr.TPlayer.statLife.ToString())
                .Replace("%玩家魔力%", plr.TPlayer.statMana.ToString())
                .Replace("%玩家血量最大值%", plr.TPlayer.statLifeMax2.ToString())
                .Replace("%玩家魔力最大值%", plr.TPlayer.statManaMax2.ToString())
                .Replace("%玩家幸运值%", plr.TPlayer.luck.ToString(CultureInfo.InvariantCulture))
                .Replace("%玩家X坐标%", plr.TileX.ToString())
                .Replace("%玩家Y坐标%", plr.TileY.ToString())
                .Replace("%玩家所处区域%", plr.CurrentRegion == null
                    ? GetString("空区域")
                    : plr.CurrentRegion.Name)
                .Replace("%玩家死亡状态%", plr.Dead
                    ? GetString("已死亡")
                    : GetString("存活"))
                .Replace("%重生倒计时%", plr.RespawnTimer == 0
                    ? GetString("未死亡")
                    : "%plr.RespawnTimer%")
                .Replace("%当前环境%", plr.GetEnvString())
                .Replace("%服务器在线列表%", online)
                .Replace("%渔夫任务鱼完成%", Main.anglerWhoFinishedToday.Exists((string x) => x == plr.Name)
                    ? GetString("已完成")
                    : GetString("未完成"));
        }

        return text;
    }
}

public static class Tools
{
    public static string GetFishMissionName()
    {
        var itemId = Main.anglerQuestItemNetIDs[Main.anglerQuest];
        return Lang.GetItemName(itemId).ToString();
    }

    public static int GetFishId()
    {
        return Main.anglerQuestItemNetIDs[Main.anglerQuest];
    }

    private static readonly Regex FishMissionPlaceRegex = new (@"(?<=（抓捕位置：|\(Capturado no |\(Поймано в |\(można złapać w |\(Se trouve |\(Se encuentra en |\(Caught ).*?(?=）|\))");
    private static readonly Regex FishMissionPlaceExceptionalCasesRegex = new (@"(?<=（|\().*?(?=）|\))");

    public static string GetFisheMissionPlace()
    {
        var itemId = Main.anglerQuestItemNetIDs[Main.anglerQuest];
        var questText = Language.GetTextValue($"AnglerQuestText.Quest_{ItemID.Search.GetName(itemId)}");
        return Language.ActiveCulture.Name switch
        {
            "en-US" or "fr-FR" or "es-ES" or "ru-RU" or "zh-Hans" or "pt-BR" or "pl-PL" =>
                FishMissionPlaceRegex.Match(questText).ToString(),
            _ =>
                FishMissionPlaceExceptionalCasesRegex.Match(questText).ToString()
        };
    }

    public static bool CheckNPCActive(string npcId)
    {
        if (!int.TryParse(npcId, out var result))
        {
            return false;
        }

        for (var i = 0; i < Main.npc.Length; i++)
        {
            if (Main.npc[i] != null && Main.npc[i].active && Main.npc[i].netID == result)
            {
                return true;
            }
        }

        return false;
    }

    public static string GetEnvString(this TSPlayer plr)
    {
        StringBuilder stringBuilder = new ();
        var envInfo = plr.GetEnvInfo();
        var colorHexCode = envInfo.Contains(GetString("空岛")) ? "00BFFF"
            : envInfo.Contains(GetString("地下")) ? "FF8C00"
            : envInfo.Contains(GetString("洞穴")) ? "A0522D"
            : envInfo.Contains(GetString("地狱")) ? "FF0000"
            : "008000";
        stringBuilder.Append($"[c/{colorHexCode}:{string.Join(',', envInfo)}]");
        return stringBuilder.ToString();
    }

    public static List<string> GetEnvInfo(this TSPlayer plr)
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
}