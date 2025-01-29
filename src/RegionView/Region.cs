using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Utilities;
using TShockAPI;

namespace RegionView;

public class Region
{
    private Tile[]? RealTiles;
    public const int MaximumSize = 256;

    public Rectangle Area;
    public Rectangle ShowArea;

    public string Name { get; }

    public byte Color { get; }

    public bool Command { get; set; }

    public Region(string name, Rectangle area, bool command = true)
    {
        this.Name = name;
        this.Area = area;
        this.ShowArea = area;
        this.Command = command;

        var total = 0;
        for (var i = 0; i < name.Length; i++)
        {
            total += name[i];
        }

        this.Color = (byte) ((total % 12) + 13);
    }

    public void CalculateArea(TSPlayer tPlayer)
    {
        this.ShowArea = this.Area;

        // If the region is large, only part of its border will be shown.
        if (this.ShowArea.Width >= MaximumSize)
        {
            this.ShowArea.X = (int) (tPlayer.X / 16) - (MaximumSize / 2);
            this.ShowArea.Width = MaximumSize - 1;

            if (this.ShowArea.Left < this.Area.Left)
            {
                this.ShowArea.X = this.Area.Left;
            }
            else if (this.ShowArea.Right > this.Area.Right)
            {
                this.ShowArea.X = this.Area.Right - (MaximumSize - 1);
            }
        }
        if (this.ShowArea.Height >= MaximumSize)
        {
            this.ShowArea.Y = (int) (tPlayer.Y / 16) - (MaximumSize / 2);
            this.ShowArea.Height = MaximumSize - 1;

            if (this.ShowArea.Top < this.Area.Top)
            {
                this.ShowArea.Y = this.Area.Top;
            }
            else if (this.ShowArea.Bottom > this.Area.Bottom)
            {
                this.ShowArea.Y = this.Area.Bottom - (MaximumSize - 1);
            }
        }

        // Ensure the region boundary is within the world.
        if (this.ShowArea.Left < 1)
        {
            this.ShowArea.X = 1;
        }
        else if (this.ShowArea.Left >= Main.maxTilesX - 1)
        {
            this.ShowArea.X = Main.maxTilesX - 1;
        }

        if (this.ShowArea.Top < 1)
        {
            this.ShowArea.Y = 1;
        }
        else if (this.ShowArea.Top >= Main.maxTilesY - 1)
        {
            this.ShowArea.Y = Main.maxTilesY - 1;
        }

        if (this.ShowArea.Right >= Main.maxTilesX - 1)
        {
            this.ShowArea.Width = Main.maxTilesX - this.ShowArea.X - 2;
        }

        if (this.ShowArea.Bottom >= Main.maxTilesY - 1)
        {
            this.ShowArea.Height = Main.maxTilesY - this.ShowArea.Y - 2;
        }
    }

    /// <summary>Spawns fake tiles for the region border.</summary>
    /// <exception cref="InvalidOperationException">Fake tiles have already been set, which would cause a desync.</exception>
    public void SetFakeTiles()
    {
        int d; var index = 0;

        if (this.RealTiles != null)
        {
            throw new InvalidOperationException(GetString("该区域已设置虚拟图块。"));
        }

        // Initialise the temporary tile array.
        this.RealTiles = this.ShowArea.Width == 0
            ? (new Tile[this.ShowArea.Height + 1])
            : this.ShowArea.Height == 0 ? (new Tile[this.ShowArea.Width + 1]) : (new Tile[(this.ShowArea.Width + this.ShowArea.Height) * 2]);

        // Top boundary
        if (this.ShowArea.Top == this.Area.Top)
        {
            for (d = 0; d <= this.ShowArea.Width; d++)
            {
                this.SetFakeTile(index++, this.ShowArea.Left + d, this.ShowArea.Top);
            }
        }
        // East boundary
        if (this.ShowArea.Right == this.Area.Right)
        {
            for (d = 1; d <= this.ShowArea.Height; d++)
            {
                this.SetFakeTile(index++, this.ShowArea.Right, this.ShowArea.Top + d);
            }
        }
        // West boundary
        if (this.ShowArea.Width > 0 && this.ShowArea.Left == this.Area.Left)
        {
            for (d = 1; d <= this.ShowArea.Height; d++)
            {
                this.SetFakeTile(index++, this.ShowArea.Left, this.ShowArea.Top + d);
            }
        }
        // Bottom boundary
        if (this.ShowArea.Height > 0 && this.ShowArea.Bottom == this.Area.Bottom)
        {
            for (d = 1; d < this.ShowArea.Width; d++)
            {
                this.SetFakeTile(index++, this.ShowArea.Left + d, this.ShowArea.Bottom);
            }
        }
    }

    /// <summary>Removes fake tiles for the region, reverting to the real tiles.</summary>
    /// <exception cref="InvalidOperationException">Fake tiles have not been set.</exception>
    public void UnsetFakeTiles()
    {
        int d; var index = 0;

        if (this.RealTiles == null)
        {
            throw new InvalidOperationException(GetString("区域未设置虚拟图块。"));
        }

        // Top boundary
        if (this.ShowArea.Top == this.Area.Top)
        {
            for (d = 0; d <= this.ShowArea.Width; d++)
            {
                this.UnsetFakeTile(index++, this.ShowArea.Left + d, this.ShowArea.Top);
            }
        }
        // East boundary
        if (this.ShowArea.Right == this.Area.Right)
        {
            for (d = 1; d <= this.ShowArea.Height; d++)
            {
                this.UnsetFakeTile(index++, this.ShowArea.Right, this.ShowArea.Top + d);
            }
        }
        // West boundary
        if (this.ShowArea.Width > 0 && this.ShowArea.Left == this.Area.Left)
        {
            for (d = 1; d <= this.ShowArea.Height; d++)
            {
                this.UnsetFakeTile(index++, this.ShowArea.Left, this.ShowArea.Top + d);
            }
        }
        // Bottom boundary
        if (this.ShowArea.Height > 0 && this.ShowArea.Bottom == this.Area.Bottom)
        {
            for (d = 1; d < this.ShowArea.Width; d++)
            {
                this.UnsetFakeTile(index++, this.ShowArea.Left + d, this.ShowArea.Bottom);
            }
        }

        this.RealTiles = null;
    }

    /// <summary>Adds a single fake tile. If a tile exists, this will replace it with a painted clone. Otherwise, this will place an inactive magical ice tile with the same paint.</summary>
    /// <param name="index">The index in the realTile array into which to store the existing tile</param>
    /// <param name="x">The x coordinate of the tile position</param>
    /// <param name="y">The y coordinate of the tile position</param>
    public void SetFakeTile(int index, int x, int y)
    {
        if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
        {
            return;
        }

        if (this.RealTiles == null)
        {
            throw new InvalidOperationException(GetString("区域尚未设置虚拟图块。"));
        }

        ITile fakeTile;
        if (Main.tile[x, y] == null)
        {
            fakeTile = new Tile();
        }
        else
        {
            // As of API version 1.22, Main.tile.get now only returns a link to the tile data heap, and the tile was getting lost at Main.tile[x, y] = fakeTile.
            // This is why we actually have to copy the tile now.
            this.RealTiles[index] = new Tile(Main.tile[x, y]);
            fakeTile = Main.tile[x, y];
        }

        if (this.RealTiles[index] != null && this.RealTiles[index].active())
        {
            // There's already a tile there; apply paint.
            if (fakeTile.type == Terraria.ID.TileID.RainbowBrick)
            {
                fakeTile.type = Terraria.ID.TileID.GrayBrick;
            }

            fakeTile.color(this.Color);
        }
        else
        {
            // There isn't a tile there; place an ice block.
            Main.rand ??= new UnifiedRandom();
            fakeTile.active(true);
            fakeTile.inActive(true);
            fakeTile.type = Terraria.ID.TileID.MagicalIceBlock;
            fakeTile.frameX = (short) (162 + (Main.rand.Next(0, 2) * 18));
            fakeTile.frameY = 54;
            fakeTile.color(this.Color);
        }
    }

    public void UnsetFakeTile(int index, int x, int y)
    {
        if (this.RealTiles == null)
        {
            throw new InvalidOperationException(GetString("区域尚未设置虚拟图块。"));
        }

        if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
        {
            return;
        }

        Main.tile[x, y] = this.RealTiles[index];
    }

    public void Refresh(TSPlayer player)
    {
        // Due to the way the Rectangle class works, the Width and Height values are one tile less than the actual dimensions of the region.
        if (this.ShowArea.Width <= 3 || this.ShowArea.Height <= 3)
        {
            player.SendData(PacketTypes.TileSendSection, "", this.ShowArea.Left - 1, this.ShowArea.Top - 1, this.ShowArea.Width + 3, this.ShowArea.Height + 3, 0);
        }
        else
        {
            if (this.ShowArea.Top == this.Area.Top)
            {
                player.SendData(PacketTypes.TileSendSection, "", this.ShowArea.Left - 1, this.ShowArea.Top - 1, this.ShowArea.Width + 3, 3, 0);
            }

            if (this.ShowArea.Left == this.Area.Left)
            {
                player.SendData(PacketTypes.TileSendSection, "", this.ShowArea.Left - 1, this.ShowArea.Top + 2, 3, this.ShowArea.Height, 0);
            }

            if (this.ShowArea.Right == this.Area.Right)
            {
                player.SendData(PacketTypes.TileSendSection, "", this.ShowArea.Right - 1, this.ShowArea.Top + 2, 3, this.ShowArea.Height, 0);
            }

            if (this.ShowArea.Bottom == this.Area.Bottom)
            {
                player.SendData(PacketTypes.TileSendSection, "", this.ShowArea.Left + 2, this.ShowArea.Bottom - 1, this.ShowArea.Width - 3, 3, 0);
            }
        }

        player.SendData(PacketTypes.TileFrameSection, "", this.ShowArea.Left / 200, this.ShowArea.Top / 150, this.ShowArea.Right / 200, this.ShowArea.Bottom / 150, 0);
    }
}