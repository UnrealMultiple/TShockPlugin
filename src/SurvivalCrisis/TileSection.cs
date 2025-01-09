using Microsoft.Xna.Framework;
// using OTAPI.Tile;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace SurvivalCrisis
{
	/// <summary>
	/// 表示一个方块区
	/// </summary>
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

		public Point CenterBottom => new Point(CenterX, Bottom);

		public int CenterX => X + Width / 2;
		public int CenterY => Y + Height / 2;

		public Point Center => new Point(CenterX, CenterY);

		public int Size => Width * Height;

		public ITile this[int x, int y]
		{
			get
			{
#if DEBUG
				System.Diagnostics.Debug.Assert(0 <= x && x < Width,  "x out of range");
				System.Diagnostics.Debug.Assert(0 <= y && y < Height, "y out of range");
#endif
				return Main.tile[X + x, Y + y];
			}
			set
			{
#if DEBUG
				System.Diagnostics.Debug.Assert(0 <= x && x < Width,  "x out of range");
				System.Diagnostics.Debug.Assert(0 <= y && y < Height, "y out of range");
#endif
				Main.tile[X + x, Y + y] = value;
			}
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clover_s">[0] x1 [1] y1 [2] x2 [3] y2</param>
		public TileSection(int[] clover_s) : this(clover_s[0], clover_s[1], clover_s[2] - clover_s[0], clover_s[3] - clover_s[1])
		{

		}

		public TileSection(Point point) : this(point.X, point.Y, 1, 1)
		{

		}

		public int CountPlayers(bool ignoreWatcher = false)
		{
			int count = 0;
			foreach (var player in TShock.Players)
			{
				if (player?.Active ?? false)
				{
					if (InRange(player) && (!ignoreWatcher || SurvivalCrisis.Instance.Players[player.Index].Identity != PlayerIdentity.Watcher))
					{
						count++;
					}
				}
			}
			return count;
		}

		public void UpdateToPlayer(int who = -1)
		{
			Replenisher.UpdateSection(X, Y, X + Width, Y + Height, who);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="point">这是物块坐标系里的坐标</param>
		/// <returns></returns>
		public bool InRange(Point point)
		{
			var Dx = point.X - X;
			var Dy = point.Y - Y;
			return
				0 <= Dx && Dx < Width &&
				0 <= Dy && Dy < Height;
		}
		public bool InRange(int x, int y)
		{
			return InRange(new Point(x, y));
		}
		public bool InRange(in TileSection sec)
		{
			return
				Left <= sec.Left && sec.Right <= Right &&
				Top <= sec.Top && sec.Bottom <= Bottom;
		}
		public bool InRange(Rectangle rect)
		{
			var point = rect.Center;
			var Dx = point.X - X;
			var Dy = point.Y - Y;
			return
				rect.Width / 2 <= Dx && Dx <= Width - rect.Width / 2 &&
				rect.Height / 2 <= Dy && Dy <= Height - rect.Height / 2;
		}
		public bool InRange(Entity entity)
		{
			var rect = entity.Hitbox;
			rect.X /= 16;
			rect.Y /= 16;
			rect.Width /= 16;
			rect.Height /= 16;
			return InRange(rect);
		}
		public bool InRange(Chest chest)
		{
			return InRange(chest.x, chest.y);
		}
		public bool InRange(TSPlayer player)
		{
			return InRange(player.TPlayer);
		}
		public bool InRange(GamePlayer player)
		{
			return InRange(player.TPlayer);
		}

		public bool Intersect(in TileSection sec)
		{
			return
				Math.Abs(CenterX - sec.CenterX) < Width / 2 + sec.Width / 2 &&
				Math.Abs(CenterY - sec.CenterY) < Height / 2 + sec.Height / 2;
		}

		/// <summary>
		/// 填充一层
		/// </summary>
		/// <param name="height">填充层的高度(0到Height)</param>
		/// <param name="type">物块ID</param>
		public void FillLine(int height, ushort type)
		{
			Replenisher.SpecialLine(X, Y + height, Width, type);
		}

		public void PlaceTileAt(int i, int j, ushort type, bool netUpdate = false)
		{
			PlaceTileAt(new Point(i, j), type, netUpdate);
		}
		public void PlaceTileAt(Point point, ushort type, bool netUpdate = false)
		{
			point.X += X;
			point.Y += Y;
			if (netUpdate)
			{
				Replenisher.PlaceTileAndUpdate(point, type);
			}
			else
			{
				WorldGen.PlaceTile(point.X, point.Y, type);
			}
		}
		public void PlaceWallAt(int i, int j, ushort type, bool netUpdate = false)
		{
			PlaceWallAt(new Point(i, j), type, netUpdate);
		}
		public void PlaceWallAt(Point point, ushort type, bool netUpdate = false)
		{
			point.X += X;
			point.Y += Y;
			if (netUpdate)
			{
				Replenisher.PlaceWallAndUpdate(point, type);
			}
			else
			{
				WorldGen.PlaceWall(point.X, point.Y, type);
			}
		}

		public void TryRemoveTile(int i, int j)
		{
			if (i < 0 || j < 0 || i >= Width || j >= Height)
			{
				return;
			}
			this[i, j].active(false);
		}

		public void TryPlaceWall(int i, int j, ushort type)
		{
			if (i < 0 || j < 0 || i >= Width || j >= Height)
			{
				return;
			}
			this[i, j].wall = type;
		}


		/*
		public void LoadFile(string name)
		{
			string path = Path.Combine(Environment.CurrentDirectory, "hunger", "maps", name);
			using var stream = new BufferedStream(new GZipStream(File.Open(path, FileMode.Open), CompressionMode.Decompress), 1048576);
			using var reader = new BinaryReader(stream);
			int width = reader.ReadInt32();
			int height = reader.ReadInt32();
#if DEBUG
			System.Diagnostics.Debug.Assert(width == Width, $"width({width}) != Width({Width})");
			System.Diagnostics.Debug.Assert(height == Height, $"height({height}) != Height({Height})");
#endif
			for (int i = X; i < X + width; i++)
			{
				for (int j = Y; j < Y + height; j++)
				{
					Main.tile[i, j] = reader.ReadTile();
					Main.tile[i, j].skipLiquid(true);
				}
			}
		}
		public void SaveFile(string name)
		{
			string path = Path.Combine(Environment.CurrentDirectory, "hunger", "maps", name);

			using (var stream = new GZipStream(File.OpenWrite(path), CompressionMode.Compress))
			using (var writer = new BinaryWriter(stream))
			{
				writer.Write(Width);
				writer.Write(Height);
				for (int i = 0; i < Width; i++)
				{
					for (int j = 0; j < Height; j++)
					{
						writer.Write(this[i, j]);
					}
				}
			}
		}
		*/
		/// <summary>
		/// 检测是否可以容纳Entity
		/// </summary>
		/// <param name="point">相对Section左上角的方块坐标</param>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool CanPlaceEntity(Point point, Entity entity)
		{
			int x = (int)Math.Ceiling(entity.width / 16.0);
			int y = (int)Math.Ceiling(entity.height / 16.0);
			if (point.X + x > Width)
			{
				return false;
			}
			if (point.Y + y > Height)
			{
				return false;
			}
			for (int i = 0; i < x; i++)
			{
				for (int j = 0; j < y; j++)
				{
					if (this[point.X + i, point.Y + j].active())
					{
						return false;
					}
				}
			}
			return true;
		}

		public TileSection SubSection(int x,int y,int width, int height)
		{
			return new TileSection(X + x, Y + y, width, height);
		}
		public TileSection SubSection(Rectangle rect)
		{
			return SubSection(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void KillTiles(int x, int y, int width, int height)
		{
#if DEBUG
			System.Diagnostics.Debug.Assert(0 <= x && x < Width, "x out of range");
			System.Diagnostics.Debug.Assert(0 <= y && y < Height, "y out of range");
			System.Diagnostics.Debug.Assert(x + width <= Width, "x + width > Width");
			System.Diagnostics.Debug.Assert(Y + height <= Height, "y + height > Height");
#endif
			for (int i = 0; i <width; i++)
			for (int j = 0; j < height; j++)
			{
				WorldGen.KillTile(X + x + i, Y + y + j);
			}
		}
		public void KillTiles(Rectangle rect)
		{
			KillTiles(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public void KillAllTile()
		{
			for (int i = 0; i < Width; i++)
			for (int j = 0; j < Height; j++)
			{
				WorldGen.KillTile(X + i, Y + j);
			}
		}
		public void TurnToAir()
		{
			for (int i = 0; i < Main.chest.Length; i++)
			{
				var chest = Main.chest[i];
				if (chest != null && InRange(chest))
				{
					Main.chest[i] = null;
				}
			}
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				var npc = Main.npc[i];
				if (npc.active && InRange(npc))
				{
					npc.type = 0;
					npc.active = false;
				}
			}
			for (int i = 0; i < Width; i++)
			for (int j = 0; j < Height; j++)
			{
					this[i, j].active(false);
					this[i, j].wall = WallID.None;
					this[i, j].liquid = 0;
					this[i, j].color(0);
					this[i, j].wallColor(0);
				}
		}

		public void Fill(ushort tileType, ushort wallType = WallID.None)
		{
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					this[i, j].type = tileType;
					this[i, j].active(true);
					this[i, j].slope(0);
					this[i, j].wall = wallType;
				}
			}
		}

		public static bool operator ==(in TileSection left, in TileSection right)
		{
			return
				left.X == right.X &&
				left.Y == right.Y &&
				left.Width == right.Width &&
				left.Height == right.Height;
		}
		public static bool operator !=(in TileSection left, in TileSection right)
		{
			return !(left == right);
		}



		public Point LeftJoint { get { return new Point(Left, CenterY); } }
		public Point RightJoint { get { return new Point(Right, CenterY); } }
		public Point RandomJoint(Random rad)
		{
			return (rad.NextDouble() < 0.5) ? LeftJoint : RightJoint;
		}

		public string ToString2()
		{
			return $"({X}, {Y}) Width({Width}) Height({Height})";
		}
		public override string ToString()
		{
			return $"{{ Left: {Left}, Right: {Right}, Top: {Top}, Bottom: {Bottom} }}";
		}
	}
}
