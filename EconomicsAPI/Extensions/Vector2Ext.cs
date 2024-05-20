using Microsoft.Xna.Framework;
using Terraria;

namespace EconomicsAPI.Extensions;

public static class Vector2Ext
{
    public static bool HasNanOrInf(this Vector2 vec)
    {
        return float.IsNaN(vec.X) || float.IsInfinity(vec.X)
            || float.IsNaN(vec.Y) || float.IsInfinity(vec.Y)
            || float.IsPositiveInfinity(vec.X) || float.IsPositiveInfinity(vec.Y);

    }

    public static Vector2 RotationAngle(this Vector2 vel, float angle)
    {
        return vel.RotatedBy(angle * (Math.PI / 180));
    }

    public static Vector2 ToLenOf(this Vector2 vec, float len)
    {
        var old = vec;
        vec.Normalize();
        if (vec.HasNanOrInf())
            return old * len;
        return vec * len;
    }
    
    public static Vector2[] GetCircleEdgePoints(this Vector2 pos, float radius, float distanceBetweenPoints)
    {
        int numPoints = (int)Math.Ceiling(2 * Math.PI * radius / distanceBetweenPoints);
        Vector2[] points = new Vector2[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float angle = (float)(i * (2 * Math.PI) / numPoints);
            float x = radius * (float)Math.Cos(angle);
            float y = radius * (float)Math.Sin(angle);
            points[i] = pos + new Vector2(x, y);
        }
        return points;
    }
    
    public static Vector2[] GetArcPoints(this Vector2 vel ,float startAngle, float endAngle, float radius, float interval)
    {
        var points = new List<Vector2>();
        int steps = (int)Math.Round((endAngle - startAngle + 360) % 360 / interval);
        for (int i = 0; i <= steps; i++)
        {
            float angle = (startAngle + i * interval + 360) % 360;
            float x = (float)(radius * Math.Cos(angle * Math.PI / 180));
            float y = (float)(radius * Math.Sin(angle * Math.PI / 180));
            points.Add(new Vector2(x, y) + vel);
        }
        return points.ToArray();
    }
}





