using Terraria;
using TShockAPI;

namespace BossLock;

public static class Utils
{
    public static string TimeFormat(string time)
    {
        var inputDate = DateTime.ParseExact(time, "yyyy-MM-dd-HH:mm:ss", null);
        var today = DateTime.Today;
        var inputDateWithoutTime = inputDate.Date;
    
        var daysDifference = (inputDateWithoutTime - today).Days;

        if (daysDifference > 365)
        {
            return GetString("已锁定");
        }

        // 获取本周的开始日期(周一)
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        if (today.DayOfWeek == DayOfWeek.Sunday)
        {
            startOfWeek = today.AddDays(-6);
        }

        // 获取下周的开始日期
        var startOfNextWeek = startOfWeek.AddDays(7);
        
        switch (daysDifference)
        {
            // 今天/明天/后天
            case 0:
                return inputDate.ToString(GetString("今天HH:mm"));
            case 1:
                return inputDate.ToString(GetString("明天HH:mm"));
            case 2:
                return inputDate.ToString(GetString("后天HH:mm"));
            // 昨天/前天
            case -1:
                return inputDate.ToString(GetString("昨天HH:mm"));
            case -2:
                return inputDate.ToString(GetString("前天HH:mm"));
            // 本周内(周一到周日)
            case >= 0 when inputDateWithoutTime < startOfNextWeek:
                return inputDate.ToString(GetString($"周{ConvertToChineseWeekDay(inputDate.DayOfWeek)}HH:mm"));
        }

        // 下周内
        if (inputDateWithoutTime >= startOfNextWeek && inputDateWithoutTime < startOfNextWeek.AddDays(7))
        {
            return inputDate.ToString(GetString($"下周{ConvertToChineseWeekDay(inputDate.DayOfWeek)}HH:mm"));
        }

        // 其他情况
        return inputDate.ToString(GetString("M月d日HH:mm"));
    }

// 辅助方法：将DayOfWeek转换为中文
    private static string ConvertToChineseWeekDay(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => GetString("日"),
            DayOfWeek.Monday => GetString("一"),
            DayOfWeek.Tuesday => GetString("二"),
            DayOfWeek.Wednesday => GetString("三"),
            DayOfWeek.Thursday => GetString("四"),
            DayOfWeek.Friday => GetString("五"),
            DayOfWeek.Saturday => GetString("六"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public static bool IsPlayerValid(TSPlayer? plr)
    {
        return plr is { Active: true, ConnectionAlive: true };
    }

    public static string GetCurrentProgressStatus()
    {
        var bossStatus = new Dictionary<Func<bool>, string>
        {
            { () => !NPC.downedSlimeKing, GetString("史王") },
            { () => !NPC.downedBoss1, GetString("克眼") },
            { () => !NPC.downedBoss2, Main.drunkWorld ? GetString("世吞/克脑") : WorldGen.crimson ? GetString("克脑") : GetString("世吞") },
            { () => !NPC.downedBoss3, GetString("骷髅王") },
            { () => !Main.hardMode, GetString("血肉墙") },
            { () => !NPC.downedMechBoss2 || !NPC.downedMechBoss1 || !NPC.downedMechBoss3, Main.zenithWorld ? GetString("美杜莎") : GetString("新三王") },
            { () => !NPC.downedPlantBoss, GetString("世花") },
            { () => !NPC.downedGolemBoss, GetString("石巨人") },
            { () => !NPC.downedAncientCultist, GetString("拜月教徒") },
            { () => !NPC.downedTowers, GetString("四柱") },
            { () => !NPC.downedMoonlord, GetString("月总") }
        };
        
        var firstUnbeatenBoss = bossStatus.FirstOrDefault(pair => pair.Key());
    
        return firstUnbeatenBoss.Value == null ? GetString("已毕业") : GetString($"{firstUnbeatenBoss.Value}前");
    }
}