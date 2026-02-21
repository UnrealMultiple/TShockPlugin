using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoStoreItems;

[ApiVersion(2, 1)]
public class AutoStoreItems : LazyPlugin
{

    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "羽学 cmgy雱";
    public override Version Version => new Version(1, 3, 6);
    public override string Description => GetString("持有指定物品根据配置物品ID自动存入存储空间");
    #endregion

    #region 注册与释放
    public AutoStoreItems(Main game) : base(game) { }
    public override void Initialize()
    {
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdate);
        TShockAPI.Commands.ChatCommands.Add(new Command("AutoStore.use", Commands.Ast, "ast", "自存"));
        TShockAPI.Commands.ChatCommands.Add(new Command("AutoStore.admin", Commands.Reset, "astreset", "重置自存"));
    }


    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdate);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.Ast);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.Reset);
        }
        base.Dispose(disposing);
    }
    #endregion


    internal static MyData Data = new();


    #region 玩家更新配置方法（创建配置结构）
    private void OnJoin(JoinEventArgs args)
    {
        if (args == null || !Configuration.Instance.open)
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
                AutoMode = true,
                listen = true,
                Mess = true,
                ItemType = new List<int>()
            });
        }
    }
    #endregion

    #region 玩家移动触发自动储存物品方法
    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        var plr = e.Player;
        if (plr == null || !plr.IsLoggedIn || !plr.Active || !plr.HasPermission("AutoStore.use"))
        {
            return;
        }

        //从数据表里找名字 是否与当前玩家名一致
        var list = Data.Items.FirstOrDefault(x => x.Name == plr.Name);

        //数据表为空 返回
        if (list == null)
        {
            return;
        }

        //自动存物品，3个模式只许开一个
        if (list.AutoMode && !list.HandMode && !list.ArmorMode)
        {
            //遍历背包前50格内是否存在储物类的道具
            var inv = plr.TPlayer.inventory.Take(50).Any(x => Configuration.Instance.BankItems.Contains(x.type));
            //遍历工具栏第一格是否存在储物类道具（纯粹是为了方便装备上 眼骨）
            var miscEquips = plr.TPlayer.miscEquips.Take(1).Any(x => Configuration.Instance.BankItems.Contains(x.type));

            //如果存在则触发自动存储逻辑
            if (inv || miscEquips)
            {
                //存物品
                Tool.StoreItemInBanks(plr, list.listen, list.Mess, list.ItemType);

                //存钱逻辑，71-74 铜币-铂金币
                for (var i2 = 71; i2 <= 74; i2++)
                {
                    Tool.CoinToBank(plr, i2);
                }
            }
        }

        //手持储存，3个模式只许开一个
        if (list.HandMode && !list.AutoMode && !list.ArmorMode)
        {
            //如果储物类道具出现在玩家手上，则触发自动储存逻辑
            if (Configuration.Instance.BankItems.Contains(plr.TPlayer.inventory[plr.TPlayer.selectedItem].type))
            {
                //存物品
                Tool.StoreItemInBanks(plr, list.listen, list.Mess, list.ItemType);

                //存钱
                for (var i2 = 71; i2 <= 74; i2++)
                {
                    Tool.CoinToBank(plr, i2);
                }
            }
        }

        //盔甲识别，3个模式只许开一个
        if (list.ArmorMode && !list.HandMode && !list.AutoMode)
        {
            //遍历盔甲3格+饰品7格，是否存在储物类的道具
            var armor = plr.TPlayer.armor.Take(10).Any(x => Configuration.Instance.ArmorItem.Contains(x.type));

            //遍历宠物栏（方便眼骨）
            var miscEquips = plr.TPlayer.miscEquips.Take(1).Any(x => Configuration.Instance.ArmorItem.Contains(x.type));

            if (armor || miscEquips)
            {
                //存物品
                Tool.StoreItemInBanks(plr, list.listen, list.Mess, list.ItemType);

                //存钱
                for (var i2 = 71; i2 <= 74; i2++)
                {
                    Tool.CoinToBank(plr, i2);
                }
            }

        }
    }
    #endregion

}
