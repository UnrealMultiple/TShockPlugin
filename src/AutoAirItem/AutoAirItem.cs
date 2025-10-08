using LazyAPI;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoAirItem;

[ApiVersion(2, 1)]
public class AutoAirItem : LazyPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "羽学";
    public override Version Version => new Version(1, 2, 8);
    public override string Description => GetString("自动垃圾桶帮助玩家清理自身垃圾");
    #endregion

    #region 注册与释放
    public AutoAirItem(Main game) : base(game) { }
    internal static MyData Data = new();
    internal static Database DB = new();
    public override void Initialize()
    {
        if (Configuration.Instance.Db)
        {
            this.LoadAllPlayerData(); // 加载所有玩家的数据
        }
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdate);
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreetPlayer);
        this.addCommands.Add(new Command("AutoAir.use", Commands.AirCmd, "air", "垃圾"));
        this.addCommands.Add(new Command("AutoAir.admin", Commands.Reset, "airreset", "重置垃圾桶"));
        base.Initialize();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdate);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreetPlayer);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 加载所有玩家数据
    private void LoadAllPlayerData()
    {
        foreach (var data in DB.LoadData())
        {
            Data.Items.Add(data);
        }
    }
    #endregion

    #region 玩家更新配置方法（创建配置结构）
    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        if (args == null || !Configuration.Instance.Enable)
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
            Data.Items.Add(new MyData.PlayerData()
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
        if (!Configuration.Instance.Enable || e == null || plr == null || !plr.IsLoggedIn || !plr.Active || !plr.HasPermission("AutoAir.use"))
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
                //物品的对应格子
                var inv = plr.TPlayer.inventory[i];
                var trash = plr.TPlayer.trashItem;
                var selected = plr.TPlayer.inventory[plr.TPlayer.selectedItem];

                //排除钱币
                if (Configuration.Instance.Exclude.Contains(trash.type) || Configuration.Instance.Exclude.Contains(inv.type))
                {
                    return;
                }

                if (list.Auto) //读取垃圾桶位格的物品 写入到玩家自己的垃圾桶表里
                {
                    if (!trash.IsAir) //放进垃圾桶就视为垃圾
                    {
                        if (!list.ItemType.Contains(trash.type)) //不是垃圾桶表的物品
                        {
                            list.ItemType.Add(trash.type); //添加到垃圾桶表
                        }
                        else
                        {
                            if (list.Mess) //触发回馈信息
                            {
                                var name = Lang.GetItemName(trash.type);
                                plr.SendMessage(GetString($"已从垃圾桶移除:[c/EC73B9:{trash.stack}]个[c/FF5C5F:{name}]|[c/92C5EC:返还]: [c/A1D4C2:/air del {trash.type}]"), 255, 246, 158);
                            }

                            //将要移除的物品更新到字典，使用/air del指令能方便返还
                            UpDict(list.DelItem, trash.type, trash.stack);
                            DB.UpdateData(list); //更新到数据库
                            trash.TurnToAir();
                            plr.SendData(PacketTypes.PlayerSlot, "", plr.Index, PlayerItemSlotID.TrashItem);
                        }
                    }
                }

                //是垃圾桶表的物品,不是手上的物品 进行移除
                if (list.ItemType.Contains(inv.type) && inv.type != selected.type)
                {
                    if (list.Mess)
                    {
                        var name = Lang.GetItemName(inv.type);
                        plr.SendMessage(GetString($"已从背包移除:[c/EC73B9:{inv.stack}]个[c/FF5C5F:{name}]|[c/92C5EC:返还]: [c/A1D4C2:/air del {inv.type}]"), 255, 246, 158);
                    }

                    UpDict(list.DelItem, inv.type, inv.stack);
                    DB.UpdateData(list); //更新到数据库
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
