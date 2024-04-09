using Newtonsoft.Json;

namespace ChattyBridge;

public class Config
{
    [JsonProperty("Rest地址")]
    public List<string> RestHost = new();

    [JsonProperty("服务器名称")]
    public string ServerName = string.Empty;

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
