using System.Text;
using AutoFish.Data;
using Terraria.ID;
using TShockAPI;

namespace AutoFish.AFMain;

public partial class AutoFish
{
    /// <summary>需要关闭钓鱼权限的玩家计数。</summary>
    private static int ClearCount; //需要关闭钓鱼权限的玩家计数

    /// <summary>
    ///     消耗模式下根据玩家物品开启或关闭自动钓鱼。
    /// </summary>
    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs args)
    {
        var player = args.Player;
        if (!Config.PluginEnabled) return;
        if (!Config.GlobalConsumptionModeEnabled) return;
        if (player == null) return;
        if (!player.IsLoggedIn) return;
        if (!player.Active) return;

        var playerData = PlayerData.GetOrCreatePlayerData(player.Name, CreateDefaultPlayerData);
        if (!playerData.AutoFishEnabled) return;

        // 播报玩家消耗鱼饵用的
        var consumedItemsMessage = new StringBuilder();
        if (playerData.ConsumptionEnabled) //当消费模式开关已开启时
        {
            //由它判断关闭自动钓鱼
            ExitMod(player, playerData);
            return;
        }

        //当玩家的自动钓鱼没开启时
        //初始化一个消耗值
        var requiredBait = Config.BaitConsumeCount;

        // 统计背包中指定鱼饵的总数量(不包含手上物品)
        var totalBait = player.TPlayer.inventory.Sum(slot =>
            Config.BaitItemIds.Contains(slot.type) &&
            slot.type != player.TPlayer.inventory[player.TPlayer.selectedItem].type
                ? slot.stack
                : 0);

        // 如果背包中有足够的鱼饵数量 和消耗值相等
        if (totalBait < requiredBait) return;
        // 遍历背包58格
        for (var i = 0; i < player.TPlayer.inventory.Length && requiredBait > 0; i++)
        {
            var inventorySlot = player.TPlayer.inventory[i];

            // 是Config里指定的鱼饵,不是手上的物品
            if (!Config.BaitItemIds.Contains(inventorySlot.type)) continue;
            var consumedCount = Math.Min(requiredBait, inventorySlot.stack); // 计算需要消耗的鱼饵数量

            inventorySlot.stack -= consumedCount; // 从背包中扣除鱼饵
            requiredBait -= consumedCount; // 减少消耗值

            // 记录消耗的鱼饵数量到播报
            consumedItemsMessage.AppendFormat(" [c/F25156:{0}]([c/AECDD1:{1}]) ",
                TShock.Utils.GetItemById(inventorySlot.type).Name,
                consumedCount);

            // 如果背包中的鱼饵数量为0，清空该格子
            if (inventorySlot.stack < 1) inventorySlot.TurnToAir();

            // 发包给背包里对应格子的鱼饵
            player.SendData(PacketTypes.PlayerSlot, "", player.Index, PlayerItemSlotID.Inventory0 + i);
        }

        // 消耗值清空时，开启自动钓鱼开关
        if (requiredBait > 0) return;
        playerData.ConsumptionEnabled = true;
        playerData.LogTime = DateTime.Now;
        player.SendMessage(Lang.T("consumption.enabled", player.Name, consumedItemsMessage.ToString()), 247, 244,
            150);
    }

    /// <summary>
    ///     消耗模式下检测超时并关闭自动钓鱼权限。
    /// </summary>
    private static void ExitMod(TSPlayer player, AFPlayerData.ItemData playerData)
    {
        var expiredMessage = new StringBuilder();
        expiredMessage.AppendLine(Lang.T("consumption.expired.title"));
        expiredMessage.AppendLine(Lang.T("consumption.expired.body", Config.RewardDurationMinutes));

        // 只显示分钟
        var minutesElapsed = (DateTime.Now - playerData.LogTime).TotalMinutes;

        // 时间过期 关闭自动钓鱼权限
        if (minutesElapsed >= Config.RewardDurationMinutes)
        {
            ClearCount++;
            playerData.ConsumptionEnabled = false;
            playerData.LogTime = default; // 清空记录时间
            expiredMessage.AppendFormat(Lang.T("consumption.expired.line"), playerData.Name,
                Math.Floor(minutesElapsed));
        }

        // 确保有一个玩家计数，只播报一次
        if (ClearCount <= 0 || expiredMessage.Length <= 0) return;
        player.SendMessage(expiredMessage.ToString(), 247, 244, 150);
        ClearCount = 0;
    }
}