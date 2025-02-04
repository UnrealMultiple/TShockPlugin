

namespace Economics.Skill.Internal;
internal class JobjManager
{
    internal static PriorityQueue<Action<object>, long> scheduled = new();

    private static readonly Dictionary<Action<object>, object> Args = new();

    public static long Timer { get; internal set; }

    public static void Schedule(int interval, Action<object> action)
    {

        void Wrapper(object args)
        {
            action(args);
            Delayed(interval, Wrapper, args);
        }

        Delayed(interval, Wrapper, Args[action]);
    }

    public static void Delayed(int delay, Action<object> action, object obj)
    {
        Args[action] = obj;
        scheduled.Enqueue(action, delay + Timer);
    }

    internal static void FrameUpdate()
    {
        ++Timer;
        while (scheduled.TryPeek(out var action, out var time))
        {
            if (time > Timer)
            {
                break;
            }
            action(Args[action]);
            scheduled.Dequeue();
        }
    }

}