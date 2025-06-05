namespace WorldEdit.Commands.Biomes;

public class Mushroom : Biome
{
	public override int Dirt => 59;

	public override int[] Grass => new int[1] { 70 };

	public override int Stone => 1;

	public override int Ice => 1;

	public override int Clay => 40;

	public override int Sand => 59;

	public override int HardenedSand => 59;

	public override int Sandstone => 1;

	public override int Plants => 71;

	public override int TallPlants => -1;

	public override int Vines => -1;

	public override int Thorn => -1;

	public override ushort DirtWall => 74;

	public override ushort DirtWallUnsafe => 74;

	public override ushort CaveWall => 74;

	public override ushort DirtWallUnsafe1 => 74;

	public override ushort DirtWallUnsafe2 => 74;

	public override ushort DirtWallUnsafe3 => 74;

	public override ushort DirtWallUnsafe4 => 74;

	public override ushort StoneWall => 1;

	public override ushort HardenedSandWall => 74;

	public override ushort SandstoneWall => 74;

	public override ushort GrassWall => 74;

	public override ushort GrassWallUnsafe => 80;

	public override ushort FlowerWall => 74;

	public override ushort FlowerWallUnsafe => 80;

	public override ushort CaveWall1 => 0;

	public override ushort CaveWall2 => 0;

	public override ushort CaveWall3 => 0;

	public override ushort CaveWall4 => 0;
}
