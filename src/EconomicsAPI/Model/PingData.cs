using TShockAPI;

namespace EconomicsAPI.Model;

public class PingData
{
    public DateTime Start = DateTime.Now;

    public DateTime End = DateTime.Now;

    public TSPlayer TSPlayer;

    public Action<PingData> action;

    public double GetPing()
    {
        return (End - Start).TotalMilliseconds;
    }
}