using Terraria;

namespace WorldEdit.Expressions;

public class OrExpression : Expression
{
	public OrExpression(Expression left, Expression right)
	{
		Left = left;
		Right = right;
	}

	public override bool Evaluate(ITile tile)
	{
		return Left.Evaluate(tile) || Right.Evaluate(tile);
	}
}
