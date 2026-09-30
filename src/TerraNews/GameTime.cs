namespace TerraNews;

// 游戏时钟换算。Main.time 在 04:30 与 19:30 各归零一次，1 游戏分钟 = 60 刻，
// 时钟以 60 倍速运行 —— 所以天亮那一帧正是渔夫换新任务的那一帧。
public static class GameTime
{
    public const double TicksPerGameMinute = 60.0;
    public const double DawnMinutes = 270.0;      // 04:30
    public const double DuskMinutes = 1170.0;     // 19:30
    public const double NightLengthTicks = 32400.0;
    public const double MinutesPerCycle = 1440.0;

    // 触发时刻相对半日归零点的刻数偏移；04:30 之前的时间归入它前方的那个夜晚。
    public static double TriggerOffsetTicks(int hour, int minute)
    {
        double minutes = hour * 60 + minute;
        if (minutes >= DawnMinutes)
            return (minutes - DawnMinutes) * TicksPerGameMinute;
        return NightLengthTicks - (DawnMinutes - minutes) * TicksPerGameMinute;
    }

    // 只在触发点落入自己那扇窄窗内才为真。窗口刻意很窄：专用服务器只在有客户端
    // 连接时才推进世界，所以 04:30 空服的服务器根本没有 04:30 可报，更不该在下午
    // 拿一条过期的任务凑数。宽度单位是游戏分钟，30 即覆盖 04:30-05:00。
    public static bool IsInWindow(int hour, int minute, double time, bool dayTime, double windowTicks)
    {
        bool requestIsDaytime = hour * 60 + minute >= DawnMinutes;
        if (requestIsDaytime != dayTime)
            return false;

        double offset = TriggerOffsetTicks(hour, minute);
        return time >= offset && time < offset + windowTicks;
    }

    // 把当前游戏时刻格式化成 HH:mm。
    public static string Format(double time, bool dayTime)
    {
        double minutes = (dayTime ? DawnMinutes : DuskMinutes) + time / TicksPerGameMinute;
        minutes %= MinutesPerCycle;
        if (minutes < 0)
            minutes += MinutesPerCycle;

        return $"{(int)minutes / 60:00}:{(int)minutes % 60:00}";
    }

    // 把刻数格式化成 "8 小时 20 分"。
    public static string FormatDuration(double ticks)
    {
        int minutes = (int)(ticks / TicksPerGameMinute);
        int h = minutes / 60, m = minutes % 60;
        return h > 0 ? $"{h} 小时 {m} 分" : $"{m} 分钟";
    }
}

// 每个半日最多播一次，且只在触发窗内。靠数 Main.time 归零的次数得到单调递增的
// 半日编号，这样卡顿、/time set 跳变和重启都不会让它重复播报。
public sealed class HalfDayTrigger
{
    private double _lastTime = -1;

    public int HalfDayIndex { get; private set; }

    // 已经播报过的半日编号，-1 表示还没有播过。
    public int AnnouncedHalfDay { get; private set; } = -1;

    // 推进一个服务器刻；返回 true 表示这一刻该播报了。
    public bool Tick(double time, bool dayTime, int hour, int minute, double windowTicks)
    {
        if (_lastTime >= 0 && time < _lastTime)
            HalfDayIndex++;

        _lastTime = time;
        return AnnouncedHalfDay != HalfDayIndex
               && GameTime.IsInWindow(hour, minute, time, dayTime, windowTicks);
    }

    // 记下当前半日已播报。
    public void MarkAnnounced() => AnnouncedHalfDay = HalfDayIndex;
}
