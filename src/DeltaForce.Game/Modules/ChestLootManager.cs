using DeltaForce.Protocol.Packets;
using LazyAPI.Utility;
using Terraria;
using Terraria.ID;

namespace DeltaForce.Game.Modules;

public class ChestLootManager
{
    private static List<ItemInfo>? _availableItems;
    private static readonly Random _random = new();

    public static void SetAvailableItems(ItemInfo[] items)
    {
        _availableItems = [.. items];
    }

    public static async Task RequestAndDistributeItemsAsync()
    {
        try
        {
            var response = await CommunicationManager.GetItemList();
            if (response?.Success == true && response.Items.Length > 0)
            {
                SetAvailableItems(response.Items);
                DistributeItemsToAllChests();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(GetString($"[ChestLootManager] 请求物品列表失败: {ex.Message}"));
        }
    }

    public static void DistributeItemsToAllChests()
    {
        if (_availableItems == null || _availableItems.Count == 0)
        {
            Console.WriteLine(GetString("[ChestLootManager] 没有可用的物品配置"));
            return;
        }

        int chestCount = 0;
        for (int i = 0; i < Main.chest.Length; i++)
        {
            var chest = Main.chest[i];
            if (chest == null) continue;

            if (IsValidLootChest(chest))
            {
                ClearChest(chest);
                FillChestWithRandomItems(chest);
                chestCount++;
            }
        }

        Console.WriteLine(GetString($"[ChestLootManager] 已重置 {chestCount} 个箱子的物品"));
    }

    private static bool IsValidLootChest(Chest chest)
    {
        int tileX = chest.x;
        int tileY = chest.y;

        if (tileX < 0 || tileX >= Main.maxTilesX || tileY < 0 || tileY >= Main.maxTilesY)
            return false;

        var tile = Main.tile[tileX, tileY];
        if (tile == null) return false;

        return tile.type == TileID.Containers ||
               tile.type == TileID.Containers2;
    }

    private static void ClearChest(Chest chest)
    {
        for (int i = 0; i < chest.item.Length; i++)
        {
            if (!chest.item[i].IsAir)
            {
                chest.item[i].TurnToAir();
                NetMessage.SendData(32, -1, -1, null, chest.index, i);
            }
        }
    }

    private static void FillChestWithRandomItems(Chest chest)
    {
        int itemCount = _random.Next(1, 7);

        for (int i = 0; i < itemCount; i++)
        {
            var selectedItem = SelectItemByWeight();
            if (selectedItem != null)
            {
                int slot = FindEmptySlot(chest);
                if (slot >= 0)
                {
                    chest.item[slot].SetDefaults(selectedItem.Type);
                    chest.item[slot].stack = GetRandomStackSize(selectedItem.Type);
                    NetMessage.SendData(32, -1, -1, null, chest.index, slot);
                }
            }
        }
    }

    private static ItemInfo? SelectItemByWeight()
    {
        if (_availableItems == null || _availableItems.Count == 0)
            return null;

        int totalWeight = _availableItems.Sum(item => item.Weight);
        if (totalWeight <= 0)
            return _availableItems[_random.Next(_availableItems.Count)];

        int randomValue = _random.Next(totalWeight);
        int currentWeight = 0;

        foreach (var item in _availableItems)
        {
            currentWeight += item.Weight;
            if (randomValue < currentWeight)
            {
                return item;
            }
        }

        return _availableItems.Last();
    }

    private static int FindEmptySlot(Chest chest)
    {
        for (int i = 0; i < chest.item.Length; i++)
        {
            if (chest.item[i].IsAir)
            {
                return i;
            }
        }
        return -1;
    }

    private static int GetRandomStackSize(int itemType)
    {
        var item = new Item();
        item.SetDefaults(itemType);

        if (item.maxStack <= 1)
            return 1;

        if (item.maxStack <= 10)
            return _random.Next(1, item.maxStack + 1);

        return _random.Next(1, Math.Min(3, item.maxStack + 1));
    }
}
