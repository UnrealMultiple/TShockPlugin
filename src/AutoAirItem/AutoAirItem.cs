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
    public override Version Version => new Version(1, 1, 6);
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
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdate);
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        TShockAPI.Commands.ChatCommands.Add(new Command("AutoAir.use", Commands.AirCmd, "air", "垃圾"));
        TShockAPI.Commands.ChatCommands.Add(new Command("AutoAir.admin", Commands.Reset, "airreset", "重置垃圾桶"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdate);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.AirCmd);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.Reset);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    private static void ReloadConfig(ReloadEventArgs args)
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

        // 如果玩家不在数据表中，则创建新的数据条目
        if (!Data.Items.Any(item => item.Name == plr.Name))
        {
            Data.Items.Add(new MyData.ItemData()
            {
                Name = plr.Name,
                Enabled = true,
                Auto = true,
                Mess = true,
                ItemType = new List<int>(),
                DelItem = new Dictionary<int, int> { { 0, 0 }, }
            });
        }
    }
    #endregion

    #region 触发自动垃圾桶
    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        var plr = e.Player;
        if (!Config.Open || e == null || plr == null || !plr.IsLoggedIn || !plr.Active || !plr.HasPermission("AutoAir.use"))
        {
            return;
        }

        var list = Data.Items.FirstOrDefault(x => x.Name == plr.Name);
        if (list == null)
        {
            return;
        }

        //玩家自己的垃圾桶开关
        if (list.Enabled)
        {
            //遍历背包58格
            for (var i = 0; i < plr.TPlayer.inventory.Length; i++)
            {
                // 跳过钱币栏格子
                if (i > 50 && i < 54)
                {
                    continue;
                }

                //当前背包的格子
                var inv = plr.TPlayer.inventory[i];

                //读取垃圾桶位格的物品 写入到玩家自己的垃圾桶表里
                if (list.Auto)
                {
                    //放进垃圾桶就视为垃圾
                    if (!plr.TPlayer.trashItem.IsAir && !list.ItemType.Contains(plr.TPlayer.trashItem.type))
                    {
                        list.ItemType.Add(plr.TPlayer.trashItem.type);

                        //触发回馈信息
                        if (list.Mess)
                        {
                            plr.SendMessage(GetString($"已将 '[c/92C5EC:{plr.TPlayer.trashItem.Name}]'添加到自动垃圾桶|指令菜单: [c/A1D4C2:/air]"), 255, 246, 158);
                        }
                    }
                }

                //是垃圾桶表的物品,不是手上的物品 进行移除
                if (list.ItemType.Contains(inv.type) && inv.type != plr.TPlayer.inventory[plr.TPlayer.selectedItem].type)
                {
                    //将已经移除的物品更新到字典，使用/air del指令能方便返还
                    UpDict(list.DelItem, inv.type, inv.stack);

                    if (list.Mess)
                    {
                        var name = Lang.GetItemName(inv.type);

                        plr.SendMessage(GetString($"已将 '[c/92C5EC:{name}]'从您的背包中移除|[c/92C5EC:返还物品]: [c/A1D4C2:/air del {inv.type}]"), 255, 246, 158);
                    }

                    //将背包指定物品清空并发包
                    inv.TurnToAir();
                    plr.SendData(PacketTypes.PlayerSlot, "", plr.Index, PlayerItemSlotID.Inventory0 + i);
                }
            }
        }
    }
    #endregion

    #region 更新字典:把清理掉的物品和数量记录下来
    public static void UpDict(Dictionary<int, int> delItem, int type, int stack)
    {
        //如果ID已经在字典里
        if (delItem.ContainsKey(type))
        {
            // 给这个ID加数量
            delItem[type] += stack;
        }

        // ID不在字典里
        else
        {
            // 直接添加新ID和它的数量
            delItem.Add(type, stack);
        }
    }
    #endregion
}
