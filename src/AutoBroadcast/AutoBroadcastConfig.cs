using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoBroadcast;

[Config]
public class AutoBroadcastConfig : JsonConfigBase<AutoBroadcastConfig>
{
    [LocalizedPropertyName(CultureType.Chinese, "广播列表")]
    [LocalizedPropertyName(CultureType.English, "Broadcasts")]
    public Broadcast[] Broadcasts { get; set; } = Array.Empty<Broadcast>();

    protected override string Filename => "AutoBroadcast";

    protected override void SetDefault()
    {
        this.Broadcasts = new[]
        {
            new Broadcast
            {
                Name = "示例广播",
                Enabled = false,
                Messages = new [] { "/say Ciallo～(∠・ω< )⌒★", "自动广播执行了服务器指令/say Ciallo～(∠・ω< )⌒★" },
                ColorRgb = new [] { 255, 234, 115 },
                Interval = 600,
                StartDelay = 0,
                TriggerToWholeGroup = false
            }
        };
    }
}

