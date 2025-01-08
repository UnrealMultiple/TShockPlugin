using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TShockAPI;

public class Config
{
    public static Config config = new Config();

    private static readonly string path = Path.Combine(TShock.SavePath, "VotePlus.json");

    [JsonProperty("启用投票踢出")]

    public bool EnableKick { get; set; } = true;

    [JsonProperty("踢出持续时间(秒)")]

    public int KickDuration { get; set; } = 600;

    [JsonProperty("启用投票封禁")]
    public bool EnableBan { get; set; } = false;

    [JsonProperty("启用投票禁言")]
    public bool EnableMute { get; set; } = true;

    [JsonProperty("启用投票清除BOSS")]

    public bool EnableBossClear { get; set; } = true;

    [JsonProperty("启用投票关闭事件")]

    public bool EnableEventClear { get; set; } = true;

    [JsonProperty("启用投票修改时间")]

    public bool EnableTimeChange { get; set; } = true;

    [JsonProperty("启用投票修改天气")]

    public bool EnableWeatherChange { get; set; } = true;

    [JsonProperty("启用自由投票")]

    public bool EnableFreeVote { get; set; } = true;

    [JsonProperty("最小投票人数")]

    public int MinVote { get; set; } = 3;


    [JsonProperty("投票通过率")]
    public int VotePass { get; set; } = 60;

    public void Write()
    {
        using StreamWriter streamWriter = new StreamWriter(path);
        streamWriter.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public static void GetConfig()
    {
        var newconfig = new Config();
        if (!File.Exists(path))
        {
            using StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(JsonConvert.SerializeObject(config, Formatting.Indented));
        }
        else
        {
            using StreamReader streamReader = new StreamReader(path);
            newconfig = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd())!;
        }
        config = newconfig;
    }
}
