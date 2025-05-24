using TShockAPI;

namespace Economics.Core.Model;

public class PingData
{
    public DateTime Start = DateTime.Now;

    public DateTime End = DateTime.Now;

    public TSPlayer TSPlayer = null!;

    public Action<PingData> action = null!;

    public double GetPing()
    {
        return (this.End - this.Start).TotalMilliseconds;
    }
}