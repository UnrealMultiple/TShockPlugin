using Economics.Core.Enumerates;

namespace Economics.Core.EventArgs.PlayerEventArgs;

public class PlayerCurrencyUpdateArgs : BasePlayerEventArgs
{
    public long Current { get; init; }

    public CurrencyUpdateType CurrentType { get; init; }

    public long Change { get; init; }

    public string CurrencyType { get; init; } = string.Empty;
}