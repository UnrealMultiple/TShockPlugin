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

    [JsonIgnore]
    public static string PATH => Path.Combine(TShockAPI.TShock.SavePath, "ChattyBridge.json");

    public Config Read()
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
