using Terraria.GameContent;
using Terraria.ID;

namespace AdditionalPylons;

internal static class PylonLimits
{
    internal static TeleportPylonType GetPylonTypeFromItemId(int itemId) => itemId switch
    {
        ItemID.TeleportationPylonJungle => TeleportPylonType.Jungle,
        ItemID.TeleportationPylonPurity => TeleportPylonType.SurfacePurity,
        ItemID.TeleportationPylonHallow => TeleportPylonType.Hallow,
        ItemID.TeleportationPylonUnderground => TeleportPylonType.Underground,
        ItemID.TeleportationPylonOcean => TeleportPylonType.Beach,
        ItemID.TeleportationPylonDesert => TeleportPylonType.Desert,
        ItemID.TeleportationPylonSnow => TeleportPylonType.Snow,
        ItemID.TeleportationPylonMushroom => TeleportPylonType.GlowingMushroom,
        ItemID.TeleportationPylonVictory => TeleportPylonType.Victory,
        ItemID.TeleportationPylonUnderworld => TeleportPylonType.Underworld,
        ItemID.TeleportationPylonShimmer => TeleportPylonType.Shimmer,
        _ => TeleportPylonType.Count
    };

    internal static int GetLimit(Configuration config, TeleportPylonType type) => Math.Max(1, type switch
    {
        TeleportPylonType.Jungle => config.MaxJunglePylons,
        TeleportPylonType.SurfacePurity => config.MaxForestPylons,
        TeleportPylonType.Hallow => config.MaxHallowPylons,
        TeleportPylonType.Underground => config.MaxCavernPylons,
        TeleportPylonType.Beach => config.MaxOceanPylons,
        TeleportPylonType.Desert => config.MaxDesertPylons,
        TeleportPylonType.Snow => config.MaxSnowPylons,
        TeleportPylonType.GlowingMushroom => config.MaxMushroomPylons,
        TeleportPylonType.Victory => config.MaxUniversalPylons,
        TeleportPylonType.Underworld => config.MaxUnderworldPylons,
        TeleportPylonType.Shimmer => config.MaxShimmerPylons,
        _ => 1
    });

    // Only hide the selected type; always restore every type regardless of limits.
    internal static bool ShouldSendPylon(bool addPylons, TeleportPylonType selected, TeleportPylonType candidate)
        => addPylons || (selected != TeleportPylonType.Count && selected == candidate);
}
