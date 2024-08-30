using Newtonsoft.Json;

namespace DumpTerrariaID;

public class Dump
{
    [JsonProperty("物品")]
    public List<Project> ItemTable = new();

    [JsonProperty("Buff")]
    public List<Project> BuffTable = new();

    [JsonProperty("NPC")]
    public List<Project> NpcTable = new();

    [JsonProperty("前缀")]
    public List<Project> PrefixTable = new();

    [JsonProperty("弹幕")]
    public List<Project> ProjecttileTable = new();
}