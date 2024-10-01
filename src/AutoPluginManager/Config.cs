using Newtonsoft.Json;

namespace AutoPluginManager;

public class Config
{
    public const string Path = "tshock/AutoPluginManager.json";

    public static Config PluginConfig = new();

    public void Write()
    {
        using FileStream fileStream = new(Path, FileMode.Create, FileAccess.Write, FileShare.Write);
        this.Write(fileStream);
    }

    private void Write(Stream stream)
    {
        var value = JsonConvert.SerializeObject(this, Formatting.Indented);
        using (StreamWriter streamWriter = new(stream))
        {
            streamWriter.Write(value);
        }
    }

    public static void Read()
    {
        Config? result;
        if (!File.Exists(Path))
        {
            result = new Config();
            result.Write();
        }
        else
        {
            using FileStream fileStream = new(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            result = Read(fileStream);
        }
        PluginConfig = result!;
    }

    private static Config? Read(Stream stream)
    {
        Config? result;
        using StreamReader streamReader = new(stream);
        result = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
        return result;
    }
    [JsonProperty("允许自动更新插件")] public bool AutoUpdate = false;
    [JsonProperty("使用Github源")] public bool UseGithubSource = false;
    [JsonProperty("插件排除列表")] public List<string> UpdateBlackList = new();
    [JsonProperty("热重载升级插件")] public bool AutoReloadPlugin = true;
}