using Newtonsoft.Json;

namespace AutoBroadcast
{
    public class ABConfig
    {
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
			  ""Broadcasts"": [
				{
				  ""Name"": ""E实例广播"",
				  ""Enabled"": false,
				  ""Messages"": [
					""这是一条广播"",
					""每五分钟执行一次"",
					""可以执行命令"",
					""/time noon""
				  ],
				  ""ColorRGB"": [
					255.0,
					0.0,
					0.0
				  ],
				  ""Interval"": 300,
				  ""StartDelay"": 60,
				  ""Groups"": [],
				  ""TriggerWords"": [],
				  ""TriggerToWholeGroup"": false
				}
			  ]
			}");
        }
    }

    public class Broadcast
    {
        public string Name = string.Empty;
        public bool Enabled = false;
        public string[] Messages = new string[0];
        public float[] ColorRGB = new float[3];
        public int Interval = 0;
        public int StartDelay = 0;
        public string[] Groups = new string[0];
        public string[] TriggerWords = new string[0];
        public bool TriggerToWholeGroup = false;
    }
}