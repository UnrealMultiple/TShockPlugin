using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Global
{
    public readonly struct TileSection
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public int Left => X;
        public int Right => X + Width;
        public int Top => Y;
        public int Bottom => Y + Height;
        public Point LeftTop => new Point(Left, Top);
        public Point LeftBottom => new Point(Left, Bottom);
        public Point RightTop => new Point(Right, Top);
        public Point RightBottom => new Point(Right, Bottom);
        public int CenterX => X + Width / 2;
        public int CenterY => Y + Height / 2;
        public int Size => Width * Height;

        public ITile this[int x, int y]
        {
            get => Main.tile[X + x, Y + y];
            set => Main.tile[X + x, Y + y] = value;
        }

        public ITile this[Point point]
        {
            get => this[point.X, point.Y];
            set => this[point.X, point.Y] = value;
        }

        public TileSection(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public TileSection(int[] clover_s) : this(clover_s[0], clover_s[1], clover_s[2] - clover_s[0], clover_s[3] - clover_s[1])
        {
        }

        public TileSection(Point point) : this(point.X, point.Y, 1, 1)
        {
        }

        public int CountPlayers()
        {
            int num = 0;
            foreach (TSPlayer tsplayer in TShock.Players)
            {
                if (tsplayer != null && tsplayer.Active && InRange(tsplayer))
                {
                    num++;
                }
            }
            return num;
        }

        public void UpdateToPlayer(int who = -1)
        {
            Replenisher.UpdateSection(X, Y, X + Width, Y + Height, who);
        }

        public bool InRange(Point point)
        {
            int num = point.X - X;
            int num2 = point.Y - Y;
            return 0 <= num && num <= Width && 0 <= num2 && num2 <= Height;
        }

        public bool InRange(int x, int y)
        {
            return InRange(new Point(x, y));
        }

        public bool InRange(in TileSection sec)
        {
            return Left <= sec.Left && sec.Right <= Right && Top <= sec.Top && sec.Bottom <= Bottom;
        }

        public bool InRange(Rectangle rect)
        {
            Point center = rect.Center;
            int num = center.X - X;
            int num2 = center.Y - Y;
            return rect.Width / 2 <= num && num <= Width - rect.Width / 2 && rect.Height / 2 <= num2 && num2 <= Height - rect.Height / 2;
        }

        public bool InRange(Entity entity)
        {
            Rectangle hitbox = entity.Hitbox;
            hitbox.X /= 16;
            hitbox.Y /= 16;
            hitbox.Width /= 16;
            hitbox.Height /= 16;
            return InRange(hitbox);
        }

        public bool InRange(Chest chest)
        {
            return InRange(chest.x, chest.y);
        }

        public bool InRange(TSPlayer player)
        {
            return InRange(player.TPlayer);
        }

        public bool Intersect(in TileSection sec)
        {
            return Math.Abs(CenterX - sec.CenterX) < Width / 2 + sec.Width / 2 && Math.Abs(CenterY - sec.CenterY) < Height / 2 + sec.Height / 2;
        }

        public bool CanPlaceEntity(Point point, Entity entity)
        {
            int num = (int)Math.Ceiling(entity.width / 16.0);
            int num2 = (int)Math.Ceiling(entity.height / 16.0);
            if (point.X + num > Width)
            {
                return false;
            }
            if (point.Y + num2 > Height)
            {
                return false;
            }
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    if (this[point.X + i, point.Y + j].active())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public TileSection SubSection(int x, int y, int width, int height)
        {
            return new TileSection(X + x, Y + y, width, height);
        }

        public TileSection SubSection(Rectangle rect)
        {
            return SubSection(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void KillTiles(int x, int y, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.KillTile(X + x + i, Y + y + j, false, false, false);
                }
            }
        }

        public void KillTiles(Rectangle rect)
        {
            KillTiles(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void KillAllTile()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    WorldGen.KillTile(X + i, Y + j, false, false, false);
                }
            }
        }

        public void TurnToAir()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    this[i, j].active(false);
                }
            }
        }

        public static bool operator ==(in TileSection left, in TileSection right)
        {
            return left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
        }

        public static bool operator !=(in TileSection left, in TileSection right)
        {
            return !(left == right);
        }

        public Point LeftJoint => new Point(Left, CenterY);

        public Point RightJoint => new Point(Right, CenterY);

        public Point RandomJoint(Random rad)
        {
            return rad.NextDouble() >= 0.5 ? RightJoint : LeftJoint;
        }

        public override string ToString()
        {
            return string.Format("{{ Left: {0}, Right: {1}, Top: {2}, Bottom: {3} }}", Left, Right, Top, Bottom);
        }
    }
}
