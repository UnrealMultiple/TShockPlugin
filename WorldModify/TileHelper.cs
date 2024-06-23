using Terraria;
using Terraria.ID;
using Terraria.Map;
using TShockAPI;

namespace WorldModify
{
    public class TileHelper
    {
        public static bool isTaskRunning { get; set; }

        public static void StartGen()
        {
            isTaskRunning = true;
        }

        public static void FinishGen()
        {
            isTaskRunning = false;
            TShock.Utils.SaveWorld();
        }

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

        public static void GenAfter()
        {
            InformPlayers();
            FinishGen();
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

        public static void KillTile(int x, int y)
        {
            Main.tile[x, y].ClearTile();
            WorldGen.SquareTileFrame(x, y, true);
            NetMessage.SendTileSquare(-1, x, y, (TileChangeType)0);
        }

        public static void ClearLiquid(int x, int y)
        {
            ITile val = Main.tile[x, y];
            val.liquid = 0;
            NetMessage.SendTileSquare(-1, x, y, (TileChangeType)0);
        }

        public static void ClearEverything(int x, int y)
        {
            Main.tile[x, y].ClearEverything();
            NetMessage.SendTileSquare(-1, x, y, (TileChangeType)0);
        }

        public static void SetType(int x, int y, int id)
        {
            Main.tile[x, y].type = (ushort)id;
            Main.tile[x, y].active(true);
            Main.tile[x, y].slope(0);
            Main.tile[x, y].halfBrick(false);
        }

        public static void SetType(ITile tile, int id)
        {
            tile.type = (ushort)id;
            tile.active(true);
            tile.slope(0);
            tile.halfBrick(false);
        }

        public static void Initialize()
        {
            if (MapHelper.tileLookup == null)
            {
                bool dedServ = Main.dedServ;
                Main.dedServ = false;
                MapHelper.Initialize();
                Main.dedServ = dedServ;
            }
        }

        public static string GetTileName(int type, int style)
        {
            return Lang._mapLegendCache[MapHelper.TileToLookup(type, style)].Value;
        }
    }
}
