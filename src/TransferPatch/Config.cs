using Newtonsoft.Json;

namespace TransferPatch;

public class Config
{
    private static Config? _instance;

    public static Config Instance => _instance ??= GetConfig();

    public static string FileName => Path.Combine(TShockAPI.TShock.SavePath, "TransferPatch.json");

    private static Config GetConfig()
    {
        var c = new Config();
        if (File.Exists(FileName))
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(FileName)) ?? c;
        }
        Write(c);
        return c;
    }

    private static void Write(Config? c = null)
    {
        File.WriteAllText(FileName, JsonConvert.SerializeObject(c ?? _instance, Formatting.Indented));
    }

    internal static void OnReload(TShockAPI.Hooks.ReloadEventArgs e)
    {
        _instance = GetConfig();
    }

    [JsonProperty("启用")]
    public bool Enable { get; set; }

    [JsonProperty("目标程序集")]
    public string TargetAssembly { get; set; } = string.Empty;

    [JsonProperty("目标类")]
    public string[] TargetClassName { get; set; } = Array.Empty<string>();

    [JsonProperty("翻译列表")]
    public Dictionary<string, string> Transfers { get; set; } = new();


}
