using System.Text;
using AutoFish.Data;
using Terraria.ID;
using TShockAPI;

namespace AutoFish.AFMain;

public partial class AutoFish
{
    private static bool CanConsumeFish(TSPlayer player, AFPlayerData.ItemData playerData)
    {
        if (playerData.CanConsume()) return true;
        //没有，尝试续费
        if (ConsumeBaitAndEnableMode(player, playerData)) return true;

        //续费不了
        ExitTip(player, playerData);
        return false;
    }

    /// <summary>
    ///     消耗鱼饵并启用消耗模式。
    /// </summary>
    private static bool ConsumeBaitAndEnableMode(TSPlayer player, AFPlayerData.ItemData playerData)
    {
        if (!Config.BaitRewards.Any()) return false;

        // 查找背包中可以消耗的鱼饵（优先选择能兑换时长最多的）
        var availableBaits = new List<(int itemId, int slot, int stack, BaitReward reward)>();

        for (var i = 0; i < player.TPlayer.inventory.Length; i++)
        {
            var slot = player.TPlayer.inventory[i];
            if (slot.type == player.TPlayer.inventory[player.TPlayer.selectedItem].type) continue; // 跳过手持物品

            if (Config.BaitRewards.TryGetValue(slot.type, out var reward))
                if (slot.stack >= reward.Count)
                    availableBaits.Add((slot.type, i, slot.stack, reward));
        }

        if (!availableBaits.Any()) return false;

        // 选择能兑换最长时间的鱼饵
        var bestBait = availableBaits.OrderByDescending(b => b.reward.Minutes).First();
        var consumedCount = bestBait.reward.Count;
        var rewardMinutes = bestBait.reward.Minutes;

        // 扣除鱼饵
        var inventorySlot = player.TPlayer.inventory[bestBait.slot];
        inventorySlot.stack -= consumedCount;

        if (inventorySlot.stack < 1)
            inventorySlot.TurnToAir();

        // 同步背包
        player.SendData(PacketTypes.PlayerSlot, "", player.Index, PlayerItemSlotID.Inventory0 + bestBait.slot);

        // 更新时间
        playerData.ConsumeOverTime = DateTime.Now.AddMinutes(rewardMinutes);

        // 构建消耗信息
        var itemName = TShock.Utils.GetItemById(bestBait.itemId).Name;
        var consumedMessage = $" [c/F25156:{itemName}]([c/AECDD1:{consumedCount}])";

        player.SendMessage(Lang.T("consumption.enabled", player.Name, consumedMessage), 247, 244, 150);

        if (playerData.ConsumeStartTime == default)
            playerData.ConsumeStartTime = DateTime.Now;

        return true;
    }

    /// <summary>
    ///     消耗模式下检测超时并关闭自动钓鱼权限。
    /// </summary>
    private static void ExitTip(TSPlayer player, AFPlayerData.ItemData playerData)
    {
        //没开启过 不要提示
        if (playerData.ConsumeStartTime == default) return;

        var expiredMessage = new StringBuilder();
        expiredMessage.AppendLine(Lang.T("consumption.expired.title"));

        // 计算经过的时间（分和秒）
        var timeElapsed = DateTime.Now - playerData.ConsumeStartTime;
        var minutes = (int)timeElapsed.TotalMinutes;
        var seconds = timeElapsed.Seconds;
        playerData.ConsumeStartTime = default;

        expiredMessage.AppendLine(Lang.T("consumption.expired.body"));
        expiredMessage.AppendFormat(Lang.T("consumption.expired.line"), playerData.Name,
            minutes, seconds);

        player.SendMessage(expiredMessage.ToString(), 247, 244, 150);
    }
}