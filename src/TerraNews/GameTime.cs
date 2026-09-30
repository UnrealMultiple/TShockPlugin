namespace TerraNews;

/// <summary>
/// Game clock helpers. Main.time restarts at 0 at 04:30 and again at 19:30, 1 in-game minute
/// is 60 ticks, and the clock runs at 60x - so the Angler rolls the day's quest fish on the
/// very frame the day half begins.
/// </summary>
public static class GameTime
{
    public const double TicksPerGameMinute = 60.0;
    public const double DawnMinutes = 270.0;      // 04:30
    public const double DuskMinutes = 1170.0;     // 19:30
    public const double NightLengthTicks = 32400.0;
    public const double MinutesPerCycle = 1440.0;

    /// <summary>
    /// Offset in ticks from the half-day reset (04:30 or 19:30) at which the trigger fires.
    /// Times before 04:30 map into the night that precedes that dawn.
    /// </summary>
    public static double TriggerOffsetTicks(int hour, int minute)
    {
        double minutes = hour * 60 + minute;
        if (minutes >= DawnMinutes)
            return (minutes - DawnMinutes) * TicksPerGameMinute;
        return NightLengthTicks - (DawnMinutes - minutes) * TicksPerGameMinute;
    }

    /// <summary>
    /// True only while the trigger moment is inside its own open window. The window is
    /// deliberately narrow: a dedicated server only ticks the world while a client is
    /// connected, so a server empty at 04:30 has no 04:30 to report and must not announce a
    /// stale task in the afternoon instead. The width is in game minutes, so 30 covers
    /// 04:30-05:00.
    /// </summary>
    public static bool IsInWindow(int hour, int minute, double time, bool dayTime, double windowTicks)
    {
        bool requestIsDaytime = hour * 60 + minute >= DawnMinutes;
        if (requestIsDaytime != dayTime)
            return false;

        double offset = TriggerOffsetTicks(hour, minute);
        return time >= offset && time < offset + windowTicks;
    }

    /// <summary>Formats the current game clock as HH:mm.</summary>
    public static string Format(double time, bool dayTime)
    {
        double minutes = (dayTime ? DawnMinutes : DuskMinutes) + time / TicksPerGameMinute;
        minutes %= MinutesPerCycle;
        if (minutes < 0)
            minutes += MinutesPerCycle;

        return $"{(int)minutes / 60:00}:{(int)minutes % 60:00}";
    }

    /// <summary>Formats a tick span as "8 小时 20 分".</summary>
    public static string FormatDuration(double ticks)
    {
        int minutes = (int)(ticks / TicksPerGameMinute);
        int h = minutes / 60, m = minutes % 60;
        return h > 0 ? $"{h} 小时 {m} 分" : $"{m} 分钟";
    }
}

/// <summary>
/// Fires at most once per half day, and only inside the trigger window. Counting Main.time
/// restarts gives a monotonic half-day id, which survives lag, /time set jumps and restarts.
/// </summary>
public sealed class HalfDayTrigger
{
    private double _lastTime = -1;

    public int HalfDayIndex { get; private set; }

    /// <summary>Half-day id for which a broadcast was already sent; -1 means none yet.</summary>
    public int AnnouncedHalfDay { get; private set; } = -1;

    /// <summary>Advances the counter for one server tick; true on the tick to broadcast.</summary>
    public bool Tick(double time, bool dayTime, int hour, int minute, double windowTicks)
    {
        if (_lastTime >= 0 && time < _lastTime)
            HalfDayIndex++;

        _lastTime = time;
        return AnnouncedHalfDay != HalfDayIndex
               && GameTime.IsInWindow(hour, minute, time, dayTime, windowTicks);
    }

    /// <summary>Records the broadcast as already sent for the current half day.</summary>
    public void MarkAnnounced() => AnnouncedHalfDay = HalfDayIndex;
}
