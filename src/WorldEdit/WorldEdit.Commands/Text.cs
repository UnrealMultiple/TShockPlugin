using Terraria;
using TShockAPI;

namespace WorldEdit.Commands;

public class Text : WECommand
{
	private string text;

	public Text(int x, int y, int x2, int y2, TSPlayer plr, string text)
		: base(x, y, x2, y2, plr)
	{
		this.text = text;
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		WEPoint[,] array = Tools.CreateStatueText(text, x2 - x + 1, y2 - y + 1);
		for (int i = 0; i < array.GetLength(0); i++)
		{
			for (int j = 0; j < array.GetLength(1); j++)
			{
				if (Tools.InMapBoundaries(i + x, j + y) && (array[i, j].X != 0 || array[i, j].Y != 0))
				{
					ITile val = Main.tile[i + x, j + y];
					val.active(true);
					val.frameX = array[i, j].X;
					val.frameY = array[i, j].Y;
					val.liquidType(0);
					val.liquid = 0;
					val.type = 337;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Set text.");
	}
}
