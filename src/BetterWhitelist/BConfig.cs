using Newtonsoft.Json;

namespace BetterWhitelist;

public class BConfig
{
    [JsonProperty("白名单玩家", Order = 3)] public List<string> WhitePlayers = new();

    [JsonProperty("插件开关", Order = 0)] public bool Disabled { get; set; }

    [JsonProperty("连接时不在白名单提示", Order = 1)]
    public string NotInWhiteList { get; set; } = GetString("你不在服务器白名单中！");

    public static BConfig Load(string path)
    {
        var result = File.Exists(path)
            ? JsonConvert.DeserializeObject<BConfig>(File.ReadAllText(path))!
            : new BConfig
            {
                Disabled = false
            };
        return result;
    }
}