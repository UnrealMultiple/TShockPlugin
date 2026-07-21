namespace WorldEdit.Commands.Biomes;

public class Desert : Biome
{
	public override int Dirt => 53;

	public override int Clay => 53;

	public override int Stone => 396;

	public override int Ice => 396;

	public override int Sand => 53;

	public override int HardenedSand => 397;

	public override int Sandstone => 396;

	public override int[] Grass => new int[1] { 53 };

	public override int Plants => -1;

	public override int TallPlants => -1;

	public override int Vines => -1;

	public override int Thorn => -1;

	public override ushort DirtWall => 216;

	public override ushort DirtWallUnsafe => 216;

	public override ushort CaveWall => 216;

	public override ushort DirtWallUnsafe1 => 216;

	public override ushort DirtWallUnsafe2 => 187;

	public override ushort DirtWallUnsafe3 => 216;

	public override ushort DirtWallUnsafe4 => 187;

	public override ushort StoneWall => 187;

	public override ushort HardenedSandWall => 216;

	public override ushort SandstoneWall => 187;

	public override ushort GrassWall => 216;

	public override ushort GrassWallUnsafe => 216;

	public override ushort FlowerWall => 216;

	public override ushort FlowerWallUnsafe => 216;

	public override ushort CaveWall1 => 0;

	public override ushort CaveWall2 => 0;

	public override ushort CaveWall3 => 0;

	public override ushort CaveWall4 => 0;
}
