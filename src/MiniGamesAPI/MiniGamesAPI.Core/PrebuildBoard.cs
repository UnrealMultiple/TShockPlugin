using Microsoft.Xna.Framework;
using System.Text;
using TShockAPI;

namespace MiniGamesAPI;

public class PrebuildBoard
{
    public string Name { get; set; }

    public int ID { get; set; }

    public MiniRegion? Region { get; private set; }

    public Point TestPoint_1 { get; set; }

    public Point TestPoint_2 { get; set; }

    public List<MiniTile> Tiles { get; set; }

    public PrebuildBoard(MiniRegion region)
    {
        this.ID = region.ID;
        this.Name = GetString($"{region.Name}的预制板");
        this.Region = region;
        this.Tiles = new List<MiniTile>();
        for (var i = region.TopLeft.X; i <= region.BottomRight.X; i++)
        {
            for (var j = region.TopLeft.Y; j <= region.BottomRight.Y; j++)
            {
                this.Tiles.Add(new MiniTile(i, j, Terraria.Main.tile[i, j]));
            }
        }
    }

    public PrebuildBoard(Point topLeft, Point bottomRight, int id, string name)
    {
        this.ID = id;
        this.Name = GetString($"{name}的预制板");
        this.Region = null;
        this.Tiles = new List<MiniTile>();
        this.TestPoint_1 = topLeft;
        this.TestPoint_2 = bottomRight;
        for (var i = topLeft.X; i <= bottomRight.X; i++)
        {
            for (var j = topLeft.Y; j <= bottomRight.Y; j++)
            {
                this.Tiles.Add(new MiniTile(i, j, Terraria.Main.tile[i, j]));
            }
        }
    }

    public void ReBuild(bool noItem = false)
    {
        foreach (var tile in this.Tiles)
        {
            tile.Kill(noItem);
            tile.Place();
        }
        TSPlayer.All.SendTileRect((short) this.Region!.TopLeft.X, (short) this.Region.TopLeft.Y, (byte) this.Region.Area.Width, (byte) this.Region.Area.Height, 0);
    }

    public string ShowInfo()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"[{this.TestPoint_1.X},{this.TestPoint_1.Y}|{this.TestPoint_2.X},{this.TestPoint_2.Y}|]");
        foreach (var tile in this.Tiles)
        {
            stringBuilder.Append($"[{tile.Type}|{tile.X}|{tile.Y}]");
        }
        return stringBuilder.ToString();
    }
}