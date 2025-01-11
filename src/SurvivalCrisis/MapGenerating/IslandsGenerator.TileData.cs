using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using Terraria;
// using OTAPI.Tile;
using Microsoft.Xna.Framework;

namespace SurvivalCrisis.MapGenerating
{
	public partial class IslandsGenerator
	{
		private struct TileData
		{
			public ushort Type;
			public ushort Wall;
			public byte Liquid;
			public short FrameX;
			public short FrameY;
			public byte BTileHeader1;
			public byte BTileHeader2;
			public byte BTileHeader3;
			public ushort STileHeader;

			public ITile ToTile()
			{
				return new Tile()
				{
					sTileHeader = STileHeader,
					bTileHeader = BTileHeader1,
					bTileHeader2 = BTileHeader2,
					bTileHeader3 = BTileHeader3,
					type = Type,
					wall = Wall,
					liquid = Liquid,
					frameX = FrameX,
					frameY = FrameY
				};
			}
			public static TileData FromTile(ITile tile)
			{
				return new TileData
				{
					Type = tile.type,
					Wall = tile.wall,
					Liquid = tile.liquid,
					FrameX = tile.frameX,
					FrameY = tile.frameY,
					BTileHeader1 = tile.bTileHeader,
					BTileHeader2 = tile.bTileHeader2,
					BTileHeader3 = tile.bTileHeader3,
					STileHeader = tile.sTileHeader
				};
			}
		}

		internal class TileBlockData
		{
			private const int BufferSize = 1048576;

