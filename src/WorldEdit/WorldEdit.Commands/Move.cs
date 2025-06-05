using System;
using Terraria;
using TShockAPI;
using WorldEdit.Expressions;
using WorldEdit.Extensions;

namespace WorldEdit.Commands;

public class Move : WECommand
{
	private int down;

	private int right;

	private Expression expression;

	public Move(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int down, int right, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		this.down = down;
		this.right = right;
		this.expression = expression ?? new TestExpression((ITile t) => true);
	}

	public override void Execute()
	{
						int num = x + right;
		int num2 = y + down;
		if (num < 0)
		{
			num = 0;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num >= Main.maxTilesX - Math.Abs(x - x2))
		{
			num = Main.maxTilesX - Math.Abs(x - x2) - 1;
		}
		if (num2 >= Main.maxTilesY - Math.Abs(y - y2))
		{
			num2 = Main.maxTilesY - Math.Abs(y - y2) - 1;
		}
		int num3 = num + Math.Abs(x - x2);
		int num4 = num2 + Math.Abs(y - y2);
		int num5 = Math.Min(x, Math.Min(num, num3));
		int num6 = Math.Min(y, Math.Min(num2, num4));
		int num7 = Math.Max(x2, Math.Max(num, num3));
		int num8 = Math.Max(y2, Math.Max(num2, num4));
		if (!CanUseCommand(num5, num6, num7, num8))
		{
			return;
		}
		Tools.PrepareUndo(num5, num6, num7, num8, plr);
		WorldSectionData worldSectionData = Tools.SaveWorldSection(x, y, x2, y2);
		int num9 = 0;
		for (int i = x; i <= x2; i++)
		{
			for (int j = y; j <= y2; j++)
			{
				if (magicWand.InSelection(i, j) && expression.Evaluate(Main.tile[i, j]))
				{
					Main.tile[i, j] = (ITile)new Tile();
					num9++;
				}
			}
		}
		for (int k = num; k <= num3; k++)
		{
			for (int l = num2; l <= num4; l++)
			{
				int num10 = k - num;
				int num11 = l - num2;
				if (k >= 0 && l >= 0 && k < Main.maxTilesX && l < Main.maxTilesY && magicWand.InSelection(k - right, l - down) && expression.Evaluate(worldSectionData.Tiles[num10, num11]))
				{
					Main.tile[k, l] = worldSectionData.Tiles[num10, num11];
				}
			}
		}
		Tools.LoadWorldSection(worldSectionData, num, num2, Tiles: false);
		ResetSection();
		PlayerInfo playerInfo = plr.GetPlayerInfo();
		playerInfo.X = num;
		playerInfo.Y = num2;
		playerInfo.X2 = num3;
		playerInfo.Y2 = num4;
		plr.SendInfoMessage("Moved tiles ({0}).", num9);
	}
}
