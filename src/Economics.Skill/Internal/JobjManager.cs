

namespace Economics.Skill.Internal;
internal class JobjManager
{
    private readonly static Dictionary<Guid, Scheduler> _schedulers = new();

    internal static void FrameUpdate()
    {
        foreach (var guid in _schedulers.Keys.ToList())
        {
            var scheduler = _schedulers[guid];
            scheduler.Update().CanRun();
            if (scheduler.Running == ScheduleState.Success)
            {
                _schedulers.Remove(guid);
            }
        }
    }

    public static Scheduler Add(Action action, bool autoRest = false)
    {
        var sch = new Scheduler(action, autoRest);
        _schedulers.Add(sch.Guid, sch);
        return sch;
    }

    public static Scheduler Add(Action action, TimeSpan ts, bool autoRest = false)
    {
        return Add(action, autoRest).AddTimeSpan(ts);
    }

    public static bool Remove(Guid guid)
    {
        return _schedulers.Remove(guid);
    }
}
