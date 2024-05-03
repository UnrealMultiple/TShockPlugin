using Newtonsoft.Json;


public class ABConfig
{
    [JsonProperty("广播列表")]
    public Broadcast[] Broadcasts = new Broadcast[0];

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
        return JsonConvert.DeserializeObject<ABConfig>(File.ReadAllText(file));
    }

    public static void WriteExample(string file)
    {
        Broadcast broadcast = new Broadcast
        {
            Name = "示例广播",
            Enabled = false,
            Messages = new[] { "这是一个广播示例", "每5分钟播出一次", "广播也可以执行命令", "/time noon" },
            ColorRGB = new[] { 255f, 0f, 0f },
            Interval = 300,
            StartDelay = 60
        };

        ABConfig aBConfig = new ABConfig
        {
            Broadcasts = new[] { broadcast }
        };

        aBConfig.Write(file);
    }

    public class Broadcast
    {
        [JsonProperty("名称")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("是否启用")]
        public bool Enabled { get; set; } = false;
        [JsonProperty("消息列表")]
        public string[] Messages { get; set; } = new string[0];
        [JsonProperty("颜色RGB")]
        public float[] ColorRGB { get; set; } = new float[3];
        [JsonProperty("间隔时间")]
        public int Interval { get; set; } = 0;
        [JsonProperty("延迟时间")]
        public int StartDelay { get; set; } = 0;
        [JsonProperty("触发区域")]
        public string[] TriggerRegions { get; set; } = new string[0];
        [JsonProperty("区域触发器")]
        public string RegionTrigger { get; set; } = "none";
        [JsonProperty("组")]
        public string[] Groups { get; set; } = new string[0];
        [JsonProperty("触发词")]
        public string[] TriggerWords { get; set; } = new string[0];
        [JsonProperty("是否触发整个组")]
        public bool TriggerToWholeGroup { get; set; } = false;
    }
}