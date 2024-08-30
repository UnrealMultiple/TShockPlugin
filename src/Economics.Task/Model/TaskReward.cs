using Newtonsoft.Json;

namespace Economics.Task.Model;

public class TaskReward
{
    [JsonProperty("执行命令")]
    public List<string> Commands { get; set; } = new();
}