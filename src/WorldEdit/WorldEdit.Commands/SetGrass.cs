using System.Linq;
using Terraria;
using TShockAPI;
using WorldEdit.Commands.Biomes;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class SetGrass : WECommand
{
	private Expression expression;

	private string grass;

	public SetGrass(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, string grass, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		this.expression = expression ?? new TestExpression((ITile t) => true);
		this.grass = grass;
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
		global::WorldEdit.Commands.Biomes.Biome biome = WorldEdit.Biomes[grass];
		ushort num = (ushort)biome.Dirt;
		ushort type = (ushort)biome.Grass.First();
		int num2 = 0;
		for (int i = x; i <= x2; i++)
		{
			for (int j = y; j <= y2; j++)
			{
				bool flag = Main.tile[i, j].active();
				bool flag2 = Main.tile[i - 1, j - 1].active();
				bool flag3 = Main.tile[i - 1, j + 1].active();
				bool flag4 = Main.tile[i + 1, j - 1].active();
				bool flag5 = Main.tile[i + 1, j + 1].active();
				bool flag6 = Main.tile[i - 1, j].active();
				bool flag7 = Main.tile[i + 1, j].active();
				bool flag8 = Main.tile[i, j - 1].active();
				bool flag9 = Main.tile[i, j + 1].active();
				if (flag && !(flag2 && flag3 && flag4 && flag5 && flag6 && flag7 && flag8 && flag9) && expression.Evaluate(Main.tile[i, j]) && Main.tile[i, j].type == num && magicWand.InSelection(i, j))
				{
					Main.tile[i, j].type = type;
					num2++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Set {1} grass. ({0})", num2, grass);
	}
}
