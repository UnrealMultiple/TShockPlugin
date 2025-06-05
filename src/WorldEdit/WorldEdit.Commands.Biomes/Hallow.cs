namespace WorldEdit.Commands.Biomes;

public class Hallow : Biome
{
	public override int Dirt => 0;

	public override int[] Grass => new int[2] { 109, 492 };

	public override int Stone => 117;

	public override int Ice => 164;

	public override int Clay => 40;

	public override int Sand => 116;

	public override int HardenedSand => 402;

	public override int Sandstone => 403;

	public override int Plants => 110;

	public override int TallPlants => 113;

	public override int Vines => 115;

	public override int Thorn => -1;

	public override ushort DirtWall => 16;

	public override ushort DirtWallUnsafe => 2;

	public override ushort CaveWall => 170;

	public override ushort DirtWallUnsafe1 => 196;

	public override ushort DirtWallUnsafe2 => 197;

	public override ushort DirtWallUnsafe3 => 198;

	public override ushort DirtWallUnsafe4 => 199;

	public override ushort StoneWall => 28;

	public override ushort HardenedSandWall => 219;

	public override ushort SandstoneWall => 222;

	public override ushort GrassWall => 70;

	public override ushort GrassWallUnsafe => 70;

	public override ushort FlowerWall => 70;

	public override ushort FlowerWallUnsafe => 70;

	public override ushort CaveWall1 => 200;

	public override ushort CaveWall2 => 201;

	public override ushort CaveWall3 => 202;

	public override ushort CaveWall4 => 203;
}
