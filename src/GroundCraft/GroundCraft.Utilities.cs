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
    private static void AddCount(IDictionary<int, int> counts, int itemType, int stack)
    {
        counts[itemType] = counts.TryGetValue(itemType, out int current) ? current + stack : stack;
    }

    private static void AddReason(IDictionary<string, int> counts, string reason, int count)
    {
        counts[reason] = counts.TryGetValue(reason, out int current) ? current + count : count;
    }

    private static string ItemName(int itemType)
    {
        string name = Lang.GetItemNameValue(itemType);
        return string.IsNullOrWhiteSpace(name) ? GetString($"物品 {itemType}") : name;
    }

    private static string FormatConsumedStacks(IReadOnlyDictionary<int, int> consumedStacks)
    {
        if (consumedStacks.Count == 0)
            return GetString("无");

        return string.Join(", ", consumedStacks
            .OrderBy(p => ItemName(p.Key), StringComparer.OrdinalIgnoreCase)
            .Select(p => $"{ItemName(p.Key)} x{p.Value}"));
    }

    private static Vector2 AverageCenter(IReadOnlyCollection<DropRef> cluster)
    {
        Vector2 total = Vector2.Zero;
        foreach (DropRef drop in cluster)
            total += drop.Center;

        return total / Math.Max(1, cluster.Count);
    }
}
