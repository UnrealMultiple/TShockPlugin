using System;
using TShockAPI;

namespace WorldEdit;

public class CanEditEventArgs : EventArgs
{
	public TSPlayer Player { get; }

	public int X { get; }

	public int Y { get; }

	public int X2 { get; }

	public int Y2 { get; }

	public bool? CanEdit { get; set; }

	public CanEditEventArgs(TSPlayer Player, int X, int Y, int X2, int Y2)
	{
		this.Player = Player;
		this.X = X;
		this.Y = Y;
		this.X2 = X2;
		this.Y2 = Y2;
		CanEdit = null;
	}
}
