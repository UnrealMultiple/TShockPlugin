using Economics.Task.Model;
using Newtonsoft.Json;

namespace Economics.Task;

internal class Config
{
    [JsonProperty("不可重复接任务")]
    public bool PickTasks { get; set; } = true;

    [JsonProperty("页显示数量")]
    public int PageCount { get; set; } = 10;

    [JsonProperty("任务列表")]
    public List<TaskContent> Tasks { get; set; } = new();

    public TaskContent? GetTask(int id)
    {
        return this.Tasks.Find(f => f.TaskID == id);
    }
}