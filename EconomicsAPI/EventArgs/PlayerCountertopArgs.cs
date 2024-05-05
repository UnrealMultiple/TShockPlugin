

using EconomicsAPI.Model;

namespace EconomicsAPI.EventArgs;

public class PlayerCountertopArgs: System.EventArgs
{
    public List<CountertopMessage> Messages { get; init; } = new();

    public PingData Ping { get; init; } = new();

    public bool Handler { get; set; } = false;
}
