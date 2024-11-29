using LazyAPI;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;
using System.IO;

namespace AutoBroadcast;

[Config]
public class ABConfig : JsonConfigBase<ABConfig>
{
    [LocalizedPropertyName(CultureType.Chinese, "广播列表")]
    [LocalizedPropertyName(CultureType.English, "broadcasts")]
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
                Messages = new string[] { "/time 4:30", "设置时间为4:30" },
                ColorRGB = new float[] { 255, 234, 115 },
                Interval = 600,
            }
        };
    }
}

public class Broadcast
{
    [LocalizedPropertyName(CultureType.Chinese, "广播名称")]
    [LocalizedPropertyName(CultureType.English, "name")]
    public string Name { get; set; } = string.Empty;

    [LocalizedPropertyName(CultureType.Chinese, "启用")]
    [LocalizedPropertyName(CultureType.English, "enable")]
    public bool Enabled { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "广播消息")]
    [LocalizedPropertyName(CultureType.English, "msg")]
    public string[] Messages { get; set; } = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "RGB颜色")]
    [LocalizedPropertyName(CultureType.English, "color")]
    public float[] ColorRGB { get; set; } = new float[3];

    [LocalizedPropertyName(CultureType.Chinese, "时间间隔")]
    [LocalizedPropertyName(CultureType.English, "interval")]
    public int Interval { get; set; } = 0;

    [LocalizedPropertyName(CultureType.Chinese, "延迟执行")]
    [LocalizedPropertyName(CultureType.English, "delay")]
    public int StartDelay { get; set; } = 0;

    [LocalizedPropertyName(CultureType.Chinese, "广播组")]
    [LocalizedPropertyName(CultureType.English, "groups")]
    public string[] Groups { get; set; } = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "触发词语")]
    [LocalizedPropertyName(CultureType.English, "triggerWords")]
    public string[] TriggerWords { get; set; } = Array.Empty<string>();

    [LocalizedPropertyName(CultureType.Chinese, "触发整个组")]
    [LocalizedPropertyName(CultureType.English, "triggerToWholeGroup")]
    public bool TriggerToWholeGroup { get; set; } = false;
}