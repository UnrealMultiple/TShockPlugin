using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Paste : WECommand
{
	private readonly int alignment;

	private readonly Expression expression;

	private readonly bool mode_MainBlocks;

	private readonly string path;

	private readonly bool prepareUndo;

	public Paste(int x, int y, TSPlayer plr, string path, int alignment, Expression expression, bool mode_MainBlocks, bool prepareUndo)
		: base(x, y, int.MaxValue, int.MaxValue, plr)
	{
		this.alignment = alignment;
		this.expression = expression;
		this.mode_MainBlocks = mode_MainBlocks;
		this.path = path;
		this.prepareUndo = prepareUndo;
	}

	public override void Execute()
	{
		WorldSectionData worldSectionData = Tools.LoadWorldData(path);
		int num = worldSectionData.Width - 1;
		int num2 = worldSectionData.Height - 1;
		if ((alignment & 1) == 0)
		{
			x2 = x + num;
		}
		else
		{
			x2 = x;
			x -= num;
		}
		if ((alignment & 2) == 0)
		{
			y2 = y + num2;
		}
		else
		{
			y2 = y;
			y -= num2;
		}
		if (x < 0)
		{
			x = 0;
		}
		if (x2 < 0)
		{
			x2 = 0;
		}
		if (y < 0)
		{
			y = 0;
		}
		if (y2 < 0)
		{
			y2 = 0;
		}
		if (x >= Main.maxTilesX)
		{
			x = Main.maxTilesX - 1;
		}
		if (x2 >= Main.maxTilesX)
		{
			x2 = Main.maxTilesX - 1;
		}
		if (y >= Main.maxTilesY)
		{
			y = Main.maxTilesY - 1;
		}
		if (y2 >= Main.maxTilesY)
		{
			y2 = Main.maxTilesY - 1;
		}
		if (!CanUseCommand())
		{
			return;
		}
		if (prepareUndo)
		{
			Tools.PrepareUndo(x, y, x2, y2, plr);
		}
		for (int i = x; i <= x2; i++)
		{
			for (int j = y; j <= y2; j++)
			{
				int num3 = i - x;
				int num4 = j - y;
				if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY && (expression == null || expression.Evaluate(mode_MainBlocks ? Main.tile[i, j] : worldSectionData.Tiles[num3, num4])))
				{
					Main.tile[i, j] = worldSectionData.Tiles[num3, num4];
				}
			}
		}
		Tools.LoadWorldSection(worldSectionData, x, y, Tiles: false);
		ResetSection();
		plr.SendSuccessMessage("Pasted clipboard to selection.");
	}
}
