using Newtonsoft.Json;

namespace AutoBroadcast
{
    public class ABConfig
    {
        [JsonProperty("�㲥�б�")]
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
			  ""�㲥�б�"": [
				{
				  ""�㲥����"": ""Eʵ���㲥"",
				  ""����"": false,
				  ""�㲥��Ϣ"": [
					""����һ���㲥"",
					""ÿ�����ִ��һ��"",
					""����ִ������"",
					""/time noon""
				  ],
				  ""RGB��ɫ"": [
					255.0,
					0.0,
					0.0
				  ],
				  ""ʱ����"": 300,
				  ""�ӳ�ִ��"": 60,
				  ""�㲥��"": [],
				  ""��������"": [],
				  ""����������"": false
				}
			  ]
			}");
        }
    }

    public class Broadcast
    {
        [JsonProperty("�㲥����")]
        public string Name = string.Empty;

        [JsonProperty("����")]
        public bool Enabled = false;

        [JsonProperty("�㲥��Ϣ")]
        public string[] Messages = new string[0];

        [JsonProperty("RGB��ɫ")]
        public float[] ColorRGB = new float[3];

        [JsonProperty("ʱ����")]
        public int Interval = 0;

        [JsonProperty("�ӳ�ִ��")]
        public int StartDelay = 0;

        [JsonProperty("广播组")]
        public string[] Groups { get; set; } = new string[0];

        [JsonProperty("触发词语")]
        public string[] TriggerWords { get; set; } = new string[0];

        [JsonProperty("触发整个组")]
        public bool TriggerToWholeGroup { get; set; } = false;
    }
}