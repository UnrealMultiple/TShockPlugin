using Newtonsoft.Json;

namespace MorMorAdapter.Setting.Configs;

public class ResetConfig
{
    [JsonProperty("删除地图")]
    public bool ClearMap { get; set; }

    [JsonProperty("删除日志")]
    public bool ClearLogs { get; set; }

    [JsonProperty("执行命令")]
    public List<string> Commands { get; set; } = new();

    [JsonProperty("删除表")]
    public List<string> ClearTable { get; set; } = new();
}
