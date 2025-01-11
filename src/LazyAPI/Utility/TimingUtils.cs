namespace LazyAPI.Utility;

public static class TimingUtils
{
    internal static PriorityQueue<Action, long> scheduled = new();
    public static long Timer { get; internal set; }

    public static void Schedule(int interval, Action action)
    {
        void Wrapper()
        {
            action();
            Delayed(interval, Wrapper);
        }

        Delayed(interval, Wrapper);
    }

    public static void Delayed(int delay, Action action)
    {
        scheduled.Enqueue(action, delay + Timer);
    }
}