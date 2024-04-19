using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace Autoclear
{
    public class Configuration
    {
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "AutoClear.json");

        [JsonProperty("多久检测一次(s)")]
        public int SmartSweepThreshold { get; set; } = 100;

        [JsonProperty("不清扫的物品ID列表")]
        public List<int> NonSweepableItemIDs { get; set; } = new List<int>();

        [JsonProperty("智能清扫数量临界值")]
        public int detectionIntervalSeconds { get; set; } = 10;

        [JsonProperty("延迟清扫(s)")]
        public int DelayedSweepTimeoutSeconds { get; set; } = 10;

        [JsonProperty("延迟清扫自定义消息")]
        public string DelayedSweepCustomMessage { get; set; } = "";

        [JsonProperty("是否清扫挥动武器")]
        public bool SweepSwinging { get; set; } = true;

        [JsonProperty("是否清扫投掷武器")]
        public bool SweepThrowable { get; set; } = true;

        [JsonProperty("是否清扫普通物品")]
        public bool SweepRegular { get; set; } = true;

        [JsonProperty("是否清扫装备")]
        public bool SweepEquipment { get; set; } = true;

        [JsonProperty("是否清扫时装")]
        public bool SweepVanity { get; set; } = true;

        [JsonProperty("完成清扫自定义消息")]
        public string CustomMessage { get; set; } = "";

        [JsonProperty("具体消息")]
        public bool SpecificMessage { get; set; } = true;


        public void Write(string path)
    {
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(str);
            }
        }
    }

    public static Configuration Read(string path)
    {
        if (!File.Exists(path))
            return new Configuration();
        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (var sr = new StreamReader(fs))
            {
                var cf = JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd());
                return cf;
            }
        }
    }
}
}