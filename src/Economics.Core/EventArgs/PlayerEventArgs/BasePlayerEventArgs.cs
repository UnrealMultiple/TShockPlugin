using TShockAPI;

namespace Economics.Core.EventArgs.PlayerEventArgs;

public class BasePlayerEventArgs : BaseEventArgs
{
    public TSPlayer? Player { get; init; }
}