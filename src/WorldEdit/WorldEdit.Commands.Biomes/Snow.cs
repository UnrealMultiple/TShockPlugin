namespace WorldEdit.Commands.Biomes;

public class Snow : Biome
{
	public override int Dirt => 147;

	public override int[] Grass => new int[1] { 147 };

	public override int Stone => 161;

	public override int Ice => 161;

	public override int Clay => 147;

	public override int Sand => 147;

	public override int HardenedSand => 147;

	public override int Sandstone => 161;

	public override int Plants => -1;

	public override int TallPlants => -1;

	public override int Vines => -1;

	public override int Thorn => -1;

	public override ushort DirtWall => 40;

	public override ushort DirtWallUnsafe => 40;

	public override ushort CaveWall => 40;

	public override ushort DirtWallUnsafe1 => 40;

	public override ushort DirtWallUnsafe2 => 40;

	public override ushort DirtWallUnsafe3 => 40;

	public override ushort DirtWallUnsafe4 => 40;

	public override ushort StoneWall => 71;

	public override ushort HardenedSandWall => 71;

	public override ushort SandstoneWall => 71;

	public override ushort GrassWall => 71;

	public override ushort GrassWallUnsafe => 71;

	public override ushort FlowerWall => 71;

	public override ushort FlowerWallUnsafe => 71;

	public override ushort CaveWall1 => 0;

	public override ushort CaveWall2 => 0;

	public override ushort CaveWall3 => 0;

	public override ushort CaveWall4 => 0;
}
