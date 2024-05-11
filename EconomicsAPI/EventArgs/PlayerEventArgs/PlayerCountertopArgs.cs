using EconomicsAPI.Model;

namespace EconomicsAPI.EventArgs.PlayerEventArgs;

public class PlayerCountertopArgs : BasePlayerEventArgs
{
    public List<CountertopMessage> Messages { get; init; } = new();

    public PingData Ping { get; init; } = new();
}
