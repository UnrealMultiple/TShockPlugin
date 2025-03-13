using Terraria;

namespace ReverseWorld;

internal class Replenisher
{
    public static bool PlaceLandMine(int x, int y, int y2)
    {
        for (var i = y; i <= y2; i++)
        {
            if (TryPlaceLandMine(x, i))
            {
                return true;
            }
        }
        return false;
    }

    private static bool TryPlaceLandMine(int x, int y)
    {
        if (TileObject.CanPlace(x, y, 210, 0, 1, out var val, false))
        {
            TileUtils.SetTile(x, y, 210);
            return true;
        }
        return false;
    }

    public static bool Replenish(int amount, TileSection range, int TIMEOUT = 30000)
    {
        var random = new Random();
        var left = range.Left;
        var top = range.Top;
        var right = range.Right;
        var bottom = range.Bottom;
        var num = 0;
        for (var i = 0; i < TIMEOUT; i++)
        {
            var x = random.Next(left, right);
            var num2 = random.Next(top, bottom);
            var y = (num2 + 5 > bottom) ? bottom : (num2 + 5);
            if (!PlaceLandMine(x, num2, y) && ++num > amount)
            {
                return true;
            }
        }
        return false;
    }

    public static void UpdateSection(int x, int y, int x2, int y2, int who = -1)
    {
        var sectionX = Netplay.GetSectionX(x);
        var sectionX2 = Netplay.GetSectionX(x2);
        var sectionY = Netplay.GetSectionY(y);
        var sectionY2 = Netplay.GetSectionY(y2);
        if (who == -1)
        {
            var clients = Netplay.Clients;
            foreach (var val in clients)
            {
                if (!val.IsActive)
                {
                    continue;
                }
                for (var j = sectionX; j <= sectionX2; j++)
                {
                    for (var k = sectionY; k <= sectionY2; k++)
                    {
                        val.TileSections[j, k] = false;
                    }
                }
            }
            return;
        }
        var val2 = Netplay.Clients[who];
        if (!val2.IsActive)
        {
            return;
        }
        for (var l = sectionX; l <= sectionX2; l++)
        {
            for (var m = sectionY; m <= sectionY2; m++)
            {
                val2.TileSections[l, m] = false;
            }
        }
    }
}