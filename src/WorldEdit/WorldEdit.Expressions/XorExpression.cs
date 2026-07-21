using Terraria;

namespace WorldEdit.Expressions;

public class XorExpression : Expression
{
	public XorExpression(Expression left, Expression right)
	{
		Left = left;
		Right = right;
	}

	public override bool Evaluate(ITile tile)
	{
		return Left.Evaluate(tile) ^ Right.Evaluate(tile);
	}
}
