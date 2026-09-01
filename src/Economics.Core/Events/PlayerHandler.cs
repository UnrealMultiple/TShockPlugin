using Economics.Core.EventArgs;
using Economics.Core.EventArgs.PlayerEventArgs;

namespace Economics.Core.Events;

public class PlayerHandler
{
    public delegate void EventCallBack<in TArgs>(TArgs args) where TArgs : BaseEventArgs;

    public static event EventCallBack<PlayerKillNpcArgs>? OnPlayerKillNpc;

    internal static bool PlayerKillNpc(PlayerKillNpcArgs args)
    {
        if (OnPlayerKillNpc != null)
        {
            OnPlayerKillNpc(args);
            return args.Handler;
        }

        return false;
    }
}