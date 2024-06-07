using EconomicsAPI.Enumerates;

namespace EconomicsAPI.EventArgs.PlayerEventArgs;

public class PlayerCurrencyUpdateArgs : BasePlayerEventArgs
{
    public long Current { get; init; }

    public CurrencyUpdateType CurrentType { get; init; }

    public long Change { get; init; }
}
