using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace AutoFish.Utils;

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
        var newVec = vec.SafeNormalize(default);
        return newVec * len;
    }

    public static Vector2[] GetCircleEdgePoints(this Vector2 pos, float radius, float distanceBetweenPoints)
    {
        var numPoints = (int)Math.Ceiling(2 * Math.PI * radius / distanceBetweenPoints);
        var points = new Vector2[numPoints];
        for (var i = 0; i < numPoints; i++)
        {
            var angle = (float)(i * (2 * Math.PI) / numPoints);
            var x = radius * (float)Math.Cos(angle);
            var y = radius * (float)Math.Sin(angle);
            points[i] = pos + new Vector2(x, y);
        }

        return points;
    }

    public static Vector2[] GetArcPoints(this Vector2 vel, float startAngle, float endAngle, float radius,
        float interval)
    {
        var points = new List<Vector2>();
        var steps = (int)Math.Round((endAngle - startAngle + 360) % 360 / interval);
        for (var i = 0; i <= steps; i++)
        {
            var angle = (startAngle + i * interval + 360) % 360;
            var x = (float)(radius * Math.Cos(angle * Math.PI / 180));
            var y = (float)(radius * Math.Sin(angle * Math.PI / 180));
            points.Add(new Vector2(x, y) + vel);
        }

        return points.ToArray();
    }

    public static float Magnitude(this Vector2 vel)
    {
        return Convert.ToSingle(Math.Sqrt(vel.X * vel.X + vel.Y * vel.Y));
    }

    public static Vector2 Normalized(this Vector2 vel)
    {
        var magnitude = vel.Magnitude();
        return new Vector2(vel.X / magnitude, vel.Y / magnitude);
    }


    public static List<NPC> FindRangeNPCs(this Vector2 pos, float distanceSquared)
    {
        var npcs = new List<NPC>();
        foreach (var npc in Main.npc)
            if (npc != null && npc.active && npc.CanBeChasedBy())
                if (distanceSquared > Math.Abs(npc.Center.Distance(pos)))
                    npcs.Add(npc);

        return npcs;
    }

    public static List<Projectile> FindRangeProjectiles(this Vector2 pos, float distanceSquared)
    {
        var projectile = new List<Projectile>();
        foreach (var proj in Main.projectile)
            if (proj != null && proj.active)
                if (distanceSquared > Math.Abs(proj.Center.Distance(pos)))
                    projectile.Add(proj);

        return projectile;
    }

    public static List<Player> FindRangePlayers(this Vector2 pos, float distanceSquared)
    {
        var players = new List<Player>();
        foreach (var ply in Main.player)
            if (ply != null && ply.active)
                if (distanceSquared > Math.Abs(ply.Center.Distance(pos)))
                    players.Add(ply);

        return players;
    }

    public static List<TSPlayer> FindRangeTSPlayers(this Vector2 pos, float distanceSquared)
    {
        var players = new List<TSPlayer>();
        foreach (var ply in TShock.Players)
            if (ply != null && ply.Active)
                if (distanceSquared > Math.Abs(ply.TPlayer.Center.Distance(pos)))
                    players.Add(ply);

        return players;
    }


    public static NPC? FindRangeNPC(this Vector2 pos, float distanceSquared)
    {
        var npcs = pos.FindRangeNPCs(distanceSquared);
        if (npcs.Count == 0) return null;

        var boss = npcs.OrderBy(x => Math.Abs(x.position.Distance(pos)));
        return boss.FirstOrDefault(x => x.boss, boss.First());
    }

    public static List<Vector2> GenerateCurvePoints(Vector2 pointA, Vector2 pointB, float radius, float interval)
    {
        if (radius == 0) return new List<Vector2> { pointA, pointB };

        List<Vector2> curvePoints = new();
        double distance = (pointB - pointA).Magnitude();
        var numPoints = (int)Math.Round(distance / interval);
        for (var i = 0; i <= numPoints; i++)
        {
            var t = i / (float)numPoints;
            var midPoint = Vector2.Lerp(pointA, pointB, t);
            var direction = (pointB - pointA).Normalized();
            var perpendicular = new Vector2(-direction.Y, direction.X);
            var curvePoint = midPoint + radius * perpendicular;
            if (radius < 0) curvePoint = midPoint - Math.Abs(radius) * perpendicular;

            curvePoints.Add(curvePoint);
        }

        return curvePoints;
    }

    public static List<Vector2> GetPointsOnCircle(this Vector2 center, float radius, float startAngle,
        float angleIncrement, int loopCount)
    {
        var points = new List<Vector2>();
        var currentAngle = startAngle;
        for (var i = 0; i < loopCount; i++)
        {
            var x = center.X + radius * (float)Math.Cos(currentAngle * Math.PI / 180);
            var y = center.Y + radius * (float)Math.Sin(currentAngle * Math.PI / 180);
            points.Add(new Vector2(x, y));
            currentAngle += angleIncrement;
        }

        return points;
    }


    public static List<Vector2> CreateCircle(this Vector2 startPoint, Vector2 centerPoint, int angleIncrement,
        int numberOfPoints)
    {
        var points = new List<Vector2>();
        for (var i = 0; i < numberOfPoints; i++)
        {
            float angle = angleIncrement * i;
            var x = centerPoint.X + (float)Math.Cos(angle) * (startPoint.X - centerPoint.X);
            var y = centerPoint.Y + (float)Math.Sin(angle) * (startPoint.Y - centerPoint.Y);
            points.Add(new Vector2(x, y));
        }

        return points;
    }
}