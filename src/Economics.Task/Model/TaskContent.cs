using Newtonsoft.Json;

namespace Economics.Task.Model;

public class TaskContent
{
    [JsonProperty("任务名称")]
    public string TaskName { get; set; } = "示例任务";

    [JsonProperty("任务序号")]
    public int TaskID { get; set; } = 1;

    [JsonProperty("前置任务")]
    public HashSet<int> FinishTask { get; set; } = new();

    [JsonProperty("限制接取等级")]
    public HashSet<string> LimitLevel { get; set; } = new();

    [JsonProperty("限制接取进度")]
    public HashSet<string> LimitProgress { get; set; } = new();

    [JsonProperty("任务内容")]
    public TaskDemand TaskInfo { get; set; } = new();

    [JsonProperty("任务介绍")]
    public string Description { get; set; } = "这是一个测试任务!";

    [JsonProperty("完成后提示")]
    public string FinishTaskFormat { get; set; } = "恭喜你完成了此任务!";

    [JsonProperty("任务奖励")]
    public TaskReward Reward { get; set; } = new();
}