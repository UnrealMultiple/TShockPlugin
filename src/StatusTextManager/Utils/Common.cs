using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;

namespace StatusTextManager.Utils;

internal static class Common
{
    public static ulong TickCount;
    public static IEnumerable<TSPlayer> PlayersOnline => from p in TShock.Players where p is { Active: true } select p;

    public static StringBuilder AcquirePlayerStringBuilder(this StringBuilder?[] sbs, TSPlayer player)
    {
        var id = player.Index;
        return (
            sbs[id] ??
            (sbs[id] = new StringBuilder())
        ).Clear();
    }

    public static void CountTick()
    {
        TickCount++;
    }

    public static void ClearTickCount()
    {
        TickCount = 0;
    }

    public static string GetCurrentTime()
    {
        var num = Main.time / 3600.0;
        num += 4.5;
        if (!Main.dayTime)
        {
            num += 15.0;
        }
        num %= 24.0;
        return string.Format("{0}:{1:D2}", (int) Math.Floor(num), (int) Math.Floor(num % 1.0 * 60.0));
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

    public static string GetAnglerQuestFishName()
    {
        var itemID = Main.anglerQuestItemNetIDs[Main.anglerQuest];
        return (string) Lang.GetItemName(itemID);

    }
    public static int GetAnglerQuestFishId()
    {
        var itemID = Main.anglerQuestItemNetIDs[Main.anglerQuest];
        return itemID;
    }

    private static Regex fishMissionPlaceRegex = new (@"(?<=（抓捕位置：|\(Capturado no |\(Поймано в |\(można złapać w |\(Se trouve |\(Se encuentra en |\(Caught ).*?(?=）|\))");
    private static Regex fishMissionPlaceExceptionalCasesRegex = new (@"(?<=（|\().*?(?=）|\))");
    
    public static string GetAnglerQuestFishingBiome()
    {
        var itemId = Main.anglerQuestItemNetIDs[Main.anglerQuest];
        var questText = Language.GetTextValue($"AnglerQuestText.Quest_{ItemID.Search.GetName(itemId)}");
        return Language.ActiveCulture.Name switch
        {
            "en-US" or "fr-FR" or "es-ES" or "ru-RU" or "zh-Hans" or "pt-BR" or "pl-PL" =>
                fishMissionPlaceRegex.Match(questText).ToString(),
            _ =>
                fishMissionPlaceExceptionalCasesRegex.Match(questText).ToString()
        };
    }
}