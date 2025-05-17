using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ReFishTask;

[ApiVersion(2, 1)]
public class ReFishTask : TerrariaPlugin
{

    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "羽学";
    public override Version Version => new Version(1, 4, 5);
    public override string Description => GetString("当你完成渔夫任务时可无限接渔夫任务");
    #endregion

    #region 注册与释放
    public ReFishTask(Main game) : base(game)
    {
        this._reloadHandler = (_) => LoadConfig();
    }
    private readonly GeneralHooks.ReloadEventD _reloadHandler;
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += this._reloadHandler;
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        ServerApi.Hooks.NetGetData.Register(this, OnGetData);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this._reloadHandler;
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
            ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    private static void LoadConfig()
    {
        Config = Configuration.Read();
        Config.Write();
        TShock.Log.ConsoleInfo(GetString("[ReFishTask]重新加载配置完毕。"));
    }
    #endregion

    #region 刷新渔夫任务方法
    private static readonly HashSet<string> PlayerDoneList = new HashSet<string>();
    private static void OnGetData(GetDataEventArgs args)
    {
        var plr = TShock.Players[args.Msg.whoAmI];
        if (plr == null || !plr.IsLoggedIn)
        {
            return;
        }

        if (args.MsgID == PacketTypes.CompleteAnglerQuest)
        {
            if (Config.SwitchTasks) { AnglerQuestSwap(plr); }

            PlayerDoneList.Add(plr.Name);
            Main.anglerWhoFinishedToday.Remove(plr.Name);
            NetMessage.SendAnglerQuest(plr.Index);
        }
    }
    #endregion

    #region 处理加入服务器方法
    private void OnJoin(JoinEventArgs args)
    {
        var plr = TShock.Players[args.Who];
        if (plr == null)
        {
            return;
        }

        if (PlayerDoneList.Contains(plr.Name))
        {
            Main.anglerWhoFinishedToday.Remove(plr.Name);
        }

        NetMessage.SendAnglerQuest(plr.Index);
    }
    #endregion

    #region 更换任务鱼并判断进度方法 改了：Terraria.Main.AnglerQuestSwap();
    public static void AnglerQuestSwap(TSPlayer plr)
    {
        //我只想移除个人名单所以改成Remove
        Main.anglerWhoFinishedToday.Remove(plr.Name);
        Main.anglerQuestFinished = false;
        var flag = NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || Main.hardMode || NPC.downedSlimeKing || NPC.downedQueenBee;
        var flag2 = true;
        while (flag2)
        {
            flag2 = false;
            Main.anglerQuest = Main.rand.Next(Main.anglerQuestItemNetIDs.Length);
            var num = Main.anglerQuestItemNetIDs[Main.anglerQuest];
            if (num == 2454 && (!Main.hardMode || WorldGen.crimson))
            {
                flag2 = true;
            }

            if (num == 2457 && WorldGen.crimson)
            {
                flag2 = true;
            }

            if (num == 2462 && !Main.hardMode)
            {
                flag2 = true;
            }

            if (num == 2463 && (!Main.hardMode || !WorldGen.crimson))
            {
                flag2 = true;
            }

            if (num == 2465 && !Main.hardMode)
            {
                flag2 = true;
            }

            if (num == 2468 && !Main.hardMode)
            {
                flag2 = true;
            }

            if (num == 2471 && !Main.hardMode)
            {
                flag2 = true;
            }

            if (num == 2473 && !Main.hardMode)
            {
                flag2 = true;
            }

            if (num == 2477 && !WorldGen.crimson)
            {
                flag2 = true;
            }

            if (num == 2480 && !Main.hardMode)
            {
                flag2 = true;
            }

            if (num == 2483 && !Main.hardMode)
            {
                flag2 = true;
            }

            if (num == 2484 && !Main.hardMode)
            {
                flag2 = true;
            }

            if (num == 2485 && WorldGen.crimson)
            {
                flag2 = true;
            }

            if ((num == 2476 || num == 2453 || num == 2473) && !flag)
            {
                flag2 = true;
            }
        }

        NetMessage.SendAnglerQuest(plr.Index);
    }
    #endregion

}