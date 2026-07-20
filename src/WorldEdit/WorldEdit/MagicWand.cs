using System.Collections.Generic;
using System.Linq;
using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit;

public class MagicWand
{
    public static int MaxPointCount;

    internal bool dontCheck = false;

    internal List<WEPoint> Points = new List<WEPoint>();

    public MagicWand()
    {
        dontCheck = true;
    }

    public MagicWand(WEPoint[] Points)
    {
        dontCheck = false;
        this.Points = Points?.ToList() ?? new List<WEPoint>();
    }

    public bool InSelection(int X, int Y)
    {
        return dontCheck || Points.Any(p => p.X == X && p.Y == Y);
    }

    public static bool GetMagicWandSelection(int X, int Y, Expression Expression, TSPlayer Player, out MagicWand MagicWand)
    {
        MagicWand = new MagicWand();
        if (!Tools.InMapBoundaries(X, Y) || Expression == null)
        {
            return false;
        }
        if (!Expression.Evaluate(Main.tile[X, Y]))
        {
            return false;
        }

        short x = (short) X;
        short y = (short) Y;
        List<WEPoint> points = new List<WEPoint> { new WEPoint(x, y) };
        int count = 0;
        bool[,] visited = new bool[Main.maxTilesX, Main.maxTilesY];
        visited[x, y] = true;

        for (int i = 0; i < points.Count; i++)
        {
            WEPoint current = points[i];
            WEPoint[] neighbors = new WEPoint[]
            {
                new WEPoint((short)(current.X + 1), current.Y),
                new WEPoint((short)(current.X - 1), current.Y),
                new WEPoint(current.X, (short)(current.Y + 1)),
                new WEPoint(current.X, (short)(current.Y - 1))
            };

            foreach (WEPoint neighbor in neighbors)
            {
                if (Tools.InMapBoundaries(neighbor.X, neighbor.Y) && !visited[neighbor.X, neighbor.Y])
                {
                    visited[neighbor.X, neighbor.Y] = true;
                    if (Expression.Evaluate(Main.tile[neighbor.X, neighbor.Y]))
                    {
                        points.Add(neighbor);
                        count++;
                        if (count >= MaxPointCount)
                        {
                            Player.SendErrorMessage("Hard selection tile limit " + $"({MaxPointCount}) has been reached.");
                            return false;
                        }
                    }
                }
            }
        }

        MagicWand = new MagicWand(points.ToArray());
        return true;
    }
}