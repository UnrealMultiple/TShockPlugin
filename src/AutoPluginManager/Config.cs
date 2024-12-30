using Newtonsoft.Json;

namespace AutoPluginManager;

public class Config
{
    public const string Path = "tshock/AutoPluginManager.json";

    public const string GiteePluginArchiveUrl = "https://gitee.com/kksjsj/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";
    public const string GiteePluginManifestUrl = "https://gitee.com/kksjsj/TShockPlugin/releases/download/V1.0.0.0/Plugins.json";

    public const string GithubPluginArchiveUrl = "https://github.com/UnrealMultiple/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";
    public const string GithubPluginManifestUrl = "https://github.com/UnrealMultiple/TShockPlugin/releases/download/V1.0.0.0/Plugins.json";

    public static string UpstreamPluginArchiveUrl => Instance.UseCustomSource ? Instance.CustomSourceArchiveUrl : Instance.UseGithubSource ? GithubPluginArchiveUrl : GiteePluginArchiveUrl;
    public static string UpstreamPluginManifestUrl => Instance.UseCustomSource ? Instance.CustomSourceManifestUrl : Instance.UseGithubSource ? GithubPluginManifestUrl : GiteePluginManifestUrl;

    public static Config Instance = new ();

    public void Write()
    {
        using FileStream fileStream = new (Path, FileMode.Create, FileAccess.Write, FileShare.Write);
        this.Write(fileStream);
    }

    private void Write(Stream stream)
    {
        var value = JsonConvert.SerializeObject(this, Formatting.Indented);
        using (StreamWriter streamWriter = new (stream))
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
            using FileStream fileStream = new (Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            result = Read(fileStream);
        }

        Instance = result!;
    }

    private static Config? Read(Stream stream)
    {
        Config? result;
        using StreamReader streamReader = new (stream);
        result = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
        return result;
    }

    [JsonProperty("允许自动更新插件")] public bool AutoUpdate = false;
    [JsonProperty("使用Github源")] public bool UseGithubSource = false;
    [JsonProperty("使用自定义源")] public bool UseCustomSource = false;
    [JsonProperty("自定义源清单地址")] public string CustomSourceManifestUrl = string.Empty;
    [JsonProperty("自定义源压缩文件地址")] public string CustomSourceArchiveUrl = string.Empty;
    [JsonProperty("插件排除列表")] public List<string> UpdateBlackList = new ();
    [JsonProperty("热重载升级插件")] public bool HotReloadPlugin = true;
    [JsonProperty("热重载出错时继续")] public bool ConinueHotReloadWhenError = true;
}