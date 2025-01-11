using Newtonsoft.Json;

namespace AutoPluginManager;

public class Config
{
    public const string Path = "tshock/AutoPluginManager.json";

    public static Config Instance = new();

    public void Write()
    {
        var value = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(Path, value);
    }

    public static void Read()
    {
        Instance = !File.Exists(Path) ? new() : JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path)) ?? new();
        Instance.Write();
    }

    [JsonProperty("允许自动更新插件")] public bool AutoUpdate = false;
    [JsonProperty("插件排除列表")] public List<string> UpdateBlackList = new();
    [JsonProperty("热重载升级插件")] public bool HotReloadPlugin = true;
    [JsonProperty("热重载出错时继续")] public bool ConinueHotReloadWhenError = true;
}