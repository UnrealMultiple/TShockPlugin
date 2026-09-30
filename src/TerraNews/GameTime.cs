namespace TerraNews;

// 游戏时钟换算。Main.time 在 04:30 与 19:30 各归零一次，1 游戏分钟 = 60 刻，
// 时钟以 60 倍速运行 —— 所以天亮那一帧正是渔夫换新任务的那一帧。
public static class GameTime
{
    public const double TicksPerGameMinute = 60.0;
    public const double DawnMinutes = 270.0;      // 04:30
    public const double DuskMinutes = 1170.0;     // 19:30
    public const double NightLengthTicks = 32400.0;    // 19:30 -> 04:30
    public const double DayLengthTicks = 54000.0;     // 04:30 -> 19:30
    public const double MinutesPerCycle = 1440.0;

    // 某个钟点算不算夜晚。白天是 04:30–19:29，其余（19:30–04:29）都算夜晚。
    private static bool IsNightMinutes(double minutes) =>
        minutes < DawnMinutes || minutes >= DuskMinutes;

    // 触发时刻相对它所属那个半日归零点的刻数偏移。
    public static double TriggerOffsetTicks(int hour, int minute)
    {
        double minutes = hour * 60 + minute;

        if (minutes >= DawnMinutes && minutes < DuskMinutes)
            return (minutes - DawnMinutes) * TicksPerGameMinute;          // 白天，从 04:30 起算
        if (minutes >= DuskMinutes)
            return (minutes - DuskMinutes) * TicksPerGameMinute;          // 夜晚开头，从 19:30 起算
        return NightLengthTicks - (DawnMinutes - minutes) * TicksPerGameMinute;  // 00:00–04:29，上一夜的末尾
    }

    // 只在触发点落入自己那扇窄窗内才为真。窗口刻意很窄：专用服务器只在有客户端
    // 连接时才推进世界，所以 04:30 空服的服务器根本没有 04:30 可报，更不该在下午
    // 拿一条过期的任务凑数。宽度单位是游戏分钟，30 即覆盖 04:30-05:00。
    //
    // 触发点所在半日的剩余刻数就是窗口的自然上限：越过半日边界时 Main.time 会归零、
    // dayTime 翻转，同一个偏移量再比就没有意义了（那已是下半天的事）。所以窗口被
    // 截到半日结束，而不是让它跨过去 —— 例如 19:20 触发只剩 10 分钟到日落，就只播
    // 19:20-19:30。这是有意为之：宁可窗口短，也不在半日切换那一帧上误判。
    public static bool IsInWindow(int hour, int minute, double time, bool dayTime, double windowTicks)
    {
        // 触发点必须落在和它同一半日里，否则夜间配置会被拿去白天的 Main.time 上比。
        if (IsNightMinutes(hour * 60 + minute) == dayTime)
            return false;

        double offset = TriggerOffsetTicks(hour, minute);
        double limit = Math.Min(offset + windowTicks, dayTime ? DayLengthTicks : NightLengthTicks);
        return time >= offset && time < limit;
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

// 每个半日最多播一次，且只在触发窗内。半日编号靠数昼夜翻转得到：Main.dayTime 在
// 04:30 和 19:30 各翻转一次，而这两处 Main.time 必定归零。反过来，管理员用 /time
// 在同一半日内把时间调小（比如 10:00 退回 05:00）并不会翻转 dayTime，因此闩锁不会被
// 误重置、不会重播。
public sealed class HalfDayTrigger
{
    private double _lastTime = -1;
    private bool _lastDayTime;

    public int HalfDayIndex { get; private set; }

    // 已经播报过的半日编号，-1 表示还没有播过。
    public int AnnouncedHalfDay { get; private set; } = -1;

    // 推进一个服务器刻；返回 true 表示这一刻该播报了。
    public bool Tick(double time, bool dayTime, int hour, int minute, double windowTicks)
    {
        if (_lastTime >= 0 && dayTime != _lastDayTime)
            HalfDayIndex++;

        _lastTime = time;
        _lastDayTime = dayTime;
        return AnnouncedHalfDay != HalfDayIndex
               && GameTime.IsInWindow(hour, minute, time, dayTime, windowTicks);
    }

    // 记下当前半日已播报。
    public void MarkAnnounced() => AnnouncedHalfDay = HalfDayIndex;
}
