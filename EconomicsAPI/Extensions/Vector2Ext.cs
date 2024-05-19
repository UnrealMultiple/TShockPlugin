using Microsoft.Xna.Framework;
using Terraria;

namespace EconomicsAPI.Extensions;

public static class Vector2Ext
{
    public static double ItemUseAngle(this Player TPlayer)
    {
        double angle = TPlayer.itemRotation;
        if (TPlayer.direction == -1)
        {
            angle += Math.PI;
        }
        return angle;
    }

    public static Vector2 ItemOffSet(this Player player)
    {
        float length = player.HeldItem.height;
        var offset = new Vector2(length, 0).RotatedBy(player.ItemUseAngle());
        return offset;
    }

    public static bool HasNanOrInf(this Vector2 vec)
    {
        return float.IsNaN(vec.X) || float.IsInfinity(vec.X)
            || float.IsNaN(vec.Y) || float.IsInfinity(vec.Y)
            || float.IsPositiveInfinity(vec.X) || float.IsPositiveInfinity(vec.Y);

    }

    public static Vector2 ToLenOf(this Vector2 vec, float len)
    {
        var old = vec;
        vec.Normalize();
        if (vec.HasNanOrInf())
            return old * len;
        return vec * len;
    }
}
