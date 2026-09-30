namespace TerraNews.Core;

/// <summary>
/// Encapsulates the "fire at most once per half day, and only inside the trigger window"
/// state machine so the rule can be unit tested without a running server.
///
/// Terraria restarts <c>Main.time</c> at 0 twice a day - at 04:30 (Main.dayTime flips to
/// true) and at 19:30 (flips to false). Counting those restarts gives a monotonic half-day
/// id, which is what "already announced" is tracked against. That survives server lag,
/// /time set jumps and restarts, none of which a naive "Main.time == X" check would.
/// </summary>
public sealed class HalfDayTrigger
{
    private double _lastTime = -1;

    /// <summary>Increments every time Main.time restarts.</summary>
    public int HalfDayIndex { get; private set; }

    /// <summary>Half-day id for which a broadcast was already sent; -1 means none yet.</summary>
    public int AnnouncedHalfDay { get; private set; } = -1;

    public double LastTime => _lastTime;

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

    /// <summary>
    /// True when the trigger moment is inside its window and nothing was announced for the
    /// current half day yet. Anything later in the day - 16:30 included - is not a trigger.
    /// </summary>
    public bool ShouldAnnounce(double time, bool dayTime, int hour, int minute, double windowTicks) =>
        AnnouncedHalfDay != HalfDayIndex
        && GameTime.IsInWindow(hour, minute, time, dayTime, windowTicks);
}
