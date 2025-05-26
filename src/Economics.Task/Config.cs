using Economics.Core.ConfigFiles;
using Economics.Task.Model;
using Newtonsoft.Json;

namespace Economics.Task;

public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "Task.json";

    [JsonProperty("不可重复接任务")]
    public bool PickTasks { get; set; } = true;

    [JsonProperty("页显示数量")]
    public int PageCount { get; set; } = 10;

    [JsonProperty("任务列表")]
    public List<TaskContent> Tasks { get; set; } = [];

    protected override void SetDefault()
    {
        this.Tasks =
        [
            new TaskContent()
            {
                TaskName = "狄拉克的请求",
                TaskID = 1,
                Description = "哦，亲爱的朋友，你是来帮我的吗? 麻烦你去告诉商人那个老东西一声，让他不要忘记了我的生日，还有一件事最近有两只可恶的恶魔之眼，在我家附近，帮我杀掉他，并把晶状体给我，我还需要你去给我找几个红水晶，我要用这些打造一个神奇的小东西。作为报酬，我会请树妖对你进行赐福，在赠予你一些药水，它会让你更好的活下去。",
                FinishTaskFormat = "哦，感谢你我的朋友,你叫{0}对吧，我记住了! 收好你的奖品!",
                TaskInfo = new TaskDemand()
                {
                    TallkNPC = new()
                    {
                        17
                    },
                    Items = new List<Economics.Core.Model.Item>
                    {
                        new ()
                        {
                            netID = 178,
                            Stack = 10,
                            Prefix = 0
                        },
                        new()
                        {
                            netID = 38,
                            Stack = 2,
                            Prefix = 0
                        }
                    },
                    KillNPCS = new List<KillNpc>
                    {
                        new()
                        {
                            ID = 2,
                            Count = 2,
                        }
                    }
                },
                Reward = new TaskReward()
                {
                    Commands = new List<string>()
                    {
                        "/permabuff 165",
                        "/i 499"
                    }
                }
            }
        ];
    }

    public TaskContent? GetTask(int id)
    {
        return this.Tasks.Find(f => f.TaskID == id);
    }
}