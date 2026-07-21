namespace WorldEdit.Commands.Biomes;

public class Crimson : Biome
{
	public override int Dirt => 0;

	public override int[] Grass => new int[1] { 199 };

	public override int Stone => 203;

	public override int Ice => 200;

	public override int Clay => 40;

	public override int Sand => 234;

	public override int HardenedSand => 399;

	public override int Sandstone => 401;

	public override int Plants => 201;

	public override int TallPlants => -1;

	public override int Vines => 205;

	public override int Thorn => 352;

	public override ushort DirtWall => 16;

	public override ushort DirtWallUnsafe => 2;

	public override ushort CaveWall => 170;

	public override ushort DirtWallUnsafe1 => 196;

	public override ushort DirtWallUnsafe2 => 197;

	public override ushort DirtWallUnsafe3 => 198;

	public override ushort DirtWallUnsafe4 => 199;

	public override ushort StoneWall => 83;

	public override ushort HardenedSandWall => 218;

	public override ushort SandstoneWall => 221;

	public override ushort GrassWall => 81;

	public override ushort GrassWallUnsafe => 81;

	public override ushort FlowerWall => 81;

	public override ushort FlowerWallUnsafe => 81;

	public override ushort CaveWall1 => 192;

	public override ushort CaveWall2 => 193;

	public override ushort CaveWall3 => 194;

	public override ushort CaveWall4 => 195;
}
