using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace SurvivalCrisis
{
    public class Replenisher
    {
        public static int ForcePlaceChest(Point where)
        {
            TileSection section = new TileSection(where.X, where.Y - 1, 2, 3);
            Tile copy0 = new Tile(section[0, 2]);
            Tile copy1 = new Tile(section[1, 2]);
            section.KillAllTile();
            section.PlaceTileAt(new Point(0, 2), TileID.WoodBlock);
            section.PlaceTileAt(new Point(1, 2), TileID.WoodBlock);
            return PlaceChest(where);
        }
        public static int PlaceChest(Point coordinate, ushort type = 21)
        {
            /*
			var tileData = TileObjectData.GetTileData(type, 0);
			var chest = TileObject.Empty;
			chest.xCoord = coordinate.X - tileData.Origin.X;
			chest.yCoord = coordinate.Y - tileData.Origin.Y;
			chest.type = type;
			chest.random = Main.rand.Next(tileData.RandomStyleRange);

			TileObject.Place(chest);
			int idx = Chest.CreateChest(coordinate.X, coordinate.Y);

			return idx;
			*/
            return WorldGen.PlaceChest(coordinate.X, coordinate.Y, type);
        }

        /// <summary>
        /// 单独放置一个物块并发包更新
        /// </summary>
        /// <param name="point">物块所在的位置</param>
        /// <param name="type">最好使用TileID.XXX, 增加可读性</param>
        /// <param name="client">发包给那个玩家更新, -1就是给所有玩家</param>
        public static void PlaceTileAndUpdate(Point point, ushort type, int client = -1)
        {
            Main.tile[point.X, point.Y].active(true);
            Main.tile[point.X, point.Y].type = type;
            WorldGen.SlopeTile(point.X, point.Y);
            NetMessage.SendTileSquare(client, point.X, point.Y, 1);
        }
        public static void PlaceWallAndUpdate(Point point, ushort type, int client = -1)
        {
            Main.tile[point.X, point.Y].wall = type;
            NetMessage.SendTileSquare(client, point.X, point.Y, 1);
        }

        public static void SpecialLine(int x, int y, int length, ushort type)
        {
            for (int i = 0; i < length; i++)
            {
                Main.tile[x + i, y].active(false);
                Main.tile[x + i, y].active(true);
                Main.tile[x + i, y].type = type;
                WorldGen.SlopeTile(x + i, y, 4);
            }
        }

        public static void UpdateSection(int x, int y, int x2, int y2, int who = -1)
        {
            int lowX = Netplay.GetSectionX(x);
            int highX = Netplay.GetSectionX(x2);
            int lowY = Netplay.GetSectionY(y);
            int highY = Netplay.GetSectionY(y2);
            if (who == -1)
            {
                foreach (RemoteClient sock in Netplay.Clients)
                {
                    if (!sock.IsActive)
                    {
                        continue;
                    }
                    for (int i = lowX; i <= highX; i++)
                    {
                        for (int j = lowY; j <= highY; j++)
                            sock.TileSections[i, j] = false;
                    }
                }
            }
            else
            {
                var sock = Netplay.Clients[who];
                if (!sock.IsActive)
                {
                    return;
                }
                for (int i = lowX; i <= highX; i++)
                {
                    for (int j = lowY; j <= highY; j++)
                        sock.TileSections[i, j] = false;
                }
            }
        }

    }
}
