using System.Linq;
using Terraria;
using TShockAPI;
using WorldEdit.Commands.Biomes;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class SetGrass : WECommand
{
	private readonly Expression _expression;

	private readonly string _grass;

	public SetGrass(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, string grass, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_expression = expression ?? new TestExpression(_ => true);
		_grass = grass;
	}

	public override void Execute()
	{
		if (x < 1)
		{
			x = 1;
		}
		else if (x > Main.maxTilesX - 2)
		{
			x = Main.maxTilesX - 2;
		}
		if (y < 1)
		{
			y = 1;
		}
		else if (y > Main.maxTilesY - 2)
		{
			y = Main.maxTilesY - 2;
		}
		if (x2 < 1)
		{
			x2 = 1;
		}
		else if (x2 > Main.maxTilesX - 2)
		{
			x2 = Main.maxTilesX - 2;
		}
		if (y2 < 1)
		{
			y2 = 1;
		}
		else if (y2 > Main.maxTilesY - 2)
		{
			y2 = Main.maxTilesY - 2;
		}
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		global::WorldEdit.Commands.Biomes.Biome biome = WorldEdit.Biomes[_grass];
		ushort dirtTileType = (ushort)biome.Dirt;
		ushort grassTileType = (ushort)biome.Grass.First();
		int changedGrassCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				bool hasExposedEdge = !Main.tile[tileX - 1, tileY - 1].active() || !Main.tile[tileX - 1, tileY + 1].active() || !Main.tile[tileX + 1, tileY - 1].active() || !Main.tile[tileX + 1, tileY + 1].active() || !Main.tile[tileX - 1, tileY].active() || !Main.tile[tileX + 1, tileY].active() || !Main.tile[tileX, tileY - 1].active() || !Main.tile[tileX, tileY + 1].active();
				if (Main.tile[tileX, tileY].active() && hasExposedEdge && _expression.Evaluate(Main.tile[tileX, tileY]) && Main.tile[tileX, tileY].type == dirtTileType && magicWand.InSelection(tileX, tileY))
				{
					Main.tile[tileX, tileY].type = grassTileType;
					changedGrassCount++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Set {1} grass. ({0})", changedGrassCount, _grass);
	}
}
