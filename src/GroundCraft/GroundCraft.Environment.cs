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
    private bool ConditionsMatch(DropRecipe recipe, EnvironmentSnapshot snapshot)
    {
        ConditionSet conditions = recipe.Conditions;

        if (conditions.Layers.Length > 0 && !conditions.Layers.Contains(snapshot.Layer, StringComparer.OrdinalIgnoreCase))
            return false;

        if (conditions.Biomes.Length > 0 && !conditions.Biomes.Any(b => snapshot.Biomes.Contains(b)))
            return false;

        if (conditions.Liquids.Length > 0 && !conditions.Liquids.All(l => snapshot.Liquids.Contains(l)))
            return false;

        if (conditions.AnyLiquids.Length > 0 && !conditions.AnyLiquids.Any(l => snapshot.Liquids.Contains(l)))
            return false;

        return BossProgressMatches(conditions.BossProgress);
    }

    private EnvironmentSnapshot ProbeEnvironment(Vector2 center)
    {
        int tileX = (int)MathF.Round(center.X / 16f);
        int tileY = (int)MathF.Round(center.Y / 16f);
        HashSet<string> liquids = ProbeLiquids(tileX, tileY);
        TSPlayer? nearest = FindNearestPlayer(center, Math.Max(1, _config.BiomePlayerProbeRadiusTiles) * 16f);
        string layer = ProbeLayer(tileY, nearest?.TPlayer);
        HashSet<string> biomes = ProbeBiomes(tileX, layer, nearest?.TPlayer, liquids);

        return new EnvironmentSnapshot(layer, biomes, liquids);
    }

    private HashSet<string> ProbeLiquids(int tileX, int tileY)
    {
        HashSet<string> liquids = new(StringComparer.OrdinalIgnoreCase);
        int radius = Math.Max(0, _config.EnvironmentSearchRadiusTiles);

        for (int x = tileX - radius; x <= tileX + radius; x++)
        {
            for (int y = tileY - radius; y <= tileY + radius; y++)
            {
                if (!WorldGen.InWorld(x, y, 10))
                    continue;

                ITile tile = Framing.GetTileSafely(x, y);
                if (tile.liquid <= 0)
                    continue;

                int liquidType = tile.liquidType();
                switch (liquidType)
                {
                    case LiquidID.Lava:
                        liquids.Add("Lava");
                        break;
                    case LiquidID.Honey:
                        liquids.Add("Honey");
                        break;
                    case LiquidID.Shimmer:
                        liquids.Add("Shimmer");
                        break;
                    default:
                        liquids.Add("Water");
                        break;
                }
            }
        }

        return liquids;
    }

    private static string ProbeLayer(int tileY, Player? nearbyPlayer)
    {
        if (nearbyPlayer is not null)
        {
            if (nearbyPlayer.ZoneUnderworldHeight)
                return "Underworld";
            if (nearbyPlayer.ZoneSkyHeight)
                return "Sky";
            if (nearbyPlayer.ZoneRockLayerHeight)
                return "Cavern";
            if (nearbyPlayer.ZoneDirtLayerHeight)
                return "Underground";
        }

        if (tileY >= Main.maxTilesY - 200)
            return "Underworld";
        if (tileY < Main.worldSurface * 0.35)
            return "Sky";
        if (tileY < Main.worldSurface)
            return "Surface";
        if (tileY < Main.rockLayer)
            return "Underground";

        return "Cavern";
    }

    private static HashSet<string> ProbeBiomes(int tileX, string layer, Player? nearbyPlayer, IReadOnlySet<string> liquids)
    {
        HashSet<string> biomes = new(StringComparer.OrdinalIgnoreCase);

        if (tileX <= 380 || tileX >= Main.maxTilesX - 380)
            biomes.Add("Ocean");

        if (liquids.Contains("Shimmer"))
            biomes.Add("Shimmer");

        if (nearbyPlayer is not null)
        {
            AddIf(biomes, nearbyPlayer.ZoneSnow, "Snow");
            AddIf(biomes, nearbyPlayer.ZoneDesert, "Desert");
            AddIf(biomes, nearbyPlayer.ZoneJungle, "Jungle");
            AddIf(biomes, nearbyPlayer.ZoneCorrupt, "Corruption");
            AddIf(biomes, nearbyPlayer.ZoneCrimson, "Crimson");
            AddIf(biomes, nearbyPlayer.ZoneHallow, "Hallow");
            AddIf(biomes, nearbyPlayer.ZoneGlowshroom, "Mushroom");
            AddIf(biomes, nearbyPlayer.ZoneGraveyard, "Graveyard");
            AddIf(biomes, nearbyPlayer.ZoneBeach, "Ocean");
            AddIf(biomes, nearbyPlayer.ZoneShimmer, "Shimmer");
            AddIf(biomes, nearbyPlayer.ZoneDungeon, "Dungeon");
            AddIf(biomes, nearbyPlayer.ZoneHive, "Hive");
            AddIf(biomes, nearbyPlayer.ZoneLihzhardTemple, "Temple");
        }

        if (layer.Equals("Surface", StringComparison.OrdinalIgnoreCase) && !HasMajorBiome(biomes))
            biomes.Add("Forest");

        return biomes;
    }

    private static bool HasMajorBiome(IReadOnlySet<string> biomes)
    {
        return biomes.Contains("Snow")
            || biomes.Contains("Desert")
            || biomes.Contains("Jungle")
            || biomes.Contains("Corruption")
            || biomes.Contains("Crimson")
            || biomes.Contains("Hallow")
            || biomes.Contains("Mushroom")
            || biomes.Contains("Graveyard")
            || biomes.Contains("Ocean")
            || biomes.Contains("Shimmer")
            || biomes.Contains("Dungeon")
            || biomes.Contains("Hive")
            || biomes.Contains("Temple");
    }

    private static void AddIf(HashSet<string> set, bool condition, string value)
    {
        if (condition)
            set.Add(value);
    }

    private static TSPlayer? FindNearestPlayer(Vector2 center, float maxDistancePixels)
    {
        float bestDistanceSquared = maxDistancePixels * maxDistancePixels;
        TSPlayer? best = null;

        foreach (TSPlayer? player in TShock.Players)
        {
            if (player is not { Active: true } || player == TSPlayer.Server)
                continue;

            float distanceSquared = Vector2.DistanceSquared(center, PlayerCenter(player.TPlayer));
            if (distanceSquared >= bestDistanceSquared)
                continue;

            bestDistanceSquared = distanceSquared;
            best = player;
        }

        return best;
    }

    private static Vector2 PlayerCenter(Player player)
    {
        return player.position + new Vector2(player.width / 2f, player.height / 2f);
    }


    private static bool BossProgressMatches(BossProgressSet progress)
    {
        if (progress.Hardmode is not null && Main.hardMode != progress.Hardmode.Value)
            return false;

        foreach (string key in progress.AllDowned)
        {
            if (!BossProgressChecks[key]())
                return false;
        }

        if (progress.AnyDowned.Length > 0 && !progress.AnyDowned.Any(key => BossProgressChecks[key]()))
            return false;

        foreach (string key in progress.NoneDowned)
        {
            if (BossProgressChecks[key]())
                return false;
        }

        return true;
    }
}
