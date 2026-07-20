namespace WorldEdit.Commands.Biomes;

public class Forest : Biome
{
	public override int Dirt => 0;

	public override int[] Grass => new int[2] { 2, 477 };

	public override int Stone => 1;

	public override int Ice => 161;

	public override int Clay => 40;

	public override int Sand => 53;

	public override int HardenedSand => 397;

	public override int Sandstone => 396;

	public override int Plants => 3;

	public override int TallPlants => 73;

	public override int Vines => 52;

	public override int Thorn => -1;

	public override ushort DirtWall => 16;

	public override ushort DirtWallUnsafe => 2;

	public override ushort CaveWall => 170;

	public override ushort DirtWallUnsafe1 => 196;

	public override ushort DirtWallUnsafe2 => 197;

	public override ushort DirtWallUnsafe3 => 198;

	public override ushort DirtWallUnsafe4 => 199;

	public override ushort StoneWall => 1;

	public override ushort HardenedSandWall => 216;

	public override ushort SandstoneWall => 187;

	public override ushort GrassWall => 66;

	public override ushort GrassWallUnsafe => 63;

	public override ushort FlowerWall => 68;

	public override ushort FlowerWallUnsafe => 65;

	public override ushort CaveWall1 => 0;

	public override ushort CaveWall2 => 0;

	public override ushort CaveWall3 => 0;

	public override ushort CaveWall4 => 0;
}
