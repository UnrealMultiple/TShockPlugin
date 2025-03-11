using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace ReverseWorld;

public readonly struct TileSection
{
    // 重写 Equals
    public override bool Equals(object? obj)
    {
        // 如果 obj 不是 TileSection 直接返回 false
        return obj is TileSection other && this == other;
    }

    // 重写 GetHashCode
    public override int GetHashCode()
    {
        // 使用系统的 HashCode.Combine 提高性能
        return HashCode.Combine(this.X, this.Y, this.Width, this.Height);
    }
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }

    public int Left => this.X;
    public int Right => this.X + this.Width;
    public int Top => this.Y;
    public int Bottom => this.Y + this.Height;
    public Point LeftTop => new Point(this.Left, this.Top);
    public Point LeftBottom => new Point(this.Left, this.Bottom);
    public Point RightTop => new Point(this.Right, this.Top);
    public Point RightBottom => new Point(this.Right, this.Bottom);
    public int CenterX => this.X + (this.Width / 2);
    public int CenterY => this.Y + (this.Height / 2);
    public int Size => this.Width * this.Height;

    public ITile this[int x, int y]
    {
        get => Main.tile[this.X + x, this.Y + y];
        set => Main.tile[this.X + x, this.Y + y] = value;
    }

    public ITile this[Point point]
    {
        get => this[point.X, point.Y];
        set => this[point.X, point.Y] = value;
    }

    public TileSection(int x, int y, int width, int height)
    {
        this.X = x;
        this.Y = y;
        this.Width = width;
        this.Height = height;
    }

    public TileSection(int[] clover_s) : this(clover_s[0], clover_s[1], clover_s[2] - clover_s[0], clover_s[3] - clover_s[1])
    {
    }

    public TileSection(Point point) : this(point.X, point.Y, 1, 1)
    {
    }

    public int CountPlayers()
    {
        var num = 0;
        foreach (var tsplayer in TShock.Players)
        {
            if (tsplayer != null && tsplayer.Active && this.InRange(tsplayer))
            {
                num++;
            }
        }
        return num;
    }

    public void UpdateToPlayer(int who = -1)
    {
        Replenisher.UpdateSection(this.X, this.Y, this.X + this.Width, this.Y + this.Height, who);
    }

    public bool InRange(Point point)
    {
        var num = point.X - this.X;
        var num2 = point.Y - this.Y;
        return 0 <= num && num <= this.Width && 0 <= num2 && num2 <= this.Height;
    }

    public bool InRange(int x, int y)
    {
        return this.InRange(new Point(x, y));
    }

    public bool InRange(in TileSection sec)
    {
        return this.Left <= sec.Left && sec.Right <= this.Right && this.Top <= sec.Top && sec.Bottom <= this.Bottom;
    }

    public bool InRange(Rectangle rect)
    {
        var center = rect.Center;
        var num = center.X - this.X;
        var num2 = center.Y - this.Y;
        return rect.Width / 2 <= num && num <= this.Width - (rect.Width / 2) && rect.Height / 2 <= num2 && num2 <= this.Height - (rect.Height / 2);
    }

    public bool InRange(Entity entity)
    {
        var hitbox = entity.Hitbox;
        hitbox.X /= 16;
        hitbox.Y /= 16;
        hitbox.Width /= 16;
        hitbox.Height /= 16;
        return this.InRange(hitbox);
    }

    public bool InRange(Chest chest)
    {
        return this.InRange(chest.x, chest.y);
    }

    public bool InRange(TSPlayer player)
    {
        return this.InRange(player.TPlayer);
    }

    public bool Intersect(in TileSection sec)
    {
        return Math.Abs(this.CenterX - sec.CenterX) < (this.Width / 2) + (sec.Width / 2) && Math.Abs(this.CenterY - sec.CenterY) < (this.Height / 2) + (sec.Height / 2);
    }

    public bool CanPlaceEntity(Point point, Entity entity)
    {
        var num = (int)Math.Ceiling(entity.width / 16.0);
        var num2 = (int)Math.Ceiling(entity.height / 16.0);
        if (point.X + num > this.Width)
        {
            return false;
        }
        if (point.Y + num2 > this.Height)
        {
            return false;
        }
        for (var i = 0; i < num; i++)
        {
            for (var j = 0; j < num2; j++)
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
        return new TileSection(this.X + x, this.Y + y, width, height);
    }

    public TileSection SubSection(Rectangle rect)
    {
        return this.SubSection(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public void KillTiles(int x, int y, int width, int height)
    {
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                WorldGen.KillTile(this.X + x + i, this.Y + y + j, false, false, false);
            }
        }
    }

    public void KillTiles(Rectangle rect)
    {
        this.KillTiles(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public void KillAllTile()
    {
        for (var i = 0; i < this.Width; i++)
        {
            for (var j = 0; j < this.Height; j++)
            {
                WorldGen.KillTile(this.X + i, this.Y + j, false, false, false);
            }
        }
    }

    public void TurnToAir()
    {
        for (var i = 0; i < this.Width; i++)
        {
            for (var j = 0; j < this.Height; j++)
            {
                this[i, j].active(false);
            }
        }
    }

    public static bool operator ==(in TileSection left, in TileSection right) => left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;

    public static bool operator !=(in TileSection left, in TileSection right) => !(left == right);

    public Point LeftJoint => new Point(this.Left, this.CenterY);

    public Point RightJoint => new Point(this.Right, this.CenterY);

    public Point RandomJoint(Random rad)
    {
        return rad.NextDouble() >= 0.5 ? this.RightJoint : this.LeftJoint;
    }

    public override string ToString()
    {
        return string.Format("{{ Left: {0}, Right: {1}, Top: {2}, Bottom: {3} }}", this.Left, this.Right, this.Top, this.Bottom);
    }
}