			public string Identifier
			{
				get;
			}
			public int Width
			{
				get;
			}
			public int Height
			{
				get;
			}
			private TileData[,] tiles;
			internal TileBlockData(int width, int height, string identifier = null)
			{
				Width = width;
				Height = height;
				tiles = new TileData[Width, Height];
				Identifier = identifier ?? SurvivalCrisis.Rand.GenerateStr(10);
			}
			internal void AffixTo(in TileSection section, bool replaceTile = false)
			{
				if (section.Width < Width || section.Height < Height)
				{
					throw new Exception($"空间过小: 所需({Width}, {Height})，给予({section.Width}, {section.Height})");
				}
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Height; y++)
					{
						var tile = tiles[x, y].ToTile();
						if (section[x, y] == null)
						{
							section[x, y] = tile;
						}
						else
						{
							if (!section[x, y].active() || (replaceTile && tile.active()))
							{
								section[x, y].type = tile.type;
								section[x, y].active(tile.active());
								section[x, y].color(tile.color());
								section[x, y].slope(tile.slope());
							}
							if (section[x, y].wall == 0)
							{
								section[x, y].wall = tile.wall;
								section[x, y].wallColor(tile.wallColor());
							}
							section[x, y].liquid = tile.liquid;
							section[x, y].frameX = tile.frameX;
							section[x, y].frameY = tile.frameY;
							section[x, y].liquidType(tile.liquidType());
							section[x, y].wire(tile.wire());
							section[x, y].wire2(tile.wire2());
							section[x, y].wire3(tile.wire3());
							section[x, y].wire4(tile.wire4());
						}
					}
				}
			}
			internal void Save()
			{
				SaveTo(Identifier);
			}
			internal void SaveTo(string identifier)
			{
				string savePath = Path.Combine(SurvivalCrisis.IslandsPath, identifier + ".sec");
				var bs = new BufferedStream(new GZipStream(File.Open(savePath, FileMode.Create), CompressionMode.Compress), BufferSize);
				using var writer = new BinaryWriter(bs);
				writer.Write(Width);
				writer.Write(Height);
				for (int i = 0; i < Width; i++)
				{
					for (int j = 0; j < Height; j++)
					{
						writer.Write(tiles[i, j].BTileHeader1);
						writer.Write(tiles[i, j].BTileHeader2);
						writer.Write(tiles[i, j].BTileHeader3);
						writer.Write(tiles[i, j].STileHeader);
						writer.Write(tiles[i, j].Type);
						writer.Write(tiles[i, j].Wall);
						writer.Write(tiles[i, j].Liquid);
						writer.Write(tiles[i, j].FrameX);
						writer.Write(tiles[i, j].FrameY);
					}
				}
			}
			internal static TileBlockData FromFile(string path)
			{
				var file = new FileInfo(path);
				var stream = file.OpenRead();
				var bs = new BufferedStream(new GZipStream(stream, CompressionMode.Decompress), BufferSize);
				using var reader = new BinaryReader(bs);
				var width = reader.ReadInt32();
				var height = reader.ReadInt32();
				var data = new TileBlockData(width, height, file.Name.Substring(0, file.Name.IndexOf('.')));
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						data.tiles[i, j].BTileHeader1 = reader.ReadByte();
						data.tiles[i, j].BTileHeader2 = reader.ReadByte();
						data.tiles[i, j].BTileHeader3 = reader.ReadByte();
						data.tiles[i, j].STileHeader = reader.ReadUInt16();
						data.tiles[i, j].Type = reader.ReadUInt16();
						data.tiles[i, j].Wall = reader.ReadUInt16();
						data.tiles[i, j].Liquid = reader.ReadByte();
						data.tiles[i, j].FrameX = reader.ReadInt16();
						data.tiles[i, j].FrameY = reader.ReadInt16();
					}
				}
				return data;
			}
			internal static TileBlockData FromMap(in TileSection section)
			{
				var data = new TileBlockData(section.Width, section.Height);
				for (int x = 0; x < section.Width; x++)
				{
					for (int y = 0; y < section.Height; y++)
					{
						data.tiles[x, y] = TileData.FromTile(section[x, y]);
					}
				}
				return data;
			}
			internal static TileBlockData BFSBlock(Point point)
			{
				int left = point.X;
				int right = point.X + 1;
				int top = point.Y;
				int bottom = point.Y + 1;

				#region BFS
				var visited = new Dictionary<Point, bool>();
				var queue = new Queue<Point>();
				queue.Enqueue(point);
				Point current;
				bool Avaiable(Point p)
				{
					if (visited.ContainsKey(p))
					{
						return false;
					}
					if (p.X < 0 || p.X > Main.maxTilesX || p.Y < 0 || p.Y > Main.maxTilesY)
					{
						return false;
					}
					var tile = Main.tile[p.X, p.Y];
					return tile.active() || tile.wall != 0 || tile.liquid > 0;
				}
				visited.Add(point, true);
				while (queue.Count != 0)
				{
					current = queue.Dequeue();
					Point next;
					next = new Point(current.X - 1, current.Y - 1);
					if (Avaiable(next))
					{
						visited.Add(next, true);
						queue.Enqueue(next);
					}
					next = new Point(current.X + 0, current.Y - 1);
					if (Avaiable(next))
					{
						visited.Add(next, true);
						queue.Enqueue(next);
					}
					next = new Point(current.X + 1, current.Y - 1);
					if (Avaiable(next))
					{
						visited.Add(next, true);
						queue.Enqueue(next);
					}
					next = new Point(current.X + 1, current.Y + 0);
					if (Avaiable(next))
					{
						visited.Add(next, true);
						queue.Enqueue(next);
					}
					next = new Point(current.X + 1, current.Y + 1);
					if (Avaiable(next))
					{
						visited.Add(next, true);
						queue.Enqueue(next);
					}
					next = new Point(current.X + 0, current.Y + 1);
					if (Avaiable(next))
					{
						visited.Add(next, true);
						queue.Enqueue(next);
					}
					next = new Point(current.X - 1, current.Y + 1);
					if (Avaiable(next))
					{
						visited.Add(next, true);
						queue.Enqueue(next);
					}
					next = new Point(current.X - 1, current.Y + 0);
					if (Avaiable(next))
					{
						visited.Add(next, true);
						queue.Enqueue(next);
					}
					if (current.X < left)
					{
						left = current.X;
					}
					if (right < current.X + 1)
					{
						right = current.X + 1;
					}
					if (current.Y < top)
					{
						top = current.Y;
					}
					if (bottom < current.Y + 1)
					{
						bottom = current.Y + 1;
					}
				}
				#endregion
				var section = new TileSection(left, top, right - left, bottom - top);
				return FromMap(section);
			}
		}
	}
}
