using TShockAPI;

namespace EconomicsAPI.EventArgs.PlayerEventArgs;

public class BasePlayerEventArgs : BaseEventArgs
{
    public TSPlayer Player { get; init; }
}