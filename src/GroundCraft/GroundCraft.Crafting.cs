using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace GroundCraft;

public sealed partial class GroundCraft
{
    private void OnGameUpdate(EventArgs args)
    {
        try
        {
            lock (_stateLock)
            {
                if (!_config.Enabled)
                    return;

                _ticks++;
                if (_ticks % Math.Max(1, _config.ScanIntervalTicks) != 0)
                    return;

                ScanDrops();
            }
        }
        catch (Exception ex)
        {
            _runtime.Errors++;
            if (_runtime.Errors <= 5 || _runtime.Errors % 25 == 0)
                TShock.Log.ConsoleError(GetString($"[GroundCraft] 扫描掉落物失败：{ex}"));
        }
    }

    private void ScanDrops()
    {
        _runtime.Scans++;

        List<DropRef> drops = CollectStableDrops();
        if (drops.Count == 0)
            return;

        bool[] used = new bool[drops.Count];
        for (int i = 0; i < drops.Count; i++)
        {
            if (used[i])
                continue;

            List<DropRef> cluster = BuildCluster(drops, used, i);
            if (cluster.Count == 0)
                continue;

            _runtime.Clusters++;
            if (!TryCraftCluster(cluster))
                _runtime.NoMatches++;
        }
    }

    private List<DropRef> CollectStableDrops()
    {
        List<DropRef> drops = new();
        int max = Math.Min(Main.maxItems, Main.item.Length);

        for (int i = 0; i < max; i++)
        {
            WorldItem item = Main.item[i];
            if (!IsUsableDrop(item))
            {
                _stableScans.Remove(i);
                continue;
            }

            if (!IsStill(item))
            {
                _stableScans[i] = 0;
                continue;
            }

            int stable = _stableScans.TryGetValue(i, out int previous) ? previous + 1 : 1;
            _stableScans[i] = stable;
            if (stable < Math.Max(1, _config.RequiredStableScans))
                continue;

            Vector2 center = item.position + new Vector2(item.width / 2f, item.height / 2f);
            drops.Add(new DropRef(i, item, center));
        }

        return drops;
    }

    private List<DropRef> BuildCluster(IReadOnlyList<DropRef> drops, bool[] used, int seedIndex)
    {
        List<DropRef> cluster = new();
        Queue<int> queue = new();
        float radiusPixels = Math.Max(0.5f, _config.ClusterRadiusTiles) * 16f;
        float radiusSquared = radiusPixels * radiusPixels;

        used[seedIndex] = true;
        queue.Enqueue(seedIndex);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            DropRef currentDrop = drops[current];
            cluster.Add(currentDrop);

            for (int i = 0; i < drops.Count; i++)
            {
                if (used[i])
                    continue;

                if (Vector2.DistanceSquared(currentDrop.Center, drops[i].Center) > radiusSquared)
                    continue;

                used[i] = true;
                queue.Enqueue(i);
            }
        }

        return cluster;
    }

    private bool TryCraftCluster(List<DropRef> cluster)
    {
        Vector2 center = AverageCenter(cluster);
        EnvironmentSnapshot snapshot = ProbeEnvironment(center);
        bool craftedAny = false;
        int maxRecipePasses = Math.Max(1, _recipes.Count);
        HashSet<string> craftedRecipeSignatures = new(StringComparer.Ordinal);

        for (int pass = 0; pass < maxRecipePasses; pass++)
        {
            Dictionary<int, int> available = CountItems(cluster);
            if (available.Count == 0)
                break;

            bool craftedThisPass = false;
            foreach (DropRecipe recipe in _recipes)
            {
                if (craftedRecipeSignatures.Contains(recipe.Signature))
                    continue;

                if (!HasIngredients(available, recipe))
                    continue;

                if (!ConditionsMatch(recipe, snapshot))
                {
                    _runtime.ConditionMisses++;
                    continue;
                }

                if (!HasRequiredStation(recipe, center))
                {
                    _runtime.StationMisses++;
                    continue;
                }

                int craftCount = GetCraftCount(available, recipe);
                if (craftCount <= 0)
                    continue;

                if (!ConsumeIngredients(cluster, recipe, craftCount, out Dictionary<int, int> consumedStacks))
                {
                    _runtime.ConsumeFailures++;
                    return craftedAny;
                }

                int outputStack = recipe.OutputStack * craftCount;
                SpawnItem(recipe.OutputType, outputStack, center);
                _runtime.CraftBatches++;
                _runtime.Crafts += craftCount;

                TShock.Log.ConsoleInfo(GetString($"[GroundCraft] {recipe.Id} 在 {center.X / 16f:0}, {center.Y / 16f:0} 合成 {ItemName(recipe.OutputType)} x{outputStack}。"));
                NotifyNearby(center, recipe, outputStack, consumedStacks);
                craftedRecipeSignatures.Add(recipe.Signature);
                craftedAny = true;
                craftedThisPass = true;
                break;
            }

            if (!craftedThisPass)
                break;
        }

        return craftedAny;
    }

    private static Dictionary<int, int> CountItems(IEnumerable<DropRef> cluster)
    {
        Dictionary<int, int> counts = new();
        foreach (DropRef drop in cluster)
        {
            if (IsUsableDrop(drop.Item))
                AddCount(counts, drop.Item.type, drop.Item.stack);
        }

        return counts;
    }

    private static bool HasIngredients(IReadOnlyDictionary<int, int> available, DropRecipe recipe)
    {
        foreach ((int type, int required) in recipe.Ingredients)
        {
            if (!available.TryGetValue(type, out int count) || count < required)
                return false;
        }

        return true;
    }

    private int GetCraftCount(IReadOnlyDictionary<int, int> available, DropRecipe recipe)
    {
        int craftCount = int.MaxValue;
        foreach ((int type, int required) in recipe.Ingredients)
        {
            if (!available.TryGetValue(type, out int count))
                return 0;

            craftCount = Math.Min(craftCount, count / required);
        }

        if (craftCount == int.MaxValue)
            return 0;

        if (recipe.OutputMaxStack > 0 && recipe.OutputStack > 0)
            craftCount = Math.Min(craftCount, Math.Max(1, recipe.OutputMaxStack / recipe.OutputStack));

        return Math.Min(craftCount, Math.Max(1, _config.MaxCraftsPerClusterPerScan));
    }

    private bool ConsumeIngredients(IEnumerable<DropRef> cluster, DropRecipe recipe, int craftCount, out Dictionary<int, int> consumedStacks)
    {
        consumedStacks = new Dictionary<int, int>();
        Dictionary<int, int> remaining = recipe.Ingredients.ToDictionary(p => p.Key, p => p.Value * craftCount);

        foreach (DropRef drop in cluster.OrderBy(d => d.Index))
        {
            WorldItem item = drop.Item;
            if (!item.active || item.type <= 0 || item.stack <= 0)
                continue;

            if (!remaining.TryGetValue(item.type, out int needed) || needed <= 0)
                continue;

            int taken = Math.Min(item.stack, needed);
            remaining[item.type] = needed - taken;
            AddCount(consumedStacks, item.type, taken);

            int leftoverStack = item.stack - taken;

            if (leftoverStack > 0)
            {
                item.stack = leftoverStack;
                SyncItem(drop.Index);
                _stableScans[drop.Index] = Math.Max(1, _config.RequiredStableScans);
                continue;
            }

            item.TurnToAir(true);
            ClearConsumedItem(drop.Index);
            _stableScans.Remove(drop.Index);
        }

        return remaining.Values.All(v => v <= 0);
    }

    private static void SpawnItem(int itemType, int stack, Vector2 center)
    {
        int index = Item.NewItem(
            new EntitySource_WorldEvent(),
            (int)center.X - 8,
            (int)center.Y - 8,
            16,
            16,
            itemType,
            stack,
            false,
            0,
            true);

        if (index >= 0)
            SyncItem(index);
    }

    private void NotifyNearby(Vector2 center, DropRecipe recipe, int outputStack, IReadOnlyDictionary<int, int> consumedStacks)
    {
        if (!_config.NotifyPlayers)
            return;

        float radiusPixels = Math.Max(1, _config.NotifyRadiusTiles) * 16f;
        float radiusSquared = radiusPixels * radiusPixels;
        string message = GetString($"地上合成：{ItemName(recipe.OutputType)} x{outputStack}");
        string consumed = FormatConsumedStacks(consumedStacks);

        foreach (TSPlayer? player in TShock.Players)
        {
            if (player is not { Active: true } || player == TSPlayer.Server)
                continue;

            Vector2 playerCenter = PlayerCenter(player.TPlayer);
            if (Vector2.DistanceSquared(center, playerCenter) > radiusSquared)
                continue;

            player.SendSuccessMessage(message);
            if (_config.NotifyConsumedItems)
                player.SendInfoMessage(GetString($"已清除被消耗的掉落物：{consumed}。如果你还看到旧材料，那是客户端残影，实际已经不存在。"));
        }
    }

    private static void SyncItem(int index)
    {
        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index);
    }

    private void ClearConsumedItem(int index)
    {
        if (!_config.ClearClientGhostItems)
        {
            SyncItem(index);
            return;
        }

        NetMessage.SendData(MessageID.ReleaseItemOwnership, -1, -1, null, index);
        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index);
        NetMessage.SendData(MessageID.SyncItemDespawn, -1, -1, null, index);
        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index);
    }

    private bool HasRequiredStation(DropRecipe recipe, Vector2 center)
    {
        if (recipe.RequiredTiles.Length == 0)
            return true;

        int tileX = (int)MathF.Round(center.X / 16f);
        int tileY = (int)MathF.Round(center.Y / 16f);
        int radius = Math.Max(0, _config.StationSearchRadiusTiles);

        for (int x = tileX - radius; x <= tileX + radius; x++)
        {
            for (int y = tileY - radius; y <= tileY + radius; y++)
            {
                if (!WorldGen.InWorld(x, y, 10))
                    continue;

                ITile tile = Framing.GetTileSafely(x, y);
                if (tile.active() && TileMatchesAny(recipe.RequiredTiles, tile.type))
                    return true;
            }
        }

        return false;
    }

    private static bool TileMatchesAny(IReadOnlyCollection<int> requiredTiles, int actualTile)
    {
        foreach (int requiredTile in requiredTiles)
        {
            if (TileMatches(requiredTile, actualTile))
                return true;
        }

        return false;
    }

    private static bool TileMatches(int requiredTile, int actualTile)
    {
        if (requiredTile == actualTile)
            return true;

        List<int>[]? countsAs = Recipe.TileCountsAs;
        if (requiredTile >= 0 && requiredTile < countsAs.Length && countsAs[requiredTile]?.Contains(actualTile) == true)
            return true;

        return actualTile >= 0 && actualTile < countsAs.Length && countsAs[actualTile]?.Contains(requiredTile) == true;
    }


    private static bool IsUsableDrop(WorldItem item)
    {
        return item.active && item.type > 0 && item.stack > 0 && item.width > 0 && item.height > 0;
    }

    private bool IsStill(WorldItem item)
    {
        return item.velocity.LengthSquared() <= Math.Max(0.0001f, _config.StillVelocitySquared);
    }

}
