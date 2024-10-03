using Newtonsoft.Json;
using System.IO;

namespace AutoBroadcast;

public class ABConfig
{
    [JsonProperty("广播列表")]
    public Broadcast[] Broadcasts { get; set; } = new Broadcast[0];

    public ABConfig Write(string file)
    {
        File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        return this;
    }

    public static ABConfig Read(string file)
    {
        if (!File.Exists(file))
        {
            WriteExample(file);
        }
        return JsonConvert.DeserializeObject<ABConfig>(File.ReadAllText(file)) ?? new ABConfig();
    }

    public static void WriteExample(string file)
    {
        var exampleConfig = new ABConfig
        {
            Broadcasts = new[]
            {
                new Broadcast
                {
                    Name = "示例广播",
                    Enabled = true,
                    Messages = new string[] { "/time 4:30", "设置时间为4:30" },
                    ColorRGB = new float[] { 255, 234, 115 },
                    Interval = 600,
                }
            }
        };

        exampleConfig.Write(file);
    }
}

public class Broadcast
{
    [JsonProperty("广播名称")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("启用")]
    public bool Enabled { get; set; } = false;

    [JsonProperty("广播消息")]
    public string[] Messages { get; set; } = new string[0];

    [JsonProperty("RGB颜色")]
    public float[] ColorRGB { get; set; } = new float[3];

    [JsonProperty("时间间隔")]
    public int Interval { get; set; } = 0;

    [JsonProperty("延迟执行")]
    public int StartDelay { get; set; } = 0;

    [JsonProperty("广播组")]
    public string[] Groups { get; set; } = new string[0];

    [JsonProperty("触发词语")]
    public string[] TriggerWords { get; set; } = new string[0];

    [JsonProperty("触发整个组")]
    public bool TriggerToWholeGroup { get; set; } = false;
}