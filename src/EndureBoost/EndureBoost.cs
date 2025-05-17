using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Plugin;

[ApiVersion(2, 1)]
public class EndureBoost : TerrariaPlugin
{
    public static Configuration Config = null!;

    public override string Author => "肝帝熙恩";

    public override string Description => GetString("拥有指定数量物品给指定buff");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 7);

    public EndureBoost(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += ReloadConfig;
        ServerApi.Hooks.ServerJoin.Register(this, this.OnServerJoin);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        GetDataHandlers.PlayerSpawn.Register(this.Rebirth);
        Commands.ChatCommands.Add(new Command("EndureBoost", this.SetPlayerBuffcmd, "ebbuff", "ldbuff", "loadbuff", "刷新buff"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnServerJoin);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            GetDataHandlers.PlayerSpawn.UnRegister(this.Rebirth);
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.SetPlayerBuffcmd);
        }
        base.Dispose(disposing);
    }

    private int Timer = 0;

    private void OnUpdate(EventArgs args)
    {
        if (Config.UpdateFrequency == -1)
        {
            return;
        }
        var updateFrequencyInSeconds = Config.UpdateFrequency;
        var updateInterval = 60 * updateFrequencyInSeconds;

        // 当计数器达到更新间隔时更新玩家状态
        if (this.Timer % updateInterval == 0)
        {
            foreach (var player in TShock.Players)
            {
                if (player != null)
                {
                    this.SetPlayerBuff(player);
                }
            }
        }
        this.Timer++;
    }
    private void Rebirth(object? sender, GetDataHandlers.SpawnEventArgs args)// 重生后刷新buff
    {
        if (args.Player == null)
        {
            return;
        }
        var player = args.Player;
        this.SetPlayerBuff(player);
    }

    private void SetPlayerBuffcmd(CommandArgs args)// 通过指令立即刷新buff
    {
        var player = args.Player;
        player.SendSuccessMessage(GetString("[拥有指定数量物品给指定buff] 刷新buff完毕。"));
        this.SetPlayerBuff(player);
    }

    #region 配置
    private static void LoadConfig()
    {
        Config = Configuration.Read(Configuration.FilePath);
        Config.Write(Configuration.FilePath);
    }

    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player.SendSuccessMessage(GetString("[拥有指定数量物品给指定buff] 重新加载配置完毕。"));
    }
    #endregion

    private void OnServerJoin(JoinEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
        {
            return;
        }
        this.SetPlayerBuff(player);
    }

    private void SetPlayerBuff(TSPlayer player)
    {
        // 处理 potions（药水）
        foreach (var potion in Config.Potions)
        {
            foreach (var itemId in potion.PotionID)
            {
                var itemCount = 0;

                // 检查背包中的物品
                for (var i = 0; i < 58; i++)
                {
                    if (player.TPlayer.inventory[i].type == itemId)
                    {
                        itemCount += player.TPlayer.inventory[i].stack;
                    }
                }

                // 检查不同存储区中的物品
                this.CheckBanksForItem(player, itemId, ref itemCount);

                if (itemCount >= potion.RequiredStack)
                {
                    var buffType = this.GetBuffIDByItemID(itemId); // 获取物品的 buff 类型
                    if (buffType != 0)
                    {
                        player.SetBuff(buffType, Config.duration * 60);
                    }
                }
            }
        }

        // 处理 inventory （背包）
        foreach (var inventory in Config.Inventorys)
        {
            var isAnyItemValid = false; // 任意一个物品满足条件
            var areAllItemsValid = true; // 所有物品都满足条件

            foreach (var itemId in inventory.ItemID)
            {
                var itemCount = 0;

                // 检查背包中的物品
                for (var i = 0; i < 58; i++)
                {
                    if (player.TPlayer.inventory[i].type == itemId)
                    {
                        itemCount += player.TPlayer.inventory[i].stack;
                    }
                }

                // 检查不同存储区中的物品
                this.CheckBanksForItem(player, itemId, ref itemCount);


                if (inventory.Trigger)
                {
                    if (itemCount < inventory.RequiredStack)
                    {
                        areAllItemsValid = false; // 有物品不满足条件
                        break; // 跳出循环
                    }
                }
                else
                {
                    if (itemCount >= inventory.RequiredStack)
                    {
                        isAnyItemValid = true; // 任意一个物品满足条件
                    }
                }
            }


            if ((inventory.Trigger && areAllItemsValid) || (!inventory.Trigger && isAnyItemValid))
            {
                if (inventory.BuffType != null)
                {
                    foreach (var buffId in inventory.BuffType)
                    {
                        player.SetBuff(buffId, Config.duration * 60);
                    }
                }
            }
        }

        // 处理 accessories（配饰）
        foreach (var accessorie in Config.Accessories)
        {
            var isAnyItemValid = false; // 任意一个物品满足条件
            var areAllItemsValid = true; // 所有物品都满足条件

            foreach (var accessorieId in accessorie.AccessorieID)
            {
                var itemCount = 0;

                // 检查当前装备栏
                var currentLoadout = player.TPlayer.armor;

                // 检查当前装备栏中的物品

                for (var i = 0; i < currentLoadout.Length; i++)
                {
                    // 仅在允许社交栏或当前索引处于普通装备栏时，才进行计数
                    if (accessorie.AllowSocialSlot || i <= 9)
                    {
                        if (currentLoadout[i].type == accessorieId)
                        {
                            itemCount += currentLoadout[i].stack;
                        }
                    }
                }


                // 如果允许非本装备栏也触发，检查所有其他装备栏中的物品
                if (accessorie.AllowOtherLoadouts)
                {
                    for (var loadoutIndex = 0; loadoutIndex < 3; loadoutIndex++) // 3套装备栏
                    {
                        var armor = player.TPlayer.Loadouts[loadoutIndex].Armor;
                        // 检查当前装备栏中的物品
                        for (var i = 0; i < currentLoadout.Length; i++)
                        {
                            if (accessorie.AllowSocialSlot || i <= 9)
                            {
                                if (armor[i].type == accessorieId)
                                {
                                    itemCount += armor[i].stack;
                                }
                            }
                        }
                    }
                }

                // 根据 Trigger 的值进行判断
                if (accessorie.Trigger)
                {
                    if (itemCount == 0) // 如果没有找到相应的配饰
                    {
                        areAllItemsValid = false; // 有物品不满足条件
                        break; // 跳出循环
                    }
                }
                else
                {
                    if (itemCount > 0)
                    {
                        isAnyItemValid = true; // 任意一个物品满足条件
                    }
                }
            }

            // 根据 Trigger 的状态给予 Buff
            if ((accessorie.Trigger && areAllItemsValid) || (!accessorie.Trigger && isAnyItemValid))
            {
                if (accessorie.BuffType != null)
                {
                    foreach (var buffId in accessorie.BuffType)
                    {
                        player.SetBuff(buffId, Config.duration * 60);
                    }
                }
            }
        }

        // 处理 dyes（染料）
        foreach (var dye in Config.Dyes)
        {
            var isAnyItemValid = false; // 任意一个物品满足条件
            var areAllItemsValid = true; // 所有物品都满足条件

            foreach (var dyeId in dye.DyeID)
            {
                var itemCount = 0;

                // 检查当前装备栏中的染料
                var currentLoadout = player.TPlayer.dye;

                // 检查当前装备栏中的物品
                for (var i = 0; i < currentLoadout.Length; i++)
                {
                    if (currentLoadout[i].type == dyeId)
                    {
                        itemCount += 1; // 染料每个格子固定只能装一个，计数为 1
                    }
                }

                // 如果允许非本装备栏也触发，检查所有其他装备栏中的物品
                if (dye.AllowOtherLoadouts)
                {
                    for (var loadoutIndex = 0; loadoutIndex < 3; loadoutIndex++) // 3套装备栏
                    {
                        var dyeArray = player.TPlayer.Loadouts[loadoutIndex].Dye;
                        // 检查当前装备栏中的物品
                        for (var i = 0; i < dyeArray.Length; i++)
                        {
                            if (dyeArray[i].type == dyeId)
                            {
                                itemCount += 1; // 染料每个格子固定只能装一个，计数为 1
                            }
                        }
                    }
                }

                // 根据 Trigger 的值进行判断
                if (dye.Trigger)
                {
                    if (itemCount == 0) // 如果没有找到相应的染料
                    {
                        areAllItemsValid = false; // 有物品不满足条件
                        break; // 跳出循环
                    }
                }
                else
                {
                    if (itemCount > 0)
                    {
                        isAnyItemValid = true; // 任意一个物品满足条件
                    }
                }
            }

            // 根据 Trigger 的状态给予 Buff
            if ((dye.Trigger && areAllItemsValid) || (!dye.Trigger && isAnyItemValid))
            {
                if (dye.BuffType != null)
                {
                    foreach (var buffId in dye.BuffType)
                    {
                        player.SetBuff(buffId, Config.duration * 60);
                    }
                }
            }
        }

        // 处理 全部物品
        foreach (var custom in Config.Customs)
        {
            var isAnyItemValid = false; // 任意一个物品满足条件
            var areAllItemsValid = true; // 所有物品都满足条件

            foreach (var itemId in custom.CustomItemID)
            {
                var itemCount = 0;
                var plr = player.TPlayer;
                for (var i = 0; i < 350; i++)
                {
                    // 背包 (0-49: 背包 ; 50-53: 钱币 ; 54-58: 弹药)
                    if (i < 59)
                    {
                        if (plr.inventory[i].type == itemId)
                        {
                            itemCount += plr.inventory[i].stack;
                        }
                    }
                    // 装备栏 (59-68: 盔甲 ; 69-78: 时装)
                    else if (i >= 59 && i < 79)
                    {
                        if (plr.armor[i - 59].type == itemId)
                        {
                            itemCount += plr.armor[i - 59].stack;
                        }
                    }
                    // 染料栏 (79-88)
                    else if (i >= 79 && i < 89)
                    {
                        if (plr.dye[i - 79].type == itemId)
                        {
                            itemCount += plr.dye[i - 79].stack;
                        }
                    }
                    // Misc装备栏 (89-93) 和Misc染料栏 (94-98)
                    else if (i >= 89 && i < 94)
                    {
                        if (plr.miscEquips[i - 89].type == itemId)
                        {
                            itemCount += plr.miscEquips[i - 89].stack;
                        }
                    }
                    else if (i >= 94 && i < 99)
                    {
                        if (plr.miscDyes[i - 94].type == itemId)
                        {
                            itemCount += plr.miscDyes[i - 94].stack;
                        }
                    }
                    // 银行 (99-138)
                    else if (i >= 99 && i < 139)
                    {
                        if (plr.bank.item[i - 99].type == itemId)
                        {
                            itemCount += plr.bank.item[i - 99].stack;
                        }
                    }
                    // 银行2 (139-178)
                    else if (i >= 139 && i < 179)
                    {
                        if (plr.bank2.item[i - 139].type == itemId)
                        {
                            itemCount += plr.bank2.item[i - 139].stack;
                        }
                    }
                    // 银行3 (180-219)
                    else if (i >= 180 && i < 220)
                    {
                        if (plr.bank3.item[i - 180].type == itemId)
                        {
                            itemCount += plr.bank3.item[i - 180].stack;
                        }
                    }
                    // 银行4 (220-259)
                    else if (i >= 220 && i < 260)
                    {
                        if (plr.bank4.item[i - 220].type == itemId)
                        {
                            itemCount += plr.bank4.item[i - 220].stack;
                        }
                    }
                    // 第一套装备栏的盔甲和社交栏 (260-279) 和染料 (280-289)
                    else if (i >= 260 && i < 280)
                    {
                        if (plr.Loadouts[0].Armor[i - 260].type == itemId)
                        {
                            itemCount += plr.Loadouts[0].Armor[i - 260].stack;
                        }
                    }
                    else if (i >= 280 && i < 290)
                    {
                        if (plr.Loadouts[0].Dye[i - 280].type == itemId)
                        {
                            itemCount += plr.Loadouts[0].Dye[i - 280].stack;
                        }
                    }
                    // 第二套装备栏的盔甲和社交栏 (290-309) 和染料 (310-319)
                    else if (i >= 290 && i < 310)
                    {
                        if (plr.Loadouts[1].Armor[i - 290].type == itemId)
                        {
                            itemCount += plr.Loadouts[1].Armor[i - 290].stack;
                        }
                    }
                    else if (i >= 310 && i < 320)
                    {
                        if (plr.Loadouts[1].Dye[i - 310].type == itemId)
                        {
                            itemCount += plr.Loadouts[1].Dye[i - 310].stack;
                        }
                    }
                    // 第三套装备栏的盔甲和社交栏 (320-339) 和染料 (340-349)
                    else if (i >= 320 && i < 340)
                    {
                        if (plr.Loadouts[2].Armor[i - 320].type == itemId)
                        {
                            itemCount += plr.Loadouts[2].Armor[i - 320].stack;
                        }
                    }
                    else if (i >= 340 && i < 350)
                    {
                        if (plr.Loadouts[2].Dye[i - 340].type == itemId)
                        {
                            itemCount += plr.Loadouts[2].Dye[i - 340].stack;
                        }
                    }
                }


                // 根据 Trigger 的值进行判断
                if (custom.Trigger)
                {
                    if (itemCount < custom.RequiredStack) // 如果数量不满足
                    {
                        areAllItemsValid = false; // 有物品不满足条件
                        break; // 跳出循环
                    }
                }
                else
                {
                    if (itemCount >= custom.RequiredStack) // 如果有足够数量
                    {
                        isAnyItemValid = true; // 任意一个物品满足条件
                    }
                }
            }

            // 根据 Trigger 的状态给予 Buff
            if ((custom.Trigger && areAllItemsValid) || (!custom.Trigger && isAnyItemValid))
            {
                if (custom.BuffType != null)
                {
                    foreach (var buffId in custom.BuffType)
                    {
                        player.SetBuff(buffId, Config.duration * 60);
                    }
                }
            }
        }
    }

    private void CheckBanksForItem(TSPlayer player, int itemId, ref int itemCount)
    {
        for (var j = 0; j < 40; j++)
        {
            if (player.TPlayer.bank.item[j].type == itemId && Config.bank)// 检查猪猪储钱罐
            {
                itemCount += player.TPlayer.bank.item[j].stack;
            }
            if (player.TPlayer.bank2.item[j].type == itemId && Config.bank2)// 检查保险箱
            {
                itemCount += player.TPlayer.bank2.item[j].stack;
            }
            if (player.TPlayer.bank3.item[j].type == itemId && Config.bank3)// 检查护卫熔炉
            {
                itemCount += player.TPlayer.bank3.item[j].stack;
            }
            if (player.TPlayer.bank4.item[j].type == itemId && Config.bank4)// 检查虚空宝藏袋
            {
                itemCount += player.TPlayer.bank4.item[j].stack;
            }
        }
    }

    private int GetBuffIDByItemID(int itemId)
    {
        var item = new Item();
        item.SetDefaults(itemId);
        return item.buffType;
    }
}