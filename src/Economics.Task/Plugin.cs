using Economics.Task.DB;
using Economics.Task.Model;
using EconomicsAPI.Configured;
using EconomicsAPI.EventArgs.PlayerEventArgs;
using EconomicsAPI.Events;
using Rests;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.Task;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("提供任务系统!");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 0, 3);

    internal static Config TaskConfig = new();

    private readonly string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Task.json");

    internal static TaskFinishManager TaskFinishManager { get; private set; } = null!;

    internal static TaskKillNPCManager KillNPCManager = null!;

    internal static TaskTallkManager TallkManager = null!;

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        this.LoadConfig();
        TaskFinishManager = new();
        KillNPCManager = new();
        TallkManager = new();
        PlayerHandler.OnPlayerKillNpc += this.OnKillNpc;
        GetDataHandlers.NpcTalk.Register(this.OnNpcTalk);
        GeneralHooks.ReloadEvent += this.LoadConfig;
        TShock.RestApi.Register("/taskFinish", this.Finish);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            EconomicsAPI.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            EconomicsAPI.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            PlayerHandler.OnPlayerKillNpc -= this.OnKillNpc;
            GetDataHandlers.NpcTalk.UnRegister(this.OnNpcTalk);
            GeneralHooks.ReloadEvent -= this.LoadConfig;
        }
        base.Dispose(disposing);
    }

    private object Finish(RestRequestArgs args)
    {
        if (args.Parameters["name"] == null)
        {
            return new RestObject("201") { Response = GetString("没有检测到玩家名称") };
        }

        if (args.Parameters["taskid"] == null)
        {
            return new RestObject("201") { Response = GetString("没有检测到任务ID") };
        }

        if (!int.TryParse(args.Parameters["taskid"], out var taskid))
        {
            return new RestObject("201") { Response = GetString("非法的任务ID") };
        }

        var task = TaskFinishManager.GetTaksByName(args.Parameters["name"]);
        var finish = task.Any(x => x.TaskID == taskid);
        return new RestObject() { { "response", GetString("查询成功") }, { "code", finish } };
    }

    private void LoadConfig(ReloadEventArgs? args = null)
    {
        if (!File.Exists(this.PATH))
        {
            TaskConfig.Tasks = new()
            {
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
                        Items = new List<EconomicsAPI.Model.Item>
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
            };
        }
        TaskConfig = ConfigHelper.LoadConfig(this.PATH, TaskConfig);
    }

    public static bool InOfFinishTask(TSPlayer tSPlayer, HashSet<int> tasks)
    {
        if (tasks.Count == 0)
        {
            return true;
        }

        var successtask = TaskFinishManager.GetTaksByName(tSPlayer.Name, TaskStatus.Success);
        if (successtask != null)
        {
            foreach (var task in tasks)
            {
                if (!successtask.Any(x => x.TaskID == task))
                {
                    return false;
                }
            }
        }
        return true;
    }



    private void OnNpcTalk(object? sender, GetDataHandlers.NpcTalkEventArgs e)
    {
        var task = UserTaskData.GetUserTask(e.Player.Name);
        if (task != null && e.NPCTalkTarget != -1)
        {
            if (task.TaskInfo.TallkNPC.Contains(Main.npc[e.NPCTalkTarget].netID))
            {
                TallkManager.AddTallkNPC(e.Player.Name, Main.npc[e.NPCTalkTarget].netID);
            }
        }
    }

    private void OnKillNpc(PlayerKillNpcArgs args)
    {
        if (args.Npc == null)
        {
            return;
        }

        var task = UserTaskData.GetUserTask(args.Player!.Name);
        if (task != null)
        {
            var kill = task.TaskInfo.KillNPCS.Find(x => x.ID == args.Npc.netID);
            if (kill != null)
            {
                if (KillNPCManager.GetKillNpcsCountByID(args.Player.Name, args.Npc.netID) < kill.Count)
                {
                    KillNPCManager.AddKillNpc(args.Player.Name, args.Npc.netID);
                }
            }
        }
    }
}