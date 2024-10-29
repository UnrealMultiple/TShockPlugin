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
    public override Version Version => new Version(1, 1, 4);
    public override string Description => "涡轮增压不蒸鸭";
    #endregion

    #region 注册与释放
    public AutoAirItem(Main game) : base(game) { }
    internal static Configuration Config = new();
    internal static MyData Data = new();
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += ReloadConfig;
        ServerApi.Hooks.GameUpdate.Register(this, this.OnGameUpdate);
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        TShockAPI.Commands.ChatCommands.Add(new Command("AutoAir.use", Commands.AirCmd, "air", "垃圾"));
        TShockAPI.Commands.ChatCommands.Add(new Command("AutoAir.admin", Commands.Reset, "airreset", "重置垃圾桶"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnGameUpdate);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.AirCmd);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.Reset);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    private static void ReloadConfig(ReloadEventArgs args = null!)
    {
        LoadConfig();
        args.Player.SendInfoMessage(GetString("[自动垃圾桶]重新加载配置完毕。"));
    }
    private static void LoadConfig()
    {
        Config = Configuration.Read();
        Config.Write();
    }
    #endregion

    #region 玩家更新配置方法（创建配置结构）
    private void OnJoin(JoinEventArgs args)
    {
        if (args == null || !Config.Open)
        {
            return;
        }

        var plr = TShock.Players[args.Who];

        if (plr == null)
        {
            return;
        }

        // 查找玩家数据
        var data = Data.Items.FirstOrDefault(x => x.Name == plr.Name);

        // 如果玩家不在数据表中，则创建新的数据条目
        if (data == null || plr.Name != data.Name)
        {
            Data.Items.Add(new MyData.ItemData()
            {
                Name = plr.Name,
                Enabled = true,
                Auto = true,
                Mess = true,
                UpdateRate = 10,
                ItemName = new List<string>()
            });
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
            var list = Data.Items.FirstOrDefault(x => x.Name == plr.Name);
            if (list != null && list.Enabled && Timer % list.UpdateRate == 0)
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

        for (var i = 0; i < plr.inventory.Length; i++)
        {
            var item = plr.inventory[i];
            var id = TShock.Utils.GetItemById(item.type).netID;

            if (Auto)
            {
                if (!plr.trashItem.IsAir && !List.Contains(plr.trashItem.Name))
                {
                    List.Add(plr.trashItem.Name);
                    if (mess)
                    {
                        player.SendMessage(GetString($"已将 '[c/92C5EC:{plr.trashItem.Name}]'添加到自动垃圾桶|指令菜单:[c/A1D4C2:/air]"), 255, 246, 158);
                    }
                }
            }

            if (item != null && List.Contains(item.Name) && item.Name != plr.inventory[plr.selectedItem].Name)
            {
                item.TurnToAir();
                player.SendData(PacketTypes.PlayerSlot, null, player.Index, PlayerItemSlotID.Inventory0 + i);

                if (mess)
                {
                    var itemName = Lang.GetItemNameValue(id);
                    player.SendMessage(GetString($"【自动垃圾桶】已将 '[c/92C5EC:{itemName}]'从您的背包中移除"), 255, 246, 158);
                }
                return true;
            }
        }
        return false;
    }
    #endregion
}
