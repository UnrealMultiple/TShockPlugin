using Economics.Core.EventArgs;
using Economics.Core.EventArgs.PlayerEventArgs;

namespace Economics.Core.Events;

public class PlayerHandler
{
    public delegate void EventCallBack<in TArgs>(TArgs args) where TArgs : BaseEventArgs;

    public static event EventCallBack<PlayerKillNpcArgs>? OnPlayerKillNpc;

    public static event EventCallBack<PlayerCountertopArgs>? OnPlayerCountertop;

    public static event EventCallBack<PlayerCurrencyUpdateArgs>? OnPlayerCurrencyUpdate;

    internal static bool PlayerCurrencyUpdate(PlayerCurrencyUpdateArgs args)
    {
        if (OnPlayerCurrencyUpdate != null)
        {
            OnPlayerCurrencyUpdate(args);
            return args.Handler;
        }
        return false;
    }

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