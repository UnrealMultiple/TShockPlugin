using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoBroadcast;

[Config]
public class ABConfig : JsonConfigBase<ABConfig>
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
                Enabled = true,
                Messages = new string[] { "/say Ciallo～(∠・ω< )⌒★", "自动广播执行了服务器指令/say Ciallo～(∠・ω< )⌒★" },
                ColorRGB = new float[] { 255, 234, 115 },
                Interval = 600,
            }
        };
    }
}

public class Broadcast
{
    [LocalizedPropertyName(CultureType.Chinese, "广播名称")]
    [LocalizedPropertyName(CultureType.English, "Name")]
    public string Name { get; set; } = string.Empty;

    [LocalizedPropertyName(CultureType.Chinese, "启用")]
    [LocalizedPropertyName(CultureType.English, "Enable")]
    public bool Enabled { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "广播消息")]
    [LocalizedPropertyName(CultureType.English, "Msg")]
    public string[] Messages { get; set; } = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "RGB颜色")]
    [LocalizedPropertyName(CultureType.English, "Color")]
    public float[] ColorRGB { get; set; } = new float[3];

    [LocalizedPropertyName(CultureType.Chinese, "时间间隔")]
    [LocalizedPropertyName(CultureType.English, "Interval")]
    public int Interval { get; set; } = 0;

    [LocalizedPropertyName(CultureType.Chinese, "延迟执行")]
    [LocalizedPropertyName(CultureType.English, "Delay")]
    public int StartDelay { get; set; } = 0;

    [LocalizedPropertyName(CultureType.Chinese, "广播组")]
    [LocalizedPropertyName(CultureType.English, "Groups")]
    public string[] Groups { get; set; } = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "触发词语")]
    [LocalizedPropertyName(CultureType.English, "TriggerWords")]
    public string[] TriggerWords { get; set; } = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "触发整个组")]
    [LocalizedPropertyName(CultureType.English, "TriggerToWholeGroup")]
    public bool TriggerToWholeGroup { get; set; } = false;
}