using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using TShockAPI;

namespace Economics.Skill.Model.Loop;
public class BaseLoop
{
    [JsonProperty("运行累计触发", Order = -4)]
    public uint Interval { get; set; } = 1;

    [JsonProperty("运行区间始", Order = -3)]
    public uint Mark { get; set; } = 1;

    [JsonProperty("运行区间终", Order = -2)]
    public int EndMark { get; set; } = 1;

    [JsonProperty("广播", Order = -1)]
    public string Broadcast { get; set; } = string.Empty;

    public void SendBroadcast()
    {
        if (!string.IsNullOrEmpty(this.Broadcast))
        {
            TShock.Utils.Broadcast(this.Broadcast, Color.Wheat);
        }
    }
}
