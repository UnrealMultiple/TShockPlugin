using Terraria;

namespace Global
{
    internal class Replenisher
    {
        public static bool PlaceLandMine(int x, int y, int y2)
        {
            for (int i = y; i <= y2; i++)
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
            TileObject val;
            if (TileObject.CanPlace(x, y, 210, 0, 1, out val, false))
            {
                TileUtils.SetTile(x, y, 210);
                return true;
            }
            return false;
        }

        public static bool Replenish(int amount, TileSection range, int TIMEOUT = 30000)
        {
            Random random = new Random();
            int left = range.Left;
            int top = range.Top;
            int right = range.Right;
            int bottom = range.Bottom;
            int num = 0;
            for (int i = 0; i < TIMEOUT; i++)
            {
                int x = random.Next(left, right);
                int num2 = random.Next(top, bottom);
                int y = ((num2 + 5 > bottom) ? bottom : (num2 + 5));
                if (!PlaceLandMine(x, num2, y) && ++num > amount)
                {
                    return true;
                }
            }
            return false;
        }

        public static void UpdateSection(int x, int y, int x2, int y2, int who = -1)
        {
            int sectionX = Netplay.GetSectionX(x);
            int sectionX2 = Netplay.GetSectionX(x2);
            int sectionY = Netplay.GetSectionY(y);
            int sectionY2 = Netplay.GetSectionY(y2);
            if (who == -1)
            {
                RemoteClient[] clients = Netplay.Clients;
                foreach (RemoteClient val in clients)
                {
                    if (!val.IsActive)
                    {
                        continue;
                    }
                    for (int j = sectionX; j <= sectionX2; j++)
                    {
                        for (int k = sectionY; k <= sectionY2; k++)
                        {
                            val.TileSections[j, k] = false;
                        }
                    }
                }
                return;
            }
            RemoteClient val2 = Netplay.Clients[who];
            if (!val2.IsActive)
            {
                return;
            }
            for (int l = sectionX; l <= sectionX2; l++)
            {
                for (int m = sectionY; m <= sectionY2; m++)
                {
                    val2.TileSections[l, m] = false;
                }
            }
        }
    }
}