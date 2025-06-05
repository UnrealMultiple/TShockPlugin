namespace WorldEdit.Expressions;

public class Token
{
	public enum OperatorType
	{
		And,
		Not,
		Or,
		Xor
	}

	public enum TokenType
	{
		BinaryOperator,
		CloseParentheses,
		OpenParentheses,
		Test,
		UnaryOperator
	}

	public TokenType Type;

	public object Value;
}
