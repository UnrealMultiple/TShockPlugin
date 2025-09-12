using TerrariaApi.Server;


namespace CaiBotLite.Services;

public static class ProgressControlSupport
{
    public static bool Support { get; private set; }

    public static void Init()
    {
        var pluginContainer = ServerApi.Plugins.FirstOrDefault(x => x.Plugin.Name == "ProgressControls");
        if (pluginContainer is not null)
        {
            Support = true;
        }
        
    }

    public static Dictionary<string, string> GetLockBosses()
    {
        if (!Support)
        {
            throw new NotSupportedException("没有安装ProgressControls插件!");
        }

        var enable = ProgressControl.PControl.config.OpenAutoControlProgressLock;
        var lockedBosses = ProgressControl.PControl.config.ProgressLockTimeForStartServerDate;
        var initDate = ProgressControl.PControl.config.StartServerDate;
        var result = new Dictionary<string, string>();

        if (!enable)
        {
            return result;
        }
        
        var bossIdNameToIdentity = new Dictionary<string, string>
        {
            { "史莱姆王", "King Slime" },
            { "克苏鲁之眼", "Eye of Cthulhu" },
            { "世界吞噬者", "Eater of Worlds" },
            { "克苏鲁之脑", "Brain of Cthulhu" },
            { "蜂后", "Queen Bee" },
            { "巨鹿", "Deerclops" },
            { "骷髅王", "Skeletron" },
            { "血肉墙", "Wall of Flesh" },
            { "史莱姆皇后", "Queen Slime" },
            { "双子魔眼", "The Twins" },
            { "毁灭者", "The Destroyer" },
            { "机械骷髅王", "Skeletron Prime" },
            { "世纪之花", "Plantera" },
            { "石巨人", "Golem" },
            { "猪龙鱼公爵", "Duke Fishron" },
            { "光之女皇", "Empress of Light" },
            { "拜月教教徒", "Lunatic Cultist" },
            { "月亮领主", "Moon Lord" }
        };
        
        
        foreach (var lockedBoss in lockedBosses)
        {
            if (!bossIdNameToIdentity.TryGetValue(lockedBoss.Key, out var bossName))
            {
                continue;
            }
            
            
            

            result[bossName] = TimeFormat(initDate + TimeSpan.FromHours(lockedBoss.Value));
        }

        return result;
    }
    
    public static string TimeFormat(DateTime dateTime)
    {
        var today = DateTime.Today;
        var inputDateWithoutTime = dateTime.Date;
    
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
                return dateTime.ToString(GetString("今天HH:mm"));
            case 1:
                return dateTime.ToString(GetString("明天HH:mm"));
            case 2:
                return dateTime.ToString(GetString("后天HH:mm"));
            // 昨天/前天
            case -1:
                return dateTime.ToString(GetString("昨天HH:mm"));
            case -2:
                return dateTime.ToString(GetString("前天HH:mm"));
            // 本周内(周一到周日)
            case >= 0 when inputDateWithoutTime < startOfNextWeek:
                return dateTime.ToString(GetString($"周{ConvertToChineseWeekDay(dateTime.DayOfWeek)}HH:mm"));
        }

        // 下周内
        if (inputDateWithoutTime >= startOfNextWeek && inputDateWithoutTime < startOfNextWeek.AddDays(7))
        {
            return dateTime.ToString(GetString($"下周{ConvertToChineseWeekDay(dateTime.DayOfWeek)}HH:mm"));
        }

        // 其他情况
        return dateTime.ToString(GetString("M月d日HH:mm"));
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
    
}