using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Utilities;
using TShockAPI;

namespace RegionView
{
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
			Name = name;
			Area = area;
			ShowArea = area;
			Command = command;

			var total = 0;
			for (var i = 0; i < name.Length; i++) total += name[i];
			Color = (byte)(total % 12 + 13);
		}

		public void CalculateArea(TSPlayer tPlayer)
		{
			ShowArea = Area;

			// If the region is large, only part of its border will be shown.
			if (ShowArea.Width >= MaximumSize)
			{
				ShowArea.X = (int)(tPlayer.X / 16) - MaximumSize / 2;
				ShowArea.Width = MaximumSize - 1;

				if (ShowArea.Left < Area.Left) 
					ShowArea.X = Area.Left;
				else if (ShowArea.Right > Area.Right) 
					ShowArea.X = Area.Right - (MaximumSize - 1);
			}
			if (ShowArea.Height >= MaximumSize)
			{
				ShowArea.Y = (int)(tPlayer.Y / 16) - MaximumSize / 2;
				ShowArea.Height = MaximumSize - 1;

				if (ShowArea.Top < Area.Top) 
					ShowArea.Y = Area.Top;
				else if (ShowArea.Bottom > Area.Bottom) 
					ShowArea.Y = Area.Bottom - (MaximumSize - 1);
			}

			// Ensure the region boundary is within the world.
			if (ShowArea.Left < 1) 
				ShowArea.X = 1;
			else if (ShowArea.Left >= Main.maxTilesX - 1) 
				ShowArea.X = Main.maxTilesX - 1;

			if (ShowArea.Top < 1) 
				ShowArea.Y = 1;
			else if (ShowArea.Top >= Main.maxTilesY - 1) 
				ShowArea.Y = Main.maxTilesY - 1;

			if (ShowArea.Right >= Main.maxTilesX - 1) 
				ShowArea.Width = Main.maxTilesX - ShowArea.X - 2;

			if (ShowArea.Bottom >= Main.maxTilesY - 1) 
				ShowArea.Height = Main.maxTilesY - ShowArea.Y - 2;
		}

		/// <summary>Spawns fake tiles for the region border.</summary>
		/// <exception cref="InvalidOperationException">Fake tiles have already been set, which would cause a desync.</exception>
		public void SetFakeTiles()
		{
			int d; var index = 0;

            if (RealTiles != null) throw new InvalidOperationException("该区域已设置虚拟图块。");

            // Initialise the temporary tile array.
            if (ShowArea.Width == 0)
				RealTiles = new Tile[ShowArea.Height + 1];
			else if (ShowArea.Height == 0)
				RealTiles = new Tile[ShowArea.Width + 1];
			else
				RealTiles = new Tile[(ShowArea.Width + ShowArea.Height) * 2];

			// Top boundary
			if (ShowArea.Top == Area.Top)
				for (d = 0; d <= ShowArea.Width; d++) 
					SetFakeTile(index++, ShowArea.Left + d, ShowArea.Top);
			// East boundary
			if (ShowArea.Right == Area.Right)
				for (d = 1; d <= ShowArea.Height; d++) 
					SetFakeTile(index++, ShowArea.Right, ShowArea.Top + d);
			// West boundary
			if (ShowArea.Width > 0 && ShowArea.Left == Area.Left)
				for (d = 1; d <= ShowArea.Height; d++) 
					SetFakeTile(index++, ShowArea.Left, ShowArea.Top + d);
			// Bottom boundary
			if (ShowArea.Height > 0 && ShowArea.Bottom == Area.Bottom)
				for (d = 1; d < ShowArea.Width; d++) 
					SetFakeTile(index++, ShowArea.Left + d, ShowArea.Bottom);
		}

		/// <summary>Removes fake tiles for the region, reverting to the real tiles.</summary>
		/// <exception cref="InvalidOperationException">Fake tiles have not been set.</exception>
		public void UnsetFakeTiles()
		{
			int d; var index = 0;

			if (RealTiles == null)
                throw new InvalidOperationException("区域未设置虚拟图块。");

            // Top boundary
            if (ShowArea.Top == Area.Top)
				for (d = 0; d <= ShowArea.Width; d++) 
					UnsetFakeTile(index++, ShowArea.Left + d, ShowArea.Top);
			// East boundary
			if (ShowArea.Right == Area.Right)
				for (d = 1; d <= ShowArea.Height; d++) 
					UnsetFakeTile(index++, ShowArea.Right, ShowArea.Top + d);
			// West boundary
			if (ShowArea.Width > 0 && ShowArea.Left == Area.Left)
				for (d = 1; d <= ShowArea.Height; d++) 
					UnsetFakeTile(index++, ShowArea.Left, ShowArea.Top + d);
			// Bottom boundary
			if (ShowArea.Height > 0 && ShowArea.Bottom == Area.Bottom)
				for (d = 1; d < ShowArea.Width; d++) 
					UnsetFakeTile(index++, ShowArea.Left + d, ShowArea.Bottom);

			RealTiles = null;
		}

		/// <summary>Adds a single fake tile. If a tile exists, this will replace it with a painted clone. Otherwise, this will place an inactive magical ice tile with the same paint.</summary>
		/// <param name="index">The index in the realTile array into which to store the existing tile</param>
		/// <param name="x">The x coordinate of the tile position</param>
		/// <param name="y">The y coordinate of the tile position</param>
		public void SetFakeTile(int index, int x, int y)
		{
			if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY) 
				return;

			if (RealTiles == null)
                throw new InvalidOperationException("区域尚未设置虚拟图块。");

            ITile fakeTile;
			if (Main.tile[x, y] == null)
			{
				fakeTile = new Tile();
			}
			else
			{
				// As of API version 1.22, Main.tile.get now only returns a link to the tile data heap, and the tile was getting lost at Main.tile[x, y] = fakeTile.
				// This is why we actually have to copy the tile now.
				RealTiles[index] = new Tile(Main.tile[x, y]);
				fakeTile = Main.tile[x, y];
			}

			if (RealTiles[index] != null && RealTiles[index].active())
			{
				// There's already a tile there; apply paint.
				if (fakeTile.type == Terraria.ID.TileID.RainbowBrick) fakeTile.type = Terraria.ID.TileID.GrayBrick;

				fakeTile.color(Color);
			}
			else
			{
				// There isn't a tile there; place an ice block.
				if (Main.rand == null) Main.rand = new UnifiedRandom();
				fakeTile.active(true);
				fakeTile.inActive(true);
				fakeTile.type = Terraria.ID.TileID.MagicalIceBlock;
				fakeTile.frameX = (short)(162 + Main.rand.Next(0, 2) * 18);
				fakeTile.frameY = 54;
				fakeTile.color(Color);
			}
		}

		public void UnsetFakeTile(int index, int x, int y)
		{
			if (RealTiles == null)
                throw new InvalidOperationException("区域尚未设置虚拟图块。");

            if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY) 
				return;

			Main.tile[x, y] = RealTiles[index];
		}

		public void Refresh(TSPlayer player)
		{
			// Due to the way the Rectangle class works, the Width and Height values are one tile less than the actual dimensions of the region.
			if (ShowArea.Width <= 3 || ShowArea.Height <= 3)
			{
				player.SendData(PacketTypes.TileSendSection, "", ShowArea.Left - 1, ShowArea.Top - 1, ShowArea.Width + 3, ShowArea.Height + 3, 0);
			}
			else
			{
				if (ShowArea.Top == Area.Top)
					player.SendData(PacketTypes.TileSendSection, "", ShowArea.Left - 1, ShowArea.Top - 1, ShowArea.Width + 3, 3, 0);

				if (ShowArea.Left == Area.Left)
					player.SendData(PacketTypes.TileSendSection, "", ShowArea.Left - 1, ShowArea.Top + 2, 3, ShowArea.Height, 0);

				if (ShowArea.Right == Area.Right)
					player.SendData(PacketTypes.TileSendSection, "", ShowArea.Right - 1, ShowArea.Top + 2, 3, ShowArea.Height, 0);

				if (ShowArea.Bottom == Area.Bottom)
					player.SendData(PacketTypes.TileSendSection, "", ShowArea.Left + 2, ShowArea.Bottom - 1, ShowArea.Width - 3, 3, 0);
			}

			player.SendData(PacketTypes.TileFrameSection, "", (ShowArea.Left / 200), (ShowArea.Top / 150), (ShowArea.Right / 200), (ShowArea.Bottom / 150), 0);
		}
    }
}
