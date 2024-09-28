

namespace Economics.Skill.Internal;
internal class JobjManager
{
    private readonly static List<Scheduler> _schedulers = new();

    internal static void FrameUpdate()
    {
        for (var i = _schedulers.Count - 1; i >= 0; i--)
        {
            var scheduler = _schedulers[i];
            scheduler.Update().CanRun();
            if (scheduler.Running == ScheduleState.Success)
            {
                _schedulers.RemoveAt(i);
            }
        }
    }

    public static Scheduler Add(Action action, bool autoRest = false)
    {
        var sch = new Scheduler(action, autoRest);
        _schedulers.Add(sch);
        return sch;
    }

    public static Scheduler Add(Action action, TimeSpan ts, bool autoRest = false)
    {
        return Add(action, autoRest).AddTimeSpan(ts);
    }

    public static int RemoveAll(Guid guid)
    {
        return _schedulers.RemoveAll(s => s.Guid == guid);
    }

    public static bool Remove(Scheduler scheduler)
    {
        return _schedulers.Remove(scheduler);
    }
}