using Newtonsoft.Json;

namespace DumpTerrariaID;

public class Project
{
    [JsonProperty("中文名称")]
    public string Chains { get; set; } = "";

    [JsonProperty("英文名称")]
    public string English { get; set; } = "";
    public int ID { get; set; }
}