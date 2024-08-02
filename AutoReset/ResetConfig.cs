using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AutoReset.MainPlugin;

[JsonObject]
public class ResetConfig
{
    [JsonProperty("CaiBot服务器令牌",Order = 8)]
    public string CaiBotToken = "西江超级可爱喵";
    
    [JsonProperty("替换文件",Order = 1)]
    public Dictionary<string, string>? Files;

    [JsonProperty("击杀重置",Order = 2)]
    public AutoReset KillToReset = new();

    [JsonProperty("重置后指令",Order = 3)]
    public string[]? PostResetCommands;

    [JsonProperty("重置前指令",Order = 4)]
    public string[]? PreResetCommands;

    [JsonProperty("重置提醒",Order = 7)]
    public bool ResetCaution;

    [JsonProperty("地图预设",Order = 6)]
    public SetWorldConfig SetWorld = new();

    [JsonProperty("重置后SQL命令",Order = 5)]
    public string[]? SqLs;


    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class SetWorldConfig
    {
        [JsonProperty("地图名",Order = 0)]
        public string? Name;

        [JsonProperty("地图种子",Order = 1)]
        public string? Seed;
    }

    public class AutoReset
    {
        [JsonProperty("击杀重置开关",Order = 0)]
        public bool Enable;

        [JsonProperty("已击杀次数",Order = 1)]
        public int KillCount;

        [JsonProperty("需要击杀次数",Order = 3)]
        public int NeedKillCount = 50;

        [JsonProperty("生物ID",Order = 2)]
        public int NpcId = 50;
    }
}