namespace WorldEdit.Commands.Biomes;

public class Hell : Biome
{
	public override int Dirt => 57;

	public override int[] Grass => new int[1] { 57 };

	public override int Stone => 58;

	public override int Ice => 58;

	public override int Clay => 57;

	public override int Sand => 123;

	public override int HardenedSand => 57;

	public override int Sandstone => 58;

	public override int Plants => -1;

	public override int TallPlants => -1;

	public override int Vines => -1;

	public override int Thorn => -1;

	public override ushort DirtWall => 0;

	public override ushort DirtWallUnsafe => 0;

	public override ushort CaveWall => 0;

	public override ushort DirtWallUnsafe1 => 0;

	public override ushort DirtWallUnsafe2 => 0;

	public override ushort DirtWallUnsafe3 => 0;

	public override ushort DirtWallUnsafe4 => 0;

	public override ushort StoneWall => 0;

	public override ushort HardenedSandWall => 0;

	public override ushort SandstoneWall => 0;

	public override ushort GrassWall => 0;

	public override ushort GrassWallUnsafe => 0;

	public override ushort FlowerWall => 0;

	public override ushort FlowerWallUnsafe => 0;

	public override ushort CaveWall1 => 0;

	public override ushort CaveWall2 => 0;

	public override ushort CaveWall3 => 0;

	public override ushort CaveWall4 => 0;
}
