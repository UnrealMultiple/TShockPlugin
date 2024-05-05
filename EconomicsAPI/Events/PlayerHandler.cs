using EconomicsAPI.EventArgs;

namespace EconomicsAPI.Events;

public class PlayerHandler
{
    public delegate void EventCallBack<in TArgs>(TArgs args) where TArgs : System.EventArgs;

    public static event EventCallBack<PlayerKillNpcArgs> OnPlayerKillNpc;

    public static event EventCallBack<PlayerCountertopArgs> OnPlayerCountertop;

    internal static bool PlayerKillNpc(PlayerKillNpcArgs args)
    {
        if (OnPlayerKillNpc != null)
        {
            OnPlayerKillNpc(args);
            return args.Handler;
        }

        return false;
    }

    internal static bool PlayerCountertopUpdate(PlayerCountertopArgs args)
    {
        if (OnPlayerCountertop != null)
        {
            OnPlayerCountertop(args);
            return args.Handler;
        }
        return false;
    }
}
