namespace TerraNews.Core;

/// <summary>
/// "At most once per half day, and only inside the trigger window", as a testable state
/// machine. Terraria restarts Main.time at 04:30 and 19:30; counting those restarts gives a
/// monotonic half-day id, which survives lag, /time set jumps and restarts.
/// </summary>
public sealed class HalfDayTrigger
{
    private double _lastTime = -1;

    /// <summary>Increments every time Main.time restarts.</summary>
    public int HalfDayIndex { get; private set; }

    /// <summary>Half-day id for which a broadcast was already sent; -1 means none yet.</summary>
    public int AnnouncedHalfDay { get; private set; } = -1;

    /// <summary>Advances the half-day counter for one server tick. Returns true to broadcast.</summary>
    public bool Tick(double time, bool dayTime, int hour, int minute, double windowTicks)
    {
        if (_lastTime >= 0 && time < _lastTime)
            HalfDayIndex++;
        _lastTime = time;

        return ShouldAnnounce(time, dayTime, hour, minute, windowTicks);
    }

    /// <summary>Records the broadcast as already sent for the current half day.</summary>
    public void MarkAnnounced()
    {
        AnnouncedHalfDay = HalfDayIndex;
    }

    /// <summary>True when the trigger is inside its window and this half day has not fired yet.</summary>
    public bool ShouldAnnounce(double time, bool dayTime, int hour, int minute, double windowTicks) =>
        AnnouncedHalfDay != HalfDayIndex
        && GameTime.IsInWindow(hour, minute, time, dayTime, windowTicks);
}
