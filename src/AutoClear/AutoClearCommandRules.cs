using System;
using System.Collections.Generic;

namespace AutoClear
{
    internal static class AutoClearCommandRules
    {
        internal const int DefaultRadiusTiles = 50;
        internal const int ItemsPerTick = 8;
        internal const int WorldItemSlotCount = 400;

        internal static bool IsItemSubcommand(string value)
        {
            return string.Equals(value, "item", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "items", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "i", StringComparison.OrdinalIgnoreCase);
        }

        internal static bool TryParseItemClearParameters(IReadOnlyList<string> parameters, out int radiusTiles)
        {
            radiusTiles = DefaultRadiusTiles;
            if (parameters == null
                || (parameters.Count != 1 && parameters.Count != 2)
                || !IsItemSubcommand(parameters[0]))
            {
                return false;
            }

            return parameters.Count == 1
                || (int.TryParse(parameters[1], out radiusTiles) && radiusTiles > 0);
        }

        internal static bool TryParseSafeCommandParameters(IReadOnlyList<string> parameters, out int radiusTiles)
        {
            radiusTiles = DefaultRadiusTiles;
            return parameters != null
                && (parameters.Count == 0
                    || (parameters.Count == 1
                        && int.TryParse(parameters[0], out radiusTiles)
                        && radiusTiles > 0));
        }

        internal static bool IsWithinRadius(
            float itemX,
            float itemY,
            float centerX,
            float centerY,
            int radiusTiles)
        {
            if (radiusTiles <= 0)
            {
                return false;
            }

            double deltaX = itemX - centerX;
            double deltaY = itemY - centerY;
            double radiusPixels = radiusTiles * 16d;
            return deltaX * deltaX + deltaY * deltaY <= radiusPixels * radiusPixels;
        }

        internal static int GetBatchSize(int remainingItems)
        {
            return Math.Clamp(remainingItems, 0, ItemsPerTick);
        }
    }
}
