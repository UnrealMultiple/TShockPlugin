using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;

namespace MiniGamesAPI;

public class MiniRegion
{
    public string Name { get; set; }

    public int ID { get; set; }

    public List<string> AllowGroups { get; set; }

    public List<string> AllowUsers { get; set; }

    public List<string> Owners { get; set; }

    public Point TopLeft => new Point(this.Area.X, this.Area.Y);

    public Point TopRight => new Point(this.Area.X + this.Area.Width - 1, this.Area.Y);

    public Point BottomLeft => new Point(this.Area.X, this.Area.Y + this.Area.Height - 1);

    public Point BottomRight => new Point(this.Area.X + this.Area.Width - 1, this.Area.Y + this.Area.Height - 1);

    public Point Center => new Point((this.Area.Width / 2) + this.Area.X, this.Area.Y + (this.Area.Height / 2));

    public Rectangle Area { get; set; }

    public bool IsLocked { get; set; }

    public MiniRegion(string name, int id, int topLeft_x, int topLeft_y, int bottomRight_x, int bottomRight_y)
    {
        this.ID = id;
        this.Name = name;
        this.IsLocked = false;
        this.AllowGroups = new List<string>();
        this.AllowUsers = new List<string>();
        this.Owners = new List<string>();

        var minX = Math.Min(topLeft_x, bottomRight_x);
        var minY = Math.Min(topLeft_y, bottomRight_y);
        var width = Math.Abs(topLeft_x - bottomRight_x) + 1;
        var height = Math.Abs(topLeft_y - bottomRight_y) + 1;

        this.Area = new Rectangle(minX, minY, width, height);
    }

    public MiniRegion(string name, int id, Rectangle area)
    {
        this.ID = id;
        this.Name = name;
        this.IsLocked = false;
        this.AllowGroups = new List<string>();
        this.AllowUsers = new List<string>();
        this.Owners = new List<string>();
        this.Area = area;
    }

    public void ShowFramework()
    {
        var vector1 = new Vector2((this.TopRight.X + 1 - this.TopLeft.X) * 16f, 0f);
        var vector2 = new Vector2(0f, (this.BottomRight.Y + 1 - this.TopLeft.Y) * 16f);
        var center = new Vector2(this.Area.Center.X * 16f, this.Area.Center.Y * 16f);

        var projectile1 = Projectile.NewProjectile(
            new EntitySource_DebugCommand(),
            new Vector2(this.TopLeft.X * 16f, this.TopLeft.Y * 16f),
            vector1 * 0.01f,
            116,
            0,
            0f,
            255,
            0f,
            0f
        );

        var projectile2 = Projectile.NewProjectile(
            new EntitySource_DebugCommand(),
            new Vector2(this.TopLeft.X * 16f, this.BottomLeft.Y * 16f),
            vector2 * 0.01f,
            116,
            0,
            0f,
            255,
            0f,
            0f
        );

        var projectile3 = Projectile.NewProjectile(
            new EntitySource_DebugCommand(),
            new Vector2(this.BottomRight.X * 16f, (this.BottomRight.Y + 1) * 16f),
            vector1 * 0.01f,
            116,
            0,
            0f,
            255,
            0f,
            0f
        );

        var projectile4 = Projectile.NewProjectile(
            new EntitySource_DebugCommand(),
            new Vector2((this.TopRight.X + 1) * 16f, this.TopRight.Y * 16f),
            vector2 * 0.01f,
            116,
            0,
            0f,
            255,
            0f,
            0f
        );

        var projectile5 = Projectile.NewProjectile(
            new EntitySource_DebugCommand(),
            center,
            Vector2.Zero,
            467,
            0,
            0f,
            255,
            0f,
            0f
        );

        TSPlayer.All.SendData((PacketTypes) 27, "", projectile1, 0f, 0f, 0f, 0);
        TSPlayer.All.SendData((PacketTypes) 27, "", projectile2, 0f, 0f, 0f, 0);
        TSPlayer.All.SendData((PacketTypes) 27, "", projectile3, 0f, 0f, 0f, 0);
        TSPlayer.All.SendData((PacketTypes) 27, "", projectile4, 0f, 0f, 0f, 0);
        TSPlayer.All.SendData((PacketTypes) 27, "", projectile5, 0f, 0f, 0f, 0);
    }

    public string ShowCoordination()
    {
        return GetString($"左上角({this.TopLeft.X},{this.TopLeft.Y})\n左下角({this.BottomLeft.X},{this.BottomLeft.Y})\n右上角({this.TopRight.X},{this.TopRight.Y})\n右下角({this.BottomRight.X},{this.BottomRight.Y})\n");
    }

    public bool Contains(Point point)
    {
        return this.Area.Contains(point);
    }

    public bool Contains(int x, int y)
    {
        return this.Area.Contains(x, y);
    }

    public void BuildFramework(int tileID, bool send = false)
    {
        for (var i = this.TopLeft.X; i <= this.TopRight.X; i++)
        {
            for (var j = this.TopLeft.Y; j <= this.BottomLeft.Y; j++)
            {
                if (i == this.TopLeft.X || i == this.TopRight.X || j == this.TopLeft.Y || j == this.BottomLeft.Y)
                {
                    WorldGen.PlaceTile(i, j, tileID, false, false, -1, 0);
                }
            }
        }

        if (send)
        {
            TSPlayer.All.SendTileRect((short) this.TopLeft.X, (short) this.TopLeft.Y, (byte) (this.Area.Width + 3), (byte) (this.Area.Height + 3), 0);
        }
    }

    public List<MiniRegion>? Divide(int width, int height, int amount, int gap)
    {
        var x = this.TopLeft.X;
        var y = this.TopLeft.Y;

        if ((gap * amount) + (amount * (width + 2)) > this.Area.Width)
        {
            return null;
        }

        var regions = new List<MiniRegion>();

        for (var i = 0; i < amount; i++)
        {
            var subArea = new Rectangle(x, y, width + 2, height + 2);
            regions.Add(new MiniRegion($"{this.Name}_{i}", this.ID + i + 1, subArea));
            x += gap + width + 2;
        }

        return regions;
    }
}