using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace WorldEdit.Expressions;

public static class Parser
{
	public static Expression ParseExpression(IEnumerable<Token> postfix)
	{
		Stack<Expression> stack = new Stack<Expression>();
		foreach (Token item in postfix)
		{
			switch (item.Type)
			{
			case Token.TokenType.BinaryOperator:
				switch ((Token.OperatorType)item.Value)
				{
				case Token.OperatorType.And:
					stack.Push(new AndExpression(stack.Pop(), stack.Pop()));
					break;
				case Token.OperatorType.Or:
					stack.Push(new OrExpression(stack.Pop(), stack.Pop()));
					break;
				case Token.OperatorType.Xor:
					stack.Push(new XorExpression(stack.Pop(), stack.Pop()));
					break;
				default:
					return null;
				}
				break;
			case Token.TokenType.Test:
				stack.Push(new TestExpression((Test)item.Value));
				break;
			case Token.TokenType.UnaryOperator:
			{
				Token.OperatorType operatorType = (Token.OperatorType)item.Value;
				Token.OperatorType operatorType2 = operatorType;
				if (operatorType2 == Token.OperatorType.Not)
				{
					stack.Push(new NotExpression(stack.Pop()));
					break;
				}
				return null;
			}
			default:
				return null;
			}
		}
		return stack.Pop();
	}

