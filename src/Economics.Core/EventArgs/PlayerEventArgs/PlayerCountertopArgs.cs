using Economics.Core.Model;

namespace Economics.Core.EventArgs.PlayerEventArgs;

public class PlayerCountertopArgs : BasePlayerEventArgs
{
    public List<CountertopMessage> Messages { get; init; } = new();

    public PingData Ping { get; init; } = new();
}