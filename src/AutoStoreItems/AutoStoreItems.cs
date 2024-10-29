using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AutoStoreItems;

[ApiVersion(2, 1)]
public class AutoStoreItems : TerrariaPlugin
{

    #region 插件信息
    public override string Name => "自动存储";
    public override string Author => "羽学 cmgy雱";
    public override Version Version => new Version(1, 2, 8);
    public override string Description => "涡轮增压不蒸鸭";
    #endregion

    #region 注册与释放
    public AutoStoreItems(Main game) : base(game) { }
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += this.ReloadConfig;
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
        GetDataHandlers.PlayerUpdate.Register(this.PlayerUpdate);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnGameUpdate);
        TShockAPI.Commands.ChatCommands.Add(new Command("AutoStore.use", Commands.Ast, "ast", "自存"));
        TShockAPI.Commands.ChatCommands.Add(new Command("AutoStore.admin", Commands.Reset, "astreset", "重置自存"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this.ReloadConfig;
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
            GetDataHandlers.PlayerUpdate.UnRegister(this.PlayerUpdate);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnGameUpdate);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.Ast);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.Reset);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    internal static MyData Data = new();
    private void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player.SendInfoMessage(GetString("[自动存储]重新加载配置完毕。"));
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
        if (args == null || !Config.open)
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
                AutoMode = true,
                Bank = true,
                Mess = true,
                UpdateRate = 10,
                ItemName = new List<string>()
            });
        }
    }
    #endregion

    #region 检测背包持有物品方法
    public static long Timer = 0L;
    private void OnGameUpdate(EventArgs args)
    {
        Timer++;

        if (!Config.open)
        {
            return;
        }

        foreach (var plr in TShock.Players.Where(plr => plr != null && plr.Active && plr.IsLoggedIn && Config.open))
        {
            var list = Data.Items.FirstOrDefault(x => x.Name == plr.Name);

            if (list == null)
            {
                continue;
            }

            for (var i = 0; i < plr.TPlayer.inventory.Length; i++)
            {
                var inv = plr.TPlayer.inventory[i];

                //自动存物品
                if (list.AutoMode && !list.HandMode && !list.ArmorMode)
                {
                    if (Config.BankItems.Contains(inv.type) && Timer % list.UpdateRate == 0)
                    {
                        Tool.StoreItemInBanks(plr, inv, list.Bank, list.Mess, list.ItemName);

                        for (var i2 = 71; i2 <= 74; i2++)
                        {
                            Tool.CoinToBank(plr, i2);
                        }
                        break;
                    }
                }
                //手持储存
                else if(list.HandMode && !list.AutoMode && !list.ArmorMode)
                {
                    if (Config.BankItems.Contains(plr.TPlayer.inventory[plr.TPlayer.selectedItem].type))
                    {
                        Tool.StoreItemInBanks(plr, inv, list.Bank, list.Mess, list.ItemName);

                        for (var i2 = 71; i2 <= 74; i2++)
                        {
                            Tool.CoinToBank(plr, i2);
                        }

                        break;
                    }
                }
            }
        }
    }
    #endregion

    #region 检测装备物品方法
    private void PlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        var plr = e.Player;
        if (plr == null)
        {
            return;
        }

        var list = Data.Items.FirstOrDefault(x => x.Name == plr.Name);

        if (list == null || !list.ArmorMode || list.AutoMode || list.HandMode)
        {
            return;
        }

        var Stored = false;
        foreach (var item in Config.ArmorItem)
        {
            var armor = plr.TPlayer.armor.Take(10).Where(x => x.netID == item).ToList();
            var hasArmor = armor.Any();

            if (hasArmor)
            {
                for (var i = 71; i <= 74; i++)
                {
                    Tool.CoinToBank(plr, i);
                }

                Stored |= Config.bank1 && Tool.AutoStoredItem(plr, plr.TPlayer.bank.item, PlayerItemSlotID.Bank1_0, GetString("存钱罐"), list.Bank, list.Mess, list.ItemName);
                Stored |= Config.bank2 && Tool.AutoStoredItem(plr, plr.TPlayer.bank2.item, PlayerItemSlotID.Bank2_0, GetString("保险箱"), list.Bank, list.Mess, list.ItemName);
                Stored |= Config.bank3 && Tool.AutoStoredItem(plr, plr.TPlayer.bank3.item, PlayerItemSlotID.Bank3_0, GetString("护卫熔炉"), list.Bank, list.Mess, list.ItemName);
                Stored |= Config.bank4 && Tool.AutoStoredItem(plr, plr.TPlayer.bank4.item, PlayerItemSlotID.Bank4_0, GetString("虚空袋"), list.Bank, list.Mess, list.ItemName);

                if (Stored)
                {
                    break;
                }
            }
        }
    }
    #endregion

}
