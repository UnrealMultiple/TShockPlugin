using System;
using System.Linq;
using Terraria;

namespace WorldEdit.Commands.Biomes;

public class Biome
{
	public virtual int Dirt { get; }

	public virtual int[] Grass { get; }

	public virtual int Stone { get; }

	public virtual int Ice { get; }

	public virtual int Clay { get; }

	public virtual int Sand { get; }

	public virtual int HardenedSand { get; }

	public virtual int Sandstone { get; }

	public virtual int Plants { get; }

	public virtual int TallPlants { get; }

	public virtual int Vines { get; }

	public virtual int Thorn { get; }

	public int[] Tiles => new int[11]
	{
		Dirt, Stone, Ice, Clay, Sand, HardenedSand, Sandstone, Plants, TallPlants, Vines,
		Thorn
	}.Concat(Grass).ToArray();

	public virtual ushort DirtWall { get; }

	public virtual ushort StoneWall { get; }

	public virtual ushort HardenedSandWall { get; }

	public virtual ushort SandstoneWall { get; }

	public virtual ushort GrassWall { get; }

	public virtual ushort GrassWallUnsafe { get; }

	public virtual ushort FlowerWall { get; }

	public virtual ushort FlowerWallUnsafe { get; }

	public virtual ushort CaveWall1 { get; }

	public virtual ushort CaveWall2 { get; }

	public virtual ushort CaveWall3 { get; }

	public virtual ushort CaveWall4 { get; }

	public virtual ushort DirtWallUnsafe { get; }

	public virtual ushort DirtWallUnsafe1 { get; }

	public virtual ushort DirtWallUnsafe2 { get; }

	public virtual ushort DirtWallUnsafe3 { get; }

	public virtual ushort DirtWallUnsafe4 { get; }

	public virtual ushort CaveWall { get; }

	public ushort[] Walls => new ushort[17]
	{
		DirtWall, StoneWall, HardenedSandWall, SandstoneWall, GrassWall, GrassWallUnsafe, FlowerWall, FlowerWallUnsafe, CaveWall1, CaveWall2,
		CaveWall3, CaveWall4, DirtWallUnsafe, DirtWallUnsafe1, DirtWallUnsafe2, DirtWallUnsafe3, DirtWallUnsafe4
	};

	public bool ConvertTile(ITile Tile, Biome ToBiome)
	{
		if (Tile == null)
		{
			return false;
		}
		bool result = false;
		if (Tile.active())
		{
			int num = Array.FindIndex(Tiles, (int t) => t == Tile.type);
			if (num >= 0)
			{
				if (ToBiome.Tiles[num] == -1)
				{
					Tile.type = 0;
					Tile.frameX = -1;
					Tile.frameY = -1;
					Tile.active(false);
				}
				else
				{
					Tile.type = (ushort)ToBiome.Tiles[num];
				}
				result = true;
			}
		}
		if (Tile.wall > 0)
		{
			int num2 = Array.FindIndex(Walls, (ushort w) => w == Tile.wall);
			if (num2 >= 0 && ToBiome.Walls[num2] != 0)
			{
				Tile.wall = ToBiome.Walls[num2];
				result = true;
			}
		}
		return result;
	}
}
