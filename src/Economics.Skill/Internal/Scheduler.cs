namespace Economics.Skill.Internal;

internal class Scheduler
{
    public int NextRun
    {
        get => this._nextRun;
        private set
        {
            this._nextRun = value;
            this.Running = ScheduleState.Running;
        }
    }

    private int _nextRun = 0;

    private Action _action { get; }

    public int Count { get; private set; }

    public bool Autoreset { get; private set; }

    public ScheduleState Running { get; private set; } = ScheduleState.Wait;

    public Guid Guid { get; private set; }

    public Scheduler(Action action, bool autoreset)
    {
        this._action = action;
        this.Autoreset = autoreset;
        this.Guid = Guid.NewGuid();
    }

    public Scheduler Update()
    {
        this.Count += 1;
        return this;
    }

    public Scheduler CanRun()
    {
        if (this.Count > this.NextRun && this.Running == ScheduleState.Running)
        {
            this._action();
            this.Count = 0;
            if (!this.Autoreset)
            {
                this.Running = ScheduleState.Success;
            }
        }
        return this;
    }

    public Scheduler AddMilliSeconds(int millisecond)
    {
        this.NextRun = Convert.ToInt32(Math.Ceiling(millisecond / 1000f * 60));
        return this;
    }

    public Scheduler AddSeconds(int seconds)
    {
        return this.AddMilliSeconds(seconds * 1000);
    }

    public Scheduler AddMinute(int minutes)
    {
        return this.AddSeconds(minutes * 60);
    }

    public Scheduler AddTimeSpan(TimeSpan ts)
    {
        return this.AddMilliSeconds((int) ts.TotalMilliseconds);
    }
}