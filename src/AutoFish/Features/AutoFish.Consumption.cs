using System.Text;
using Terraria.ID;
using TShockAPI;

namespace AutoFish;

public partial class Plugin
{
    private static bool CanConsumeFish(TSPlayer player, AFPlayerData.ItemData playerData)
    {
        if (playerData.CanConsume())
        {
            return true;
        }
        if (ConsumeBaitAndEnableMode(player, playerData))
        {
            return true;
        }
        ExitTip(player, playerData);
        return false;
    }

    /// <summary>
    ///     消耗鱼饵并启用消耗模式。
    /// </summary>
    private static bool ConsumeBaitAndEnableMode(TSPlayer player, AFPlayerData.ItemData playerData)
    {
        if (Configuration.Instance.BaitRewards.Count == 0)
        {
            return false;
        }
        var availableBaits = new List<(int itemId, int slot, int stack, Configuration.BaitReward reward)>();
        for (var i = 0; i < player.TPlayer.inventory.Length; i++)
        {
            var slot = player.TPlayer.inventory[i];
            if (slot.type == player.TPlayer.inventory[player.TPlayer.selectedItem].type)
            {
                continue;
            }

            if (Configuration.Instance.BaitRewards.TryGetValue(slot.type, out var reward))
            {
                if (slot.stack >= reward.Count)
                {
                    availableBaits.Add((slot.type, i, slot.stack, reward));
                }
            }
        }

        if (availableBaits.Count == 0)
        {
            return false;
        }

        var bestBait = availableBaits.OrderByDescending(b => b.reward.Minutes).First();
        var consumedCount = bestBait.reward.Count;
        var rewardMinutes = bestBait.reward.Minutes;

        var inventorySlot = player.TPlayer.inventory[bestBait.slot];
        inventorySlot.stack -= consumedCount;

        if (inventorySlot.stack < 1)
        {
            inventorySlot.TurnToAir();
        }

        player.SendData(PacketTypes.PlayerSlot, "", player.Index, PlayerItemSlotID.Inventory0 + bestBait.slot);
        playerData.ConsumeOverTime = DateTime.Now.AddMinutes(rewardMinutes);
        player.SendMessage(GetString($"玩家 [c/46C2D4:{player.Name}] 已开启[c/F5F251:自动钓鱼] 消耗物品为:[i:{bestBait.itemId}x{consumedCount}]"), 247, 244, 150);

        if (playerData.ConsumeStartTime == default)
        {
            playerData.ConsumeStartTime = DateTime.Now;
        }

        return true;
    }

    /// <summary>
    ///     消耗模式下检测超时并关闭自动钓鱼权限。
    /// </summary>
    private static void ExitTip(TSPlayer player, AFPlayerData.ItemData playerData)
    {
        if (playerData.ConsumeStartTime == default)
        {
            return;
        }

        var expiredMessage = new StringBuilder();
        expiredMessage.AppendLine(GetString("[i:3455][c/AD89D5:自][c/D68ACA:动][c/DF909A:钓][c/E5A894:鱼][i:3454]"));
        var timeElapsed = DateTime.Now - playerData.ConsumeStartTime;
        var minutes = (int) timeElapsed.TotalMinutes;
        var seconds = timeElapsed.Seconds;
        playerData.ConsumeStartTime = default;

        expiredMessage.AppendLine(GetString("以下玩家已关闭[c/76D5B4:自动钓鱼]权限："));
        expiredMessage.AppendFormat(GetString("[c/A7DDF0:{0}]:[c/74F3C9:{1}分{2}秒]"), playerData.Name, minutes, seconds);

        player.SendMessage(expiredMessage.ToString(), 247, 244, 150);
    }
}