using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Actuator : WECommand
{
	private readonly bool remove;

	private readonly Expression expression;

	public Actuator(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, Expression expression, bool remove)
		: base(x, y, x2, y2, magicWand, plr)
	{
		this.remove = remove;
		this.expression = expression ?? new TestExpression((ITile t) => true);
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int num = 0;
		for (int i = x; i <= x2; i++)
		{
			for (int j = y; j <= y2; j++)
			{
				ITile val = Main.tile[i, j];
				if (val.active() && (remove ? val.actuator() : (!val.actuator())) && select(i, j, plr) && expression.Evaluate(val) && magicWand.InSelection(i, j))
				{
					val.actuator(!remove);
					num++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("{0} actuators. ({1})", remove ? "Removed" : "Set", num);
	}
}
