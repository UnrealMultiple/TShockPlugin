using Newtonsoft.Json;

namespace Economics.Task.Model;

public class TaskDemand
{
    [JsonProperty("NPC对话")]
    public HashSet<int> TallkNPC { get; set; } = new HashSet<int>();

    [JsonProperty("击杀怪物")]
    public List<KillNpc> KillNPCS { get; set; } = new();

    [JsonProperty("物品条件")]
    public List<Core.Model.Item> Items { get; set; } = new();
}