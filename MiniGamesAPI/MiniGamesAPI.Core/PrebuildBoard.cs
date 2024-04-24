using Microsoft.Xna.Framework;
using System.Text;
using Terraria.ID;
using TShockAPI;

namespace MiniGamesAPI
{
    public class PrebuildBoard
    {
        public string Name { get; set; }

        public int ID { get; set; }

        public MiniRegion Region { get; private set; }

        public Point TestPoint_1 { get; set; }

        public Point TestPoint_2 { get; set; }

        public List<MiniTile> Tiles { get; set; }

        public PrebuildBoard(MiniRegion region)
        {
            ID = region.ID;
            Name = region.Name + "的预制板";
            Region = region;
            Tiles = new List<MiniTile>();
            for (int i = region.TopLeft.X; i <= region.BottomRight.X; i++)
            {
                for (int j = region.TopLeft.Y; j <= region.BottomRight.Y; j++)
                {
                    Tiles.Add(new MiniTile(i, j, Terraria.Main.tile[i, j]));
                }
            }
        }

        public PrebuildBoard(Point topLeft, Point bottomRight, int id, string name)
        {
            ID = id;
            Name = name + "的预制板";
            Region = null;
            Tiles = new List<MiniTile>();
            TestPoint_1 = topLeft;
            TestPoint_2 = bottomRight;
            for (int i = topLeft.X; i <= bottomRight.X; i++)
            {
                for (int j = topLeft.Y; j <= bottomRight.Y; j++)
                {
                    Tiles.Add(new MiniTile(i, j, Terraria.Main.tile[i, j]));
                }
            }
        }

        public void ReBuild(bool noItem = false)
        {
            foreach (MiniTile tile in Tiles)
            {
                tile.Kill(noItem);
                tile.Place();
            }
            TSPlayer.All.SendTileRect((short)Region.TopLeft.X, (short)Region.TopLeft.Y, (byte)Region.Area.Width, (byte)Region.Area.Height, (TileChangeType)0);
        }

        public string ShowInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"[{TestPoint_1.X},{TestPoint_1.Y}|{TestPoint_2.X},{TestPoint_2.Y}|]");
            foreach (MiniTile tile in Tiles)
            {
                stringBuilder.Append($"[{tile.Type}|{tile.X}|{tile.Y}]");
            }
            return stringBuilder.ToString();
        }
    }
}
