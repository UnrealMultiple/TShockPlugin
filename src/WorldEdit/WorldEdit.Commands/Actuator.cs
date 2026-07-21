using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Actuator : WECommand
{
	private readonly bool _remove;

	private readonly Expression _expression;

	public Actuator(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, Expression expression, bool remove)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_remove = remove;
		_expression = expression ?? new TestExpression(_ => true);
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int changedActuatorCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				ITile tile = Main.tile[tileX, tileY];
				if (tile.active() && (_remove ? tile.actuator() : !tile.actuator()) && select(tileX, tileY, plr) && _expression.Evaluate(tile) && magicWand.InSelection(tileX, tileY))
				{
					tile.actuator(!_remove);
					changedActuatorCount++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("{0} actuators. ({1})", _remove ? "Removed" : "Set", changedActuatorCount);
	}
}
