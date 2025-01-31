using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.UI;
using Terraria.Utilities;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;


namespace Economics.WeaponPlus;

[ApiVersion(2, 1)]
public class WeaponPlus : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "z枳 少司命";

    public override string Description => GetString("允许在基础属性上强化任何武器, Allow any weapon to be strengthened on basic attributes");

    public override Version Version => new Version(2, 0, 0, 4);
    #endregion

    #region 实例变量
    public static WeaponPlusDB DB { get; set; } = null!;

    public string configPath = Path.Combine(TShock.SavePath, "WeaponPlus.json");

    public static TerrariaMap.Config config = new TerrariaMap.Config();

    public static WPlayer[] wPlayers = new WPlayer[256];

    #endregion

    #region 注册与卸载钩子
    public WeaponPlus(Main game) : base(game) { }
    public override void Initialize()
    {
        DB = new WeaponPlusDB(TShock.DB);
        LoadConfig();
        GeneralHooks.ReloadEvent += LoadConfig;
        ServerApi.Hooks.NetGreetPlayer.Register((TerrariaPlugin) (object) this, this.OnGreetPlayer);
        ServerApi.Hooks.ServerLeave.Register((TerrariaPlugin) (object) this, this.OnServerLeave);
        Commands.ChatCommands.Add(new Command("weaponplus.plus", this.PlusItem, "plus")
        {
            HelpText = GetString("输入 /plus    查看当前该武器的等级状态和升至下一级需要多少材料")
        });
        Commands.ChatCommands.Add(new Command("weaponplus.admin", this.ClearPlusItem, "clearallplayersplus")
        {
            HelpText = GetString("输入 /clearallplayersplus    将数据库中所有玩家的所有强化物品全部清理，管理员专属")
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= LoadConfig;
            ServerApi.Hooks.NetGreetPlayer.Deregister((TerrariaPlugin) (object) this, this.OnGreetPlayer);
            ServerApi.Hooks.ServerLeave.Deregister((TerrariaPlugin) (object) this, this.OnServerLeave);
        }
    }
    #endregion


    #region 配置文件创建与重读加载方法
    private static void LoadConfig(ReloadEventArgs args = null!)
    {
        config = TerrariaMap.Config.Read(TerrariaMap.Config.configPath);
        config.Write(TerrariaMap.Config.configPath);
        if (args != null && args.Player != null)
        {
            args.Player.SendSuccessMessage(GetString("[武器强化EC版]重新加载配置完毕。"));
        }
    }
    #endregion

    #region 问候玩家
    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        if (args == null)
        {
            return;
        }
        wPlayers[args.Who] = new WPlayer(TShock.Players[args.Who])
        {
            hasItems = DB.ReadDBGetWItemsFromOwner(TShock.Players[args.Who].Name).ToList()
        };
        if (!config.WhetherToTurnOnAutomaticLoadWeaponsWhenEnteringTheGame || wPlayers.Length == 0)
        {
            return;
        }
        foreach (var hasItem in wPlayers[args.Who].hasItems)
        {
            ReplaceWeaponsInBackpack(Main.player[args.Who], hasItem);
        }
    }
    #endregion

    #region 离开服务器
    private void OnServerLeave(LeaveEventArgs args)
    {
        if (args == null || TShock.Players[args.Who] == null)
        {
            return;
        }
        try
        {
            if (wPlayers[args.Who] != null)
            {
                DB.WriteDB(wPlayers[args.Who].hasItems.ToArray());
            }
        }
        catch
        {
        }
    }
    #endregion

    #region  强化物品
    private void PlusItem(CommandArgs args)
    {
        var text = GetString("几乎所有的武器和弹药都能强化，但是强化结果会无效化词缀，作为补偿，前三次强化价格降低 80%\n强化绑定一类武器，即同 ID 武器，而不是单独的一个物品。强化与人物绑定，不可分享，扔出即失效，只在背包，猪猪等个人私有库存内起效。\n当你不小心扔出或其他原因导致强化无效，请使用指令 /plus load 来重新获取。每次重新获取都会从当前背包中查找并强制拿出来重给，请注意捡取避免丢失。\n重新获取时重给的物品是单独给予，不会被其他玩家捡走，每次进入服务器时会默认强制重新获取。\n第一个物品栏是强化栏，指令只对该物品栏内的物品起效，强化完即可将武器拿走换至其他栏位，功能类似于哥布林的重铸槽。");
        var text2 = GetString("输入 /plus    查看当前该武器的等级状态和升至下一级需要多少材料\n输入 /plus load    将当前身上所有已升级的武器重新获取\n输入 /plus <damage/da/伤害> <up/down> <num>   升级/降级当前武器的伤害等级\n输入 /plus <scale/sc/大小> <up/down> <num>  升级/降级当前武器或射弹的体积等级 ±5%\n输入 /plus <knockback/kn/击退> <up/down> <num>   升级/降级当前武器的击退等级 ±5%\n输入 /plus <usespeed/us/用速> <up/down> <num>   升级/降级当前武器的使用速度等级\n输入 /plus <shootspeed/sh/飞速> <up/down> <num>   升级/降级当前武器的射弹飞行速度等级，影响鞭类武器范围±5%\n输入 /plus clear    清理当前武器的所有等级，可以回收一点消耗物\n输入 /clearallplayersplus    将数据库中所有玩家的所有强化物品全部清理，管理员专属");
        if (args.Parameters.Count == 1 && args.Parameters[0].Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            if (!args.Player.Active)
            {
                args.Player.SendInfoMessage(text + "\n" + text2);
                return;
            }
            args.Player.SendMessage(text, new Color(255, 128, 255));
            args.Player.SendMessage(text2, this.getRandColor());
            return;
        }
        if (!args.Player.Active)
        {
            args.Player.SendInfoMessage(GetString("该指令必须在游戏内使用"));
            return;
        }
        if (!TShock.ServerSideCharacterConfig.Settings.Enabled)
        {
            args.Player.SendInfoMessage(GetString("SSC 未开启"));
            return;
        }
        var wPlayer = wPlayers[args.Player.Index];
        var firstItem = args.Player.TPlayer.inventory[0];
        var select = wPlayer.hasItems.Find((x) => x.id == firstItem.netID);
        select ??= new WItem(firstItem.netID, args.Player.Name);
        if ((firstItem == null || firstItem.IsAir || TShock.Utils.GetItemById(firstItem.type).damage <= 0 || firstItem.accessory || firstItem.netID == 0) && (args.Parameters.Count != 1 || !args.Parameters[0].Equals("load", StringComparison.OrdinalIgnoreCase)))
        {
            args.Player.SendInfoMessage(GetString("请在第一个物品栏内放入武器而不是其他什么东西或空"));
        }
        else if (args.Parameters.Count == 0)
        {
            args.Player.SendMessage(GetString($"当前物品：[i:{firstItem!.netID}]   共计消耗：{select.allCost}\n{select.ItemMess()}"), this.getRandColor());
        }
        else if (args.Parameters.Count == 1)
        {
            if (args.Parameters[0].Equals("load", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var hasItem in wPlayer.hasItems)
                {
                    ReplaceWeaponsInBackpack(args.Player.TPlayer, hasItem);
                }
                args.Player.SendInfoMessage(GetString("您当前的升级武器已重新读取"));
            }
            else if (args.Parameters[0].Equals("clear", StringComparison.OrdinalIgnoreCase))
            {
                if (select.Level == 0)
                {
                    args.Player.SendInfoMessage(GetString("当前武器没有任何等级，不用回炉重做"));
                    return;
                }
                var num = (long) (select.allCost * config.ResetTheWeaponReturnMultiple);
                EconomicsAPI.Economics.CurrencyManager.AddUserCurrency(args.Player.Name, num, config.Currency);
                wPlayer.hasItems.RemoveAll((x) => x.id == firstItem!.netID);
                DB.DeleteDB(args.Player.Name, firstItem!.netID);
                ReplaceWeaponsInBackpack(args.Player.TPlayer, select, 1);
                args.Player.SendMessage(GetString($"完全重置成功！{config.Currency}回收：{num}"), new Color(0, 255, 0));
            }
            else
            {
                args.Player.SendInfoMessage(GetString("输入 /plus help    查看 plus 系列指令帮助"));
            }
        }
        else if (args.Parameters.Count >= 2 && args.Parameters.Count <= 3)
        {
            var result = 1;
            if (args.Parameters.Count == 3)
            {
                if (!int.TryParse(args.Parameters[2], out result))
                {
                    args.Player.SendInfoMessage(GetString("输入 /plus help    查看 plus 系列指令帮助"));
                    return;
                }
                if (result <= 0)
                {
                    args.Player.SendInfoMessage(GetString("请输入正整数"));
                    return;
                }
            }
            var num2 = 0;
            if (args.Parameters[1].Equals("up", StringComparison.OrdinalIgnoreCase))
            {
                num2 = 1;
            }
            else if (args.Parameters[1].Equals("down", StringComparison.OrdinalIgnoreCase))
            {
                num2 = -1;
            }
            else
            {
                args.Player.SendInfoMessage(GetString("输入 /plus help    查看 plus 系列指令帮助"));
            }
            if (args.Parameters[0].Equals("damage", StringComparison.OrdinalIgnoreCase) || args.Parameters[0].Equals("da", StringComparison.OrdinalIgnoreCase) || args.Parameters[0] == "伤害")
            {
                switch (num2)
                {
                    case 1:
                        if (this.Deduction(select, args.Player.Index, PlusType.damage, result))
                        {
                            select.damage_level += result;
                            if (!wPlayer.hasItems.Exists((x) => x.id == select.id))
                            {
                                wPlayer.hasItems.Add(select);
                            }
                            DB.WriteDB(select);
                            ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                            args.Player.SendMessage(GetString($"[i:{select.id}] 升级成功   共计消耗：{select.allCost}\n{select.ItemMess()}"), this.getRandColor());
                        }
                        break;
                    case -1:
                        if (select.damage_level - result < 0)
                        {
                            args.Player.SendInfoMessage(GetString("等级过低"));
                            break;
                        }
                        select.damage_level -= result;
                        select.CheckDB();
                        DB.WriteDB(select);
                        ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                        args.Player.SendMessage(GetString($"[i:{select.id}]  降级成功  共计消耗：{select.allCost}\n{select.ItemMess()}"), new Color(0, 255, 0));
                        break;
                }
            }
            else if (args.Parameters[0].Equals("scale", StringComparison.OrdinalIgnoreCase) || args.Parameters[0].Equals("sc", StringComparison.OrdinalIgnoreCase) || args.Parameters[0] == "大小")
            {
                switch (num2)
                {
                    case 1:
                        if (this.Deduction(select, args.Player.Index, PlusType.scale, result))
                        {
                            select.scale_level += result;
                            if (!wPlayer.hasItems.Exists((x) => x.id == select.id))
                            {
                                wPlayer.hasItems.Add(select);
                            }
                            DB.WriteDB(select);
                            ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                            args.Player.SendMessage(GetString($"[i:{select.id}] 升级成功   共计消耗：{select.allCost}\n{select.ItemMess()}"), this.getRandColor());
                        }
                        break;
                    case -1:
                        if (select.scale_level - result < 0)
                        {
                            args.Player.SendInfoMessage(GetString("等级过低"));
                            break;
                        }
                        select.scale_level -= result;
                        select.CheckDB();
                        DB.WriteDB(select);
                        ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                        args.Player.SendMessage(GetString($"[i:{select.id}]  降级成功  共计消耗：{select.allCost}\n{select.ItemMess()}"), new Color(0, 255, 0));
                        break;
                }
            }
            else if (args.Parameters[0].Equals("knockBack", StringComparison.OrdinalIgnoreCase) || args.Parameters[0].Equals("kn", StringComparison.OrdinalIgnoreCase) || args.Parameters[0] == "击退")
            {
                switch (num2)
                {
                    case 1:
                        if (this.Deduction(select, args.Player.Index, PlusType.knockBack, result))
                        {
                            select.knockBack_level += result;
                            if (!wPlayer.hasItems.Exists((x) => x.id == select.id))
                            {
                                wPlayer.hasItems.Add(select);
                            }
                            DB.WriteDB(select);
                            ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                            args.Player.SendMessage(GetString($"[i:{select.id}] 升级成功   共计消耗：{select.allCost}\n{select.ItemMess()}"), this.getRandColor());
                        }
                        break;
                    case -1:
                        if (select.knockBack_level - result < 0)
                        {
                            args.Player.SendInfoMessage(GetString("等级过低"));
                            break;
                        }
                        select.knockBack_level -= result;
                        select.CheckDB();
                        DB.WriteDB(select);
                        ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                        args.Player.SendMessage(GetString($"[i:{select.id}]  降级成功  共计消耗：{select.allCost}\n{select.ItemMess()}"), new Color(0, 255, 0));
                        break;
                }
            }
            else if (args.Parameters[0].Equals("useSpeed", StringComparison.OrdinalIgnoreCase) || args.Parameters[0].Equals("us", StringComparison.OrdinalIgnoreCase) || args.Parameters[0] == "用速")
            {
                switch (num2)
                {
                    case 1:
                        if (this.Deduction(select, args.Player.Index, PlusType.useSpeed, result))
                        {
                            select.useSpeed_level += result;
                            if (!wPlayer.hasItems.Exists((x) => x.id == select.id))
                            {
                                wPlayer.hasItems.Add(select);
                            }
                            DB.WriteDB(select);
                            ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                            args.Player.SendMessage(GetString($"[i:{select.id}] 升级成功   共计消耗：{select.allCost}\n{select.ItemMess()}"), this.getRandColor());
                        }
                        break;
                    case -1:
                        if (select.useSpeed_level - result < 0)
                        {
                            args.Player.SendInfoMessage(GetString("等级过低"));
                            break;
                        }
                        select.useSpeed_level -= result;
                        select.CheckDB();
                        DB.WriteDB(select);
                        ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                        args.Player.SendMessage(GetString($"[i:{select.id}]  降级成功  共计消耗：{select.allCost}\n{select.ItemMess()}"), new Color(0, 255, 0));
                        break;
                }
            }
            else if (args.Parameters[0].Equals("shootSpeed", StringComparison.OrdinalIgnoreCase) || args.Parameters[0].Equals("sh", StringComparison.OrdinalIgnoreCase) || args.Parameters[0] == "飞速")
            {
                switch (num2)
                {
                    case 1:
                        if (this.Deduction(select, args.Player.Index, PlusType.shootSpeed, result))
                        {
                            select.shootSpeed_level += result;
                            if (!wPlayer.hasItems.Exists((x) => x.id == select.id))
                            {
                                wPlayer.hasItems.Add(select);
                            }
                            DB.WriteDB(select);
                            ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                            args.Player.SendMessage(GetString($"[i:{select.id}] 升级成功   共计消耗：{select.allCost}\n{select.ItemMess()}"), this.getRandColor());
                        }
                        break;
                    case -1:
                        if (select.shootSpeed_level - result < 0)
                        {
                            args.Player.SendInfoMessage(GetString("等级过低"));
                            break;
                        }
                        select.shootSpeed_level -= result;
                        select.CheckDB();
                        DB.WriteDB(select);
                        ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                        args.Player.SendMessage(GetString($"[i:{select.id}]  降级成功  共计消耗：{select.allCost}\n{select.ItemMess()}"), new Color(0, 255, 0));
                        break;
                }
            }
            else
            {
                args.Player.SendInfoMessage(GetString("输入 /plus help    查看 plus 系列指令帮助"));
            }
        }
        else
        {
            args.Player.SendInfoMessage(GetString("输入 /plus help    查看 plus 系列指令帮助"));
        }
    }
    #endregion

    #region 清理强化物品
    private void ClearPlusItem(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            if (DB.DeleteDBAll())
            {
                var array = wPlayers;
                foreach (var wPlayer in array)
                {
                    if (wPlayer == null || !wPlayer.isActive)
                    {
                        continue;
                    }
                    foreach (var hasItem in wPlayer.hasItems)
                    {
                        ReplaceWeaponsInBackpack(wPlayer.me.TPlayer, hasItem, 1);
                    }
                    wPlayer.hasItems.Clear();
                }
                TSPlayer.All.SendSuccessMessage(GetString("所有玩家的所有强化数据全部清理成功！"));
            }
            else
            {
                args.Player.SendErrorMessage(GetString("强化数据清理失败！！!"));
            }
        }
        else
        {
            args.Player.SendInfoMessage(GetString("输入 /clearallplayersplus   将数据库中强化物品全部清理"));
        }
    }
    #endregion

    #region 新物品
    public static int MyNewItem(IEntitySource source, Vector2 pos, Vector2 randomBox, int Type, int Stack = 1, bool noBroadcast = false, int prefixGiven = 0, bool noGrabDelay = false, bool reverseLookup = false)
    {
        return MyNewItem(source, (int) pos.X, (int) pos.Y, (int) randomBox.X, (int) randomBox.Y, Type, Stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);
    }

    public static int MyNewItem(IEntitySource source, int X, int Y, int Width, int Height, int Type, int Stack = 1, bool noBroadcast = false, int pfix = 0, bool noGrabDelay = false, bool reverseLookup = false)
    {
        if (WorldGen.gen)
        {
            return 0;
        }
        Main.rand ??= new UnifiedRandom();
        if (Main.tenthAnniversaryWorld)
        {
            if (Type == 58)
            {
                Type = Terraria.Utils.NextFromList(Main.rand, new short[3] { 1734, 1867, 58 });
            }
            if (Type == 184)
            {
                Type = Terraria.Utils.NextFromList(Main.rand, new short[3] { 1735, 1868, 184 });
            }
        }
        if (Main.halloween)
        {
            if (Type == 58)
            {
                Type = 1734;
            }
            if (Type == 184)
            {
                Type = 1735;
            }
        }
        if (Main.xMas)
        {
            if (Type == 58)
            {
                Type = 1867;
            }
            if (Type == 184)
            {
                Type = 1868;
            }
        }
        if (Type > 0 && Item.cachedItemSpawnsByType[Type] != -1)
        {
            Item.cachedItemSpawnsByType[Type] += Stack;
            return 400;
        }
        Main.item[400] = new Item();
        var num = 400;
        if (Main.netMode != 1)
        {
            num = Item.PickAnItemSlotToSpawnItemOn(reverseLookup, num);
        }
        Main.timeItemSlotCannotBeReusedFor[num] = 0;
        Main.item[num] = new Item();
        var val = Main.item[num];
        val.SetDefaults(Type);
        val.Prefix(pfix);
        val.stack = Stack;
        val.position.X = X + (Width / 2) - (val.width / 2);
        val.position.Y = Y + (Height / 2) - (val.height / 2);
        val.wet = Collision.WetCollision(val.position, val.width, val.height);
        val.velocity.X = Main.rand.Next(-30, 31) * 0.1f;
        val.velocity.Y = Main.rand.Next(-40, -15) * 0.1f;
        if (Type == 859 || Type == 4743)
        {
            val.velocity *= 0f;
        }
        if (Type == 520 || Type == 521 || (val.type >= 0 && ItemID.Sets.NebulaPickup[val.type]))
        {
            val.velocity.X = Main.rand.Next(-30, 31) * 0.1f;
            val.velocity.Y = Main.rand.Next(-30, 31) * 0.1f;
        }
        val.active = true;
        val.timeSinceItemSpawned = ItemID.Sets.OverflowProtectionTimeOffset[val.type];
        Item.numberOfNewItems++;
        if (ItemSlot.Options.HighlightNewItems && val.type >= 0 && !ItemID.Sets.NeverAppearsAsNewInInventory[val.type])
        {
            val.newAndShiny = true;
        }
        else if (Main.netMode == 0)
        {
            val.playerIndexTheItemIsReservedFor = Main.myPlayer;
        }
        return num;
    }
    #endregion

    #region 更换背包中的武器
    public static void ReplaceWeaponsInBackpack(Player? player, WItem? item, int model = 0)
    {
        if (player == null || !player.active || item == null)
        {
            return;
        }
        var whoAmI = player.whoAmI;
        for (var i = 0; i < NetItem.InventoryIndex.Item2; i++)
        {
            if (player.inventory[i].netID == item.id)
            {
                var stack = player.inventory[i].stack;
                var prefix = player.inventory[i].prefix;
                player.inventory[i].TurnToAir(false);
                TShock.Players[whoAmI].SendData((PacketTypes) 5, "", whoAmI, i);
                switch (model)
                {
                    case 0:
                    {
                        var num2 = MyNewItem(null!, player.Center, new Vector2(1f, 1f), item.id, stack);
                        Main.item[num2].playerIndexTheItemIsReservedFor = whoAmI;
                        Main.item[num2].prefix = prefix;
                        var num3 = (int) (item.orig_damage * 0.05f * item.damage_level);
                        num3 = (num3 < item.damage_level) ? item.damage_level : num3;
                        var obj = Main.item[num2];
                        obj.damage += num3;
                        var obj2 = Main.item[num2];
                        obj2.scale += item.orig_scale * 0.05f * item.scale_level;
                        var obj3 = Main.item[num2];
                        obj3.knockBack += item.orig_knockBack * 0.05f * item.knockBack_level;
                        Main.item[num2].useAnimation = item.orig_useAnimation - item.useSpeed_level;
                        Main.item[num2].useTime = (int) (item.orig_useTime * 1f / item.orig_useAnimation * Main.item[num2].useAnimation);
                        var obj4 = Main.item[num2];
                        obj4.shootSpeed += item.orig_shootSpeed * 0.05f * item.shootSpeed_level;
                        TShock.Players[whoAmI].SendData((PacketTypes) 21, null, num2);
                        TShock.Players[whoAmI].SendData((PacketTypes) 22, null, num2);
                        TShock.Players[whoAmI].SendData((PacketTypes) 88, null, num2, 255f, 63f);
                        break;
                    }
                    case 1:
                    {
                        var num = MyNewItem(null!, player.Center, new Vector2(1f, 1f), item.id, stack);
                        Main.item[num].playerIndexTheItemIsReservedFor = whoAmI;
                        Main.item[num].prefix = prefix;
                        TShock.Players[whoAmI].SendData((PacketTypes) 21, null, num);
                        TShock.Players[whoAmI].SendData((PacketTypes) 22, null, num);
                        break;
                    }
                }
            }
        }
    }
    #endregion

    #region 扣钱的方法
    public bool Deduction(WItem WItem, int whoAMI, PlusType plusType, int gap = 1)
    {
        var name = TShock.Players[whoAMI].Name;
        if (!WItem.plusPrice(plusType, out var price, gap))
        {
            TShock.Players[whoAMI].SendMessage(GetString("当前该类型升级已达到上限，无法升级"), Color.Red);
            return false;
        }
        if (EconomicsAPI.Economics.CurrencyManager.DeductUserCurrency(name, price, config.Currency))
        {
            WItem.allCost += price;
            TShock.Players[whoAMI].SendMessage(GetString($"扣除{config.Currency}：{price}，当前剩余：{EconomicsAPI.Economics.CurrencyManager.GetUserCurrency(name, config.Currency).Number}"), new Color(99, 106, 255));
            return true;
        }
        TShock.Players[whoAMI].SendInfoMessage(GetString($"{config.Currency}不足！"));
        return false;
    }
    #endregion

    #region 提示语的随机颜色方法
    public Color getRandColor()
    {
        return new Color(Main.rand.Next(60, 255), Main.rand.Next(60, 255), Main.rand.Next(60, 255));
    }
    #endregion
}