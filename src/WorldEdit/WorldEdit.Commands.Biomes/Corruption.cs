namespace WorldEdit.Commands.Biomes;

public class Corruption : Biome
{
	public override int Dirt => 0;

	public override int[] Grass => new int[1] { 23 };

	public override int Stone => 25;

	public override int Ice => 163;

	public override int Clay => 40;

	public override int Sand => 112;

	public override int HardenedSand => 398;

	public override int Sandstone => 400;

	public override int Plants => 24;

	public override int TallPlants => -1;

	public override int Vines => -1;

	public override int Thorn => 32;

	public override ushort DirtWall => 16;

	public override ushort DirtWallUnsafe => 2;

	public override ushort CaveWall => 170;

	public override ushort DirtWallUnsafe1 => 196;

	public override ushort DirtWallUnsafe2 => 197;

	public override ushort DirtWallUnsafe3 => 198;

	public override ushort DirtWallUnsafe4 => 199;

	public override ushort StoneWall => 3;

	public override ushort HardenedSandWall => 217;

	public override ushort SandstoneWall => 220;

	public override ushort GrassWall => 69;

	public override ushort GrassWallUnsafe => 69;

	public override ushort FlowerWall => 69;

	public override ushort FlowerWallUnsafe => 69;

	public override ushort CaveWall1 => 188;

	public override ushort CaveWall2 => 189;

	public override ushort CaveWall3 => 190;

	public override ushort CaveWall4 => 191;
}
