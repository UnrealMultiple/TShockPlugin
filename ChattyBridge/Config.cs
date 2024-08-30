using Newtonsoft.Json;

namespace ChattyBridge;

public class Config
{
    [JsonProperty("是否转发指令")]
    public bool ForwardCommamd { get; set; } = false;

    [JsonProperty("Rest地址")]
    public List<string> RestHost { get; set; } = new();

    [JsonProperty("服务器名称")]
    public string ServerName { get; set; } = string.Empty;

    [JsonProperty("验证令牌")]
    public string Verify { get; set; } = string.Empty;

    [JsonProperty("消息设置")]
    public MessageFormat MessageFormat { get; set; } = new();

    [JsonIgnore]
    public static string PATH => Path.Combine(TShockAPI.TShock.SavePath, "ChattyBridge.json");

    public static Config Read()
    {
        if (File.Exists(PATH))
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(PATH)) ?? new();
        return new();
    }

    public void Write()
    {
        File.WriteAllText(PATH, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

}
