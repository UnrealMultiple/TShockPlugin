using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TShockAPI;

namespace MiniGamesAPI
{
    public class MiniRegion
    {
        public string Name { get; set; }

        public int ID { get; set; }

        public List<string> AllowGroups { get; set; }

        public List<string> AllowUsers { get; set; }

        public List<string> Owners { get; set; }

        public Point TopLeft => new Point(Area.X, Area.Y);

        public Point TopRight => new Point(Area.X + Area.Width - 1, Area.Y);

        public Point BottomLeft => new Point(Area.X, Area.Y + Area.Height - 1);

        public Point BottomRight => new Point(Area.X + Area.Width - 1, Area.Y + Area.Height - 1);

        public Point Center => new Point(Area.Width / 2 + Area.X, Area.Y + Area.Height / 2);

        public Rectangle Area { get; set; }

        public bool IsLocked { get; set; }

        public MiniRegion(string name, int id, int topLeft_x, int topLeft_y, int bottomRight_x, int bottomRight_y)
        {
            ID = id;
            Name = name;
            IsLocked = false;
            AllowGroups = new List<string>();
            AllowUsers = new List<string>();
            Owners = new List<string>();

            int minX = Math.Min(topLeft_x, bottomRight_x);
            int minY = Math.Min(topLeft_y, bottomRight_y);
            int width = Math.Abs(topLeft_x - bottomRight_x) + 1;
            int height = Math.Abs(topLeft_y - bottomRight_y) + 1;

            Area = new Rectangle(minX, minY, width, height);
        }

        public MiniRegion(string name, int id, Rectangle area)
        {
            ID = id;
            Name = name;
            IsLocked = false;
            AllowGroups = new List<string>();
            AllowUsers = new List<string>();
            Owners = new List<string>();
            Area = area;
        }

        public void ShowFramework()
        {
            Vector2 vector1 = new Vector2((float)(TopRight.X + 1 - TopLeft.X) * 16f, 0f);
            Vector2 vector2 = new Vector2(0f, (float)(BottomRight.Y + 1 - TopLeft.Y) * 16f);
            Vector2 center = new Vector2((float)Area.Center.X * 16f, (float)Area.Center.Y * 16f);

            int projectile1 = Projectile.NewProjectile(
                new EntitySource_DebugCommand(),
                new Vector2((float)TopLeft.X * 16f, (float)TopLeft.Y * 16f),
                vector1 * 0.01f,
                116,
                0,
                0f,
                255,
                0f,
                0f
            );

            int projectile2 = Projectile.NewProjectile(
                new EntitySource_DebugCommand(),
                new Vector2((float)TopLeft.X * 16f, (float)BottomLeft.Y * 16f),
                vector2 * 0.01f,
                116,
                0,
                0f,
                255,
                0f,
                0f
            );

            int projectile3 = Projectile.NewProjectile(
                new EntitySource_DebugCommand(),
                new Vector2((float)BottomRight.X * 16f, (float)(BottomRight.Y + 1) * 16f),
                vector1 * 0.01f,
                116,
                0,
                0f,
                255,
                0f,
                0f
            );

            int projectile4 = Projectile.NewProjectile(
                new EntitySource_DebugCommand(),
                new Vector2((float)(TopRight.X + 1) * 16f, (float)TopRight.Y * 16f),
                vector2 * 0.01f,
                116,
                0,
                0f,
                255,
                0f,
                0f
            );

            int projectile5 = Projectile.NewProjectile(
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

            TSPlayer.All.SendData((PacketTypes)27, "", projectile1, 0f, 0f, 0f, 0);
            TSPlayer.All.SendData((PacketTypes)27, "", projectile2, 0f, 0f, 0f, 0);
            TSPlayer.All.SendData((PacketTypes)27, "", projectile3, 0f, 0f, 0f, 0);
            TSPlayer.All.SendData((PacketTypes)27, "", projectile4, 0f, 0f, 0f, 0);
            TSPlayer.All.SendData((PacketTypes)27, "", projectile5, 0f, 0f, 0f, 0);
        }

        public string ShowCoordination()
        {
            return $"左上角({TopLeft.X},{TopLeft.Y})\n左下角({BottomLeft.X},{BottomLeft.Y})\n右上角({TopRight.X},{TopRight.Y})\n右下角({BottomRight.X},{BottomRight.Y})\n";
        }

        public bool Contains(Point point)
        {
            return Area.Contains(point);
        }

        public bool Contains(int x, int y)
        {
            return Area.Contains(x, y);
        }

        public void BuildFramework(int tileID, bool send = false)
        {
            for (int i = TopLeft.X; i <= TopRight.X; i++)
            {
                for (int j = TopLeft.Y; j <= BottomLeft.Y; j++)
                {
                    if (i == TopLeft.X || i == TopRight.X || j == TopLeft.Y || j == BottomLeft.Y)
                    {
                        WorldGen.PlaceTile(i, j, tileID, false, false, -1, 0);
                    }
                }
            }

            if (send)
            {
                TSPlayer.All.SendTileRect((short)TopLeft.X, (short)TopLeft.Y, (byte)(Area.Width + 3), (byte)(Area.Height + 3), (TileChangeType)0);
            }
        }

        public List<MiniRegion> Divide(int width, int height, int amount, int gap)
        {
            int x = TopLeft.X;
            int y = TopLeft.Y;

            if (gap * amount + amount * (width + 2) > Area.Width)
            {
                return null;
            }

            List<MiniRegion> regions = new List<MiniRegion>();

            for (int i = 0; i < amount; i++)
            {
                Rectangle subArea = new Rectangle(x, y, width + 2, height + 2);
                regions.Add(new MiniRegion($"{Name}_{i}", ID + i + 1, subArea));
                x += gap + width + 2;
            }

            return regions;
        }
    }
}