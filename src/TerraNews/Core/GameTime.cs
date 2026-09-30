using System;

namespace TerraNews.Core;

/// <summary>
/// Pure helpers for Terraria's game clock. Kept free of engine state so the arithmetic
/// can be unit tested without a running server.
///
/// Terraria time facts (Terraria 1.4.5):
///   * Main.dayLength  = 54000 ticks == 15 in-game hours (04:30 -> 19:30)
///   * Main.nightLength = 32400 ticks ==  9 in-game hours (19:30 -> 04:30)
///   * Main.time restarts at 0 at 04:30 and again at 19:30, and Main.dayTime flips there.
///   * Main.mfwh_UpdateTime_StartDay calls Main.mfwh_AnglerQuestSwap() right before time = 0,
///     so 04:30 is exactly when the day's quest fish is rolled.
///   * 1 in-game hour = 3600 ticks, so 1 in-game minute = 60 ticks = 1 real second.
/// </summary>
public static class GameTime
{
    public const double TicksPerGameMinute = 60.0;
    public const double DawnMinutes = 270.0;      // 04:30
    public const double DuskMinutes = 1170.0;      // 19:30
    public const double NightLengthTicks = 32400.0;
    public const double DayLengthTicks = 54000.0;
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
    /// True only while the trigger moment is inside its own open window.
    ///
    /// The window is deliberately narrow. A dedicated server only ticks the world while a
    /// client is connected, so a server that was empty at 04:30 has no 04:30 to report -
    /// and it must not announce a stale task in the afternoon instead. The width is given
    /// in real seconds, and Terraria's clock runs at 60x, so 30 s is 30 in-game minutes:
    /// a window of 30 covers 04:30 - 05:00.
    /// </summary>
    public static bool IsInWindow(int hour, int minute, double time, bool dayTime, double windowTicks)
    {
        if (IsDaytimeRequest(hour, minute) != dayTime)
            return false;

        double offset = TriggerOffsetTicks(hour, minute);
        return time >= offset && time < offset + windowTicks;
    }

    /// <summary>True when the requested clock time has already been reached in the current half day.</summary>
    public static bool HasPassed(int hour, int minute, double time, bool dayTime)
    {
        if (IsDaytimeRequest(hour, minute) != dayTime)
            return false;
        return time >= TriggerOffsetTicks(hour, minute);
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
