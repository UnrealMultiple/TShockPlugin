using Newtonsoft.Json;
using System.Reflection;

namespace TransferPatch;

public class Config
{
    private static Config? _instance;

    public static Config Instance => _instance ??= GetConfig();

    public static string FileName => Path.Combine(TShockAPI.TShock.SavePath, "TransferPatch.json");

    private static Config GetConfig()
    {
        if (!File.Exists(FileName))
        {
            var ass = Assembly.GetExecutingAssembly();
            using var res = ass.GetManifestResourceStream("TransferPatch.Resources.TransferPatch.json")!;
            using var fs = File.Create(FileName);
            res.CopyTo(fs);
        }
        return JsonConvert.DeserializeObject<Config>(File.ReadAllText(FileName)) ?? new();
    }


    [JsonProperty("启用")]
    public bool Enable { get; set; }

    [JsonProperty("执行列表")]
    public List<TransferTarget> TransferTargets { get; set; } = [];

    public class TransferTarget
    { 
        [JsonProperty("目标程序集")]
        public string TargetAssembly { get; set; } = string.Empty;

        [JsonProperty("目标类")]
        public string[] TargetClassName { get; set; } = [];

        [JsonProperty("翻译列表")]
        public Dictionary<string, string> Transfers { get; set; } = [];
    }
}
