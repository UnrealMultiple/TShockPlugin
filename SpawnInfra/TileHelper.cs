using Terraria.Map;
using Terraria.ID;
using Terraria;
using TShockAPI;

namespace SpawnInfra
{
    internal class TileHelper
    {
        public static bool isTaskRunning { get; set; }

        public static bool NeedWaitTask(TSPlayer op)
        {
            if (isTaskRunning)
            {
                op?.SendErrorMessage("另一个创建任务正在执行，请稍后再操作");
            }
            return isTaskRunning;
        }

        public static void InformPlayers()
        {
            TSPlayer[] players = TShock.Players;
            foreach (TSPlayer tSPlayer in players)
            {
                if (tSPlayer == null || !tSPlayer.Active)
                {
                    continue;
                }
                for (int j = 0; j < 255; j++)
                {
                    for (int k = 0; k < Main.maxSectionsX; k++)
                    {
                        for (int l = 0; l < Main.maxSectionsY; l++)
                        {
                            Netplay.Clients[j].TileSections[k, l] = false;
                        }
                    }
                }
            }
        }

        public static void ClearTile(int x, int y, int w = 1, int h = 1)
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    ClearTile(x + i, y + j);
                }
            }
        }

        public static void ClearTile(int x, int y)
        {
            Main.tile[x, y].ClearTile();
            NetMessage.SendTileSquare(-1, x, y, (TileChangeType)0);
        }

        public static string GetTileName(int type, int style)
        {
            return Lang._mapLegendCache[MapHelper.TileToLookup(type, style)].Value;
        }
    }
}
