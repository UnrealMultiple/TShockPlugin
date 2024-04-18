using Newtonsoft.Json;

namespace AutoBroadcast
{
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
                ABConfig.WriteExample(file);
            }
            return JsonConvert.DeserializeObject<ABConfig>(File.ReadAllText(file));
        }

        public static void WriteExample(string file)
        {
            File.WriteAllText(file, @"{
			  ""广播列表"": [
				{
				  ""广播名称"": ""E实例广播"",
				  ""启用"": false,
				  ""广播消息"": [
					""这是一条广播"",
					""每五分钟执行一次"",
					""可以执行命令"",
					""/time noon""
				  ],
				  ""RGB颜色"": [
					255.0,
					0.0,
					0.0
				  ],
				  ""时间间隔"": 300,
				  ""延迟执行"": 60,
				  ""广播组"": [],
				  ""触发词语"": [],
				  ""触发整个组"": false
				}
			  ]
			}");
        }
    }

    public class Broadcast
    {
        [JsonProperty("广播名称")]
        public string Name = string.Empty;

        [JsonProperty("开启")]
        public bool Enabled = false;

        [JsonProperty("广播消息")]
        public string[] Messages = new string[0];

        [JsonProperty("RGB颜色")]
        public float[] ColorRGB = new float[3];

        [JsonProperty("时间间隔")]
        public int Interval = 0;

        [JsonProperty("延迟执行")]
        public int StartDelay = 0;

        [JsonProperty("广播组")]
        public string[] Groups { get; set; } = new string[0];

        [JsonProperty("触发词语")]
        public string[] TriggerWords { get; set; } = new string[0];

        [JsonProperty("触发整个组")]
        public bool TriggerToWholeGroup { get; set; } = false;
    }
}