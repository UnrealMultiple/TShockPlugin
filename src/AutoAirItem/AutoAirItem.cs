using System.Text;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AutoAirItem;

[ApiVersion(2, 1)]
public class AutoAirItem : TerrariaPlugin
{
    #region 插件信息
    public override string Name => "自动垃圾桶";
    public override string Author => "羽学";
    public override Version Version => new Version(1, 1, 0);
    public override string Description => "涡轮增压不蒸鸭";
    #endregion

    #region 注册与释放
    public AutoAirItem(Main game) : base(game) { }
    private GeneralHooks.ReloadEventD _reloadHandler;
    internal static Configuration Config = new();
    internal static MyData data = new();
    public override void Initialize()
    {
        LoadConfig();
        this._reloadHandler = (_) => LoadConfig();
        GeneralHooks.ReloadEvent += this._reloadHandler;
        ServerApi.Hooks.GameUpdate.Register(this, this.OnGameUpdate);
        ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
        ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        TShockAPI.Commands.ChatCommands.Add(new Command("AutoAir.use", Commands.AirCmd, "air", "垃圾"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this._reloadHandler;
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnGameUpdate);
            ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
            ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.AirCmd);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    private static void LoadConfig()
    {
        Config = Configuration.Read();
        Config.Write();
        TShock.Log.ConsoleInfo("[自动垃圾桶]重新加载配置完毕。");
    }
    #endregion

    #region 玩家更新配置方法（计入记录时间并创建配置结构）
    private void OnJoin(JoinEventArgs args)
    {
        var plr = TShock.Players[args.Who];
        var list = data.Items.FirstOrDefault(x => x.Name == plr.Name);

        if (!Config.Open)
        {
            return;
        }

        //不是数据表里玩家则给他自动创建数据
        if (!data.Items.Any(item => item.Name == plr.Name))
        {
            data.Items.Add(new MyData.ItemData()
            {
                Name = plr.Name,
                Enabled = true,
                IsActive = true,
                Auto = true,
                LogTime = DateTime.Now,
                Mess = true,
                ItemName = new List<string>()
            });
        }
        else
        {
            list!.LogTime = DateTime.Now;
            list.IsActive = true;
        }
    }
    #endregion

    #region 玩家离开服务器更新记录时间，横比所有玩家的记录时间，如超过清理周期，则自动删除玩家数据
    private static int ClearCount = 0; //需要清理的玩家计数
    private void OnLeave(LeaveEventArgs args)
    {
        var plr = TShock.Players[args.Who];
        var list = data.Items.FirstOrDefault(x => x.Name == plr.Name);

        if (!Config.Open)
        {
            return;
        }

        //离开服务器更新记录时间与活跃状态
        if (data.Items.Any(item => item.Name == plr.Name))
        {
            list!.LogTime = DateTime.Now;
            list.IsActive = false;
        }

        if (Config.ClearData)
        {
            var Remove = data.Items.Where(list => list.LogTime != default && 
            (DateTime.Now - list.LogTime).TotalHours >= Config.timer).ToList();

            //数据清理的播报内容
            var mess = new StringBuilder();
            mess.AppendLine($"[i:3455][c/AD89D5:自][c/D68ACA:动][c/DF909A:垃][c/E5A894:圾][c/E5BE94:桶][i:3454]");
            mess.AppendLine($"以下玩家 与 [c/ABD6C7:{plr.Name}] 离开时间\n【[c/A1D4C2:{DateTime.Now}]】\n" +
                $"超过 [c/E17D8C:{Config.timer}] 小时 已清理 [c/76D5B4:自动垃圾桶] 数据：");

            foreach (var plr2 in Remove)
            {
                //只显示小时数 F0整数 F1保留1位小数 F2保留2位 如：24.01小时
                var hours = (DateTime.Now - plr2.LogTime).TotalHours;
                FormattableString Hours = $"{hours:F0}";

                //更新时间超过Config预设的时间，并该玩家更新状态为false则添加计数并移除数据
                if (hours >= Config.timer && !plr2.IsActive)
                {
                    ClearCount++;
                    mess.AppendFormat("[c/A7DDF0:{0}]:[c/74F3C9:{1}小时], ", plr2.Name, Hours);
                    data.Items.Remove(plr2);
                }
            }

            //确保有一个玩家计数，只播报一次
            if (ClearCount > 0 && mess.Length > 0)
            {
                //广告开关
                if (Config.Enabled)
                {
                    //自定义广告内容
                    mess.AppendLine(Config.Advertisement);
                }

                TShock.Utils.Broadcast(mess.ToString(), 247, 244, 150);
                ClearCount = 0;
            }
        }
    }
    #endregion

    #region 触发自动垃圾桶
    public static long Timer = 0L;
    private void OnGameUpdate(EventArgs args)
    {
        Timer++;

        if (!Config.Open)
        {
            return;
        }

        foreach (var plr in TShock.Players.Where(plr => plr != null && plr.Active && plr.IsLoggedIn))
        {
            var list = data.Items.FirstOrDefault(x => x.Name == plr.Name);
            if (list != null && list.Enabled && Timer % Config.UpdateRate == 0)
            {
                AutoAirItems(plr, list.ItemName, list.Auto, list.Mess);
            }
        }
    }
    #endregion

    #region 自动清理物品方法
    public static bool AutoAirItems(TSPlayer player, List<string> List, bool Auto, bool mess)
    {
        var plr = player.TPlayer;

        for (int i = 0; i < plr.inventory.Length; i++)
        {
            var item = plr.inventory[i];
            var id = TShock.Utils.GetItemById(item.type).netID;

            if (Auto)
            {
                if (!plr.trashItem.IsAir && !List.Contains(plr.trashItem.Name))
                {
                    List.Add(plr.trashItem.Name);
                    player.SendMessage($"已将 '[c/92C5EC:{plr.trashItem.Name}]'添加到自动垃圾桶|指令菜单:[c/A1D4C2:/air]", 255, 246, 158);
                }
            }

            if (item != null && List.Contains(item.Name) && item.Name != plr.inventory[plr.selectedItem].Name)
            {
                item.TurnToAir();
                player.SendData(PacketTypes.PlayerSlot, null, player.Index, PlayerItemSlotID.Inventory0 + i);

                if (mess)
                {
                    var itemName = Lang.GetItemNameValue(id);
                    player.SendMessage($"【自动垃圾桶】已将 '[c/92C5EC:{itemName}]'从您的背包中移除", 255, 246, 158);
                }
                return true;
            }
        }
        return false;
    }
    #endregion
}
