using Economics.Task.DB;
using Economics.Task.Model;
using Economics.Core.ConfigFiles;
using Economics.Core.EventArgs.PlayerEventArgs;
using Economics.Core.Events;
using Rests;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Economics.Task;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("提供任务系统!");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 0, 5);

    internal static TaskFinishManager TaskFinishManager { get; private set; } = null!;

    internal static TaskKillNPCManager KillNPCManager = null!;

    internal static TaskTallkManager TallkManager = null!;

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Config.Load();
        TaskFinishManager = new();
        KillNPCManager = new();
        TallkManager = new();
        PlayerHandler.OnPlayerKillNpc += this.OnKillNpc;
        GetDataHandlers.NpcTalk.Register(this.OnNpcTalk);
        TShock.RestApi.Register("/taskFinish", this.Finish);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Core.Economics.RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            Core.Economics.RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            PlayerHandler.OnPlayerKillNpc -= this.OnKillNpc;
            GetDataHandlers.NpcTalk.UnRegister(this.OnNpcTalk);
            Config.Load();
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