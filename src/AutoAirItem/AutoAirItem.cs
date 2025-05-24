using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoAirItem;

[ApiVersion(2, 1)]
public class AutoAirItem : LazyPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "羽学";
    public override Version Version => new Version(1, 2, 9);
    public override string Description => GetString("自动垃圾桶帮助玩家清理自身垃圾(需开启SSC)");
    #endregion

    #region 全局变量
    internal static Database DB = new();
    #endregion

    #region 注册与释放
    public AutoAirItem(Main game) : base(game) { }
    public override void Initialize()
    {
        GetDataHandlers.PlayerSlot.Register(this.OnPlayerSlot);
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdate);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
        TShockAPI.Commands.ChatCommands.Add(new Command("AutoAir.use", Commands.AirCmd, "air", "垃圾"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.PlayerSlot.UnRegister(this.OnPlayerSlot);
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdate);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.AirCmd);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 玩家更新配置方法（创建配置结构）
    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        if (args == null || !Configuration.Instance.Enabled)
        {
            return;
        }

        var plr = TShock.Players[args.Who];

        if (plr == null)
        {
            return;
        }

        var data = DB.GetData(plr.Name); //获取玩家数据方法
        if (data == null && TSPlayer.Server != plr) //如果没有获取到的玩家数据
        {
            data = new MyData.PlayerData(plr.Name, true, true, new Dictionary<int, int>());
            DB.AddData(data); //添加新数据
        }
    }

    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        var plr = e.Player;
        if (!Configuration.Instance.Enabled || plr == null || !plr.IsLoggedIn || !plr.Active || !plr.HasPermission("AutoAir.use"))
        {
            return;
        }

        var data = DB.GetData(plr.Name); //获取玩家数据方法
        if (data == null && TSPlayer.Server != plr) //如果没有获取到的玩家数据
        {
            data = new MyData.PlayerData(plr.Name, true, true, new Dictionary<int, int>());
            DB.AddData(data); //添加新数据
        }
    }
    #endregion

    #region 触发自动垃圾桶
    private void OnPlayerSlot(object? sender, GetDataHandlers.PlayerSlotEventArgs e)
    {
        var plr = e.Player;
        if (!Configuration.Instance.Enabled || e == null || plr == null || !plr.IsLoggedIn || !plr.Active || !plr.HasPermission("AutoAir.use"))
        {
            return;
        }

        var data = DB.GetData(plr.Name); //获取玩家数据方法

        if (data == null) //如果没有获取到的玩家数据
        {
            if (TSPlayer.Server != plr)
            {
                data = new MyData.PlayerData(plr.Name, true, true, new Dictionary<int, int>());
                DB.AddData(data); //添加新数据
            }
            return;
        }

        //如果玩家开启了功能开关
        if (data.Enabled) 
        {
            var trash = plr.TPlayer.trashItem; //获取玩家背包内的垃圾桶格子

            //排除钱币
            if (Configuration.Instance.Exclude.Contains(trash.type))
            {
                return;
            }

            if (!trash.IsAir) //玩家的垃圾桶不为空
            {
                //垃圾桶的物品不在 “自动垃圾桶物品表”
                if (!data.TrashList.ContainsKey(trash.type))
                {
                    //添加垃圾桶的物品和对应格子数量 到 “自动垃圾桶物品表”
                    data.TrashList.Add(trash.type, e.Stack);
                    plr.SendMessage(GetString($"检测到首次将[i/s{1}:{trash.type}]放入垃圾桶\n请手动[c/92C5EC:点击该物品]进行回收"), 240, 250, 150);
                }
            }

            //如果格子里的物品ID在“自动垃圾桶物品表”里 
            if (data.TrashList.ContainsKey(e.Type))
            {
                if (data.Mess) //触发回馈信息
                {
                    plr.SendMessage(GetString($"已从背包移除:{e.Stack}个[i/s{1}:{e.Type}]|[c/92C5EC:返还]: [c/A1D4C2:/air del {e.Type}]"), 240, 250, 150);
                }

                //将该格子的物品数量 添加到“自动垃圾桶物品表”
                data.TrashList[e.Type] += e.Stack;

                e.Stack = 0; //清空该格子的物品数量

                //避免卡BUG 顺带移除手上的物品
                plr.TPlayer.inventory[plr.TPlayer.selectedItem].TurnToAir();

                //发包给玩家到对应格子的物品触发以上移除逻辑
                plr.SendData(PacketTypes.PlayerSlot, "", plr.Index, e.Slot);
            }

            DB.UpdateData(data); //更新玩家自己的数据库
        }
    }
    #endregion
}
