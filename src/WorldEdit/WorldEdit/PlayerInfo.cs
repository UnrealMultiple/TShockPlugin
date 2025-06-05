using System;
using System.Linq;
using Terraria;
using WorldEdit.Expressions;

namespace WorldEdit;

public class PlayerInfo
{
	private int _x = -1;

	private int _x2 = -1;

	private int _y = -1;

	private int _y2 = -1;

	public const string Key = "WorldEdit_Data";

	public int Point = 0;

	public Selection Select = null;

	private MagicWand _magicWand = null;

	public Expression SavedExpression = null;

	public int X
	{
		get
		{
			return _x;
		}
		set
		{
			_x = Math.Max(0, value);
			_magicWand = null;
		}
	}

	public int X2
	{
		get
		{
			return _x2;
		}
		set
		{
			_x2 = Math.Min(value, Main.maxTilesX - 1);
			_magicWand = null;
		}
	}

	public int Y
	{
		get
		{
			return _y;
		}
		set
		{
			_y = Math.Max(0, value);
			_magicWand = null;
		}
	}

	public int Y2
	{
		get
		{
			return _y2;
		}
		set
		{
			_y2 = Math.Min(value, Main.maxTilesY - 1);
			_magicWand = null;
		}
	}

	public MagicWand MagicWand
	{
		get
		{
			return _magicWand;
		}
		set
		{
			_magicWand = value ?? new MagicWand();
			if (value == null)
			{
				_x = (_x2 = (_y = (_y2 = -1)));
				return;
			}
			IOrderedEnumerable<WEPoint> source = _magicWand.Points.OrderBy((WEPoint p) => p.X);
			IOrderedEnumerable<WEPoint> source2 = _magicWand.Points.OrderBy((WEPoint p) => p.Y);
			_x = source.First().X;
			_x2 = source.Last().X;
			_y = source2.First().Y;
			_y2 = source2.Last().Y;
		}
	}
}
