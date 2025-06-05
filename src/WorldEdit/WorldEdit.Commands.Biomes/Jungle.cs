namespace WorldEdit.Commands.Biomes;

public class Jungle : Biome
{
	public override int Dirt => 59;

	public override int[] Grass => new int[1] { 60 };

	public override int Stone => 1;

	public override int Ice => 1;

	public override int Clay => 40;

	public override int Sand => 53;

	public override int HardenedSand => 397;

	public override int Sandstone => 396;

	public override int Plants => 61;

	public override int TallPlants => 74;

	public override int Vines => 62;

	public override int Thorn => 69;

	public override ushort DirtWall => 15;

	public override ushort DirtWallUnsafe => 15;

	public override ushort CaveWall => 15;

	public override ushort DirtWallUnsafe1 => 15;

	public override ushort DirtWallUnsafe2 => 15;

	public override ushort DirtWallUnsafe3 => 15;

	public override ushort DirtWallUnsafe4 => 15;

	public override ushort StoneWall => 1;

	public override ushort HardenedSandWall => 216;

	public override ushort SandstoneWall => 187;

	public override ushort GrassWall => 67;

	public override ushort GrassWallUnsafe => 64;

	public override ushort FlowerWall => 68;

	public override ushort FlowerWallUnsafe => 65;

	public override ushort CaveWall1 => 204;

	public override ushort CaveWall2 => 205;

	public override ushort CaveWall3 => 206;

	public override ushort CaveWall4 => 207;
}