	public static List<Token> ParseInfix(string str)
	{
		str = str.Replace(" ", "").ToLowerInvariant();
		List<Token> list = new List<Token>();
		for (int i = 0; i < str.Length; i++)
		{
			switch (str[i])
			{
			case '&':
				if (str[i + 1] == '&')
				{
					i++;
				}
				list.Add(new Token
				{
					Type = Token.TokenType.BinaryOperator,
					Value = Token.OperatorType.And
				});
				continue;
			case '!':
				list.Add(new Token
				{
					Type = Token.TokenType.UnaryOperator,
					Value = Token.OperatorType.Not
				});
				continue;
			case '|':
				if (str[i + 1] == '|')
				{
					i++;
				}
				list.Add(new Token
				{
					Type = Token.TokenType.BinaryOperator,
					Value = Token.OperatorType.Or
				});
				continue;
			case '^':
				list.Add(new Token
				{
					Type = Token.TokenType.BinaryOperator,
					Value = Token.OperatorType.Xor
				});
				continue;
			case '(':
				list.Add(new Token
				{
					Type = Token.TokenType.OpenParentheses
				});
				continue;
			case ')':
				list.Add(new Token
				{
					Type = Token.TokenType.CloseParentheses
				});
				continue;
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (i < str.Length && (char.IsLetterOrDigit(str[i]) || str[i] == '!' || str[i] == '='))
			{
				stringBuilder.Append(str[i++]);
			}
			i--;
			string[] array = stringBuilder.ToString().Split('=');
			string text = array[0];
			string rhs = "";
			bool negated = false;
			if (array.Length > 1)
			{
				if (text[text.Length - 1] == '!')
				{
					text = text.Substring(0, text.Length - 1);
					negated = true;
				}
				rhs = array[1];
			}
			list.Add(new Token
			{
				Type = Token.TokenType.Test,
				Value = ParseTest(text, rhs, negated)
			});
		}
		return list;
	}

	public static List<Token> ParsePostfix(IEnumerable<Token> infix)
	{
		List<Token> list = new List<Token>();
		Stack<Token> stack = new Stack<Token>();
		foreach (Token item in infix)
		{
			switch (item.Type)
			{
			case Token.TokenType.BinaryOperator:
			case Token.TokenType.OpenParentheses:
			case Token.TokenType.UnaryOperator:
				stack.Push(item);
				break;
			case Token.TokenType.CloseParentheses:
				while (stack.Peek().Type != Token.TokenType.OpenParentheses)
				{
					list.Add(stack.Pop());
				}
				stack.Pop();
				if (stack.Count > 0 && stack.Peek().Type == Token.TokenType.UnaryOperator)
				{
					list.Add(stack.Pop());
				}
				break;
			case Token.TokenType.Test:
				list.Add(item);
				break;
			}
		}
		while (stack.Count > 0)
		{
			list.Add(stack.Pop());
		}
		return list;
	}

	public static Test ParseTest(string lhs, string rhs, bool negated)
	{
		switch (lhs)
		{
		case "lh":
		case "honey":
		{
			Test test;
			return test = (ITile t) => t.liquid > 0 && t.liquidType() == 2;
		}
		case "nlh":
		case "nhoney":
		{
			Test test;
			return test = (ITile t) => t.liquidType() != 2;
		}
		case "ll":
		case "lava":
		{
			Test test;
			return test = (ITile t) => t.liquid > 0 && t.liquidType() == 1;
		}
		case "nll":
		case "nlava":
		{
			Test test;
			return test = (ITile t) => t.liquidType() != 1;
		}
		case "li":
		case "liquid":
		{
			Test test;
			return test = (ITile t) => t.liquid > 0;
		}
		case "nli":
		case "nliquid":
		{
			Test test;
			return test = (ITile t) => t.liquid == 0;
		}
		case "t":
		case "tile":
		{
			Test test;
			if (string.IsNullOrEmpty(rhs))
			{
				return test = (ITile t) => t.active();
			}
			List<int> tiles = Tools.GetTileID(rhs);
			if (tiles.Count == 0 || tiles.Count > 1)
			{
				throw new ArgumentException();
			}
			return test = (ITile t) => (t.active() && t.type == tiles[0]) != negated;
		}
		case "nt":
		case "ntile":
		{
			Test test;
			return test = (ITile t) => !t.active();
		}
		case "tp":
		case "tilepaint":
		{
			Test test;
			if (string.IsNullOrEmpty(rhs))
			{
				return test = (ITile t) => t.active() && t.color() != 0;
			}
			List<int> colors2 = Tools.GetColorID(rhs);
			if (colors2.Count == 0 || colors2.Count > 1)
			{
				throw new ArgumentException();
			}
			return test = (ITile t) => (t.active() && t.color() == colors2[0]) != negated;
		}
		case "ntp":
		case "ntilepaint":
		{
			Test test;
			return test = (ITile t) => t.color() == 0;
		}
		case "w":
		case "wall":
		{
			Test test;
			if (string.IsNullOrEmpty(rhs))
			{
				return test = (ITile t) => t.wall != 0;
			}
			List<int> walls = Tools.GetTileID(rhs);
			if (walls.Count == 0 || walls.Count > 1)
			{
				throw new ArgumentException();
			}
			return test = (ITile t) => t.wall == walls[0] != negated;
		}
		case "nw":
		case "nwall":
		{
			Test test;
			return test = (ITile t) => t.wall == 0;
		}
		case "wp":
		case "wallpaint":
		{
			Test test;
			if (string.IsNullOrEmpty(rhs))
			{
				return test = (ITile t) => t.wall > 0 && t.wallColor() != 0;
			}
			List<int> colors = Tools.GetColorID(rhs);
			if (colors.Count == 0 || colors.Count > 1)
			{
				throw new ArgumentException();
			}
			return test = (ITile t) => (t.wall > 0 && t.wallColor() == colors[0]) != negated;
		}
		case "nwp":
		case "nwallpaint":
		{
			Test test;
			return test = (ITile t) => t.wallColor() == 0;
		}
		case "lw":
		case "water":
		{
			Test test;
			return test = (ITile t) => t.liquid > 0 && t.liquidType() == 0;
		}
		case "nlw":
		case "nwater":
		{
			Test test;
			return test = (ITile t) => t.liquidType() != 0;
		}
		case "wire":
		case "wire1":
		case "wirered":
		case "redwire":
		{
			Test test;
			return test = (ITile t) => t.wire();
		}
		case "nwire":
		case "nwire1":
		case "nwirered":
		case "nredwire":
		{
			Test test;
			return test = (ITile t) => !t.wire();
		}
		case "wire2":
		case "wireblue":
		case "bluewire":
		{
			Test test;
			return test = (ITile t) => t.wire2();
		}
		case "nwire2":
		case "nwireblue":
		case "nbluewire":
		{
			Test test;
			return test = (ITile t) => !t.wire2();
		}
		case "wire3":
		case "wiregreen":
		case "greenwire":
		{
			Test test;
			return test = (ITile t) => t.wire3();
		}
		case "nwire3":
		case "nwiregreen":
		case "ngreenwire":
		{
			Test test;
			return test = (ITile t) => !t.wire3();
		}
		case "wire4":
		case "wireyellow":
		case "yellowwire":
		{
			Test test;
			return test = (ITile t) => t.wire4();
		}
		case "nwire4":
		case "nwireyellow":
		case "nyellowwire":
		{
			Test test;
			return test = (ITile t) => !t.wire4();
		}
		case "a":
		case "active":
		{
			Test test;
			return test = (ITile t) => t.active() && !t.inActive();
		}
		case "na":
		case "nactive":
		{
			Test test;
			return test = (ITile t) => t.inActive();
		}
		case "s":
		case "slope":
		{
			Test test;
			if (string.IsNullOrEmpty(rhs))
			{
				return test = (ITile t) => t.slope() != 0 || t.halfBrick();
			}
			int slope = Tools.GetSlopeID(rhs);
			if (slope == -1)
			{
				throw new ArgumentException();
			}
			return test = (ITile t) => (t.active() && ((slope == 1) ? t.halfBrick() : (t.slope() == (byte)slope))) != negated;
		}
		case "ns":
		case "nslope":
		{
			Test test;
			return test = (ITile t) => t.slope() == 0 && !t.halfBrick();
		}
		case "ac":
		case "actuator":
		{
			Test test;
			return test = (ITile t) => t.actuator();
		}
		case "nac":
		case "nactuator":
		{
			Test test;
			return test = (ITile t) => !t.actuator();
		}
		default:
			throw new ArgumentException("Invalid test.");
		}
	}

	public static bool TryParseTree(IEnumerable<string> parameters, out Expression expression)
	{
		expression = null;
		if (parameters.FirstOrDefault() != "=>")
		{
			return false;
		}
		try
		{
			expression = ParseExpression(ParsePostfix(ParseInfix(string.Join(" ", parameters.Skip(1)))));
			return true;
		}
		catch
		{
			return false;
		}
	}
}
