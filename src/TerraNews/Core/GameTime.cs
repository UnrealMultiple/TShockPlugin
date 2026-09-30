using System;

namespace TerraNews.Core;

/// <summary>
/// Pure helpers for Terraria's game clock, free of engine state so the arithmetic is testable.
/// 1.4.5 facts: day = 54000 ticks (04:30-19:30), night = 32400 ticks, Main.time restarts at 0
/// at both boundaries, 1 in-game minute = 60 ticks = 1 real second, and the Angler rolls the
/// day's quest fish on the very frame Main.time hits 0 at dawn.
/// </summary>
public static class GameTime
{
    public const double TicksPerGameMinute = 60.0;
    public const double DawnMinutes = 270.0;      // 04:30
    public const double DuskMinutes = 1170.0;      // 19:30
    public const double NightLengthTicks = 32400.0;
    public const double MinutesPerCycle = 1440.0;

    /// <summary>Total in-game minutes of the requested wall clock time.</summary>
    public static int RequestedMinutes(int hour, int minute) => hour * 60 + minute;

    /// <summary>True when the requested time belongs to the daytime half of the cycle.</summary>
    public static bool IsDaytimeRequest(int hour, int minute) => RequestedMinutes(hour, minute) >= DawnMinutes;

    /// <summary>
    /// Offset in ticks from the half-day reset (04:30 or 19:30) at which the trigger fires.
    /// Times before 04:30 map into the night that precedes that dawn.
    /// </summary>
    public static double TriggerOffsetTicks(int hour, int minute)
    {
        double minutes = RequestedMinutes(hour, minute);
        if (minutes >= DawnMinutes)
            return (minutes - DawnMinutes) * TicksPerGameMinute;
        return NightLengthTicks - (DawnMinutes - minutes) * TicksPerGameMinute;
    }

    /// <summary>
    /// The window is deliberately narrow: a dedicated server only ticks the world while a
    /// client is connected, so a server empty at 04:30 has no 04:30 to report and must not
    /// announce a stale task in the afternoon instead. Width is real seconds at 60x, so 30 is
    /// 30 in-game minutes, i.e. 04:30-05:00.
    /// </summary>
    public static bool IsInWindow(int hour, int minute, double time, bool dayTime, double windowTicks)
    {
        if (IsDaytimeRequest(hour, minute) != dayTime)
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

        int h = (int)minutes / 60;
        int m = (int)minutes % 60;
        return $"{h:00}:{m:00}";
    }

    /// <summary>Formats a tick count as a rough duration, e.g. 3.5 小时.</summary>
    public static string FormatDuration(double ticks)
    {
        if (ticks <= 0)
            return "0 分钟";

        double totalMinutes = ticks / TicksPerGameMinute;
        int hours = (int)(totalMinutes / 60);
        int minutes = (int)(totalMinutes % 60);

        return hours > 0 ? $"{hours} 小时 {minutes} 分" : $"{minutes} 分钟";
    }
}
