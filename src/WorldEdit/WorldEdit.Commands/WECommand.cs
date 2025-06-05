using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using TShockAPI;
using TShockAPI.DB;
using WorldEdit.Extensions;

namespace WorldEdit.Commands;

public abstract class WECommand
{
	public TSPlayer plr;

	public Selection select;

	public int x;

	public int x2;

	public int y;

	public int y2;

	public MagicWand magicWand;

	public bool minMaxPoints;

	protected WECommand(int x, int y, int x2, int y2, TSPlayer plr)
		: this(x, y, x2, y2, null, plr)
	{
	}

	protected WECommand(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, bool minMaxPoints = true)
	{
		this.plr = plr;
		select = plr.GetPlayerInfo().Select ?? WorldEdit.Selections["normal"];
		this.x = x;
		this.x2 = x2;
		this.y = y;
		this.y2 = y2;
		this.magicWand = magicWand ?? new MagicWand();
		this.minMaxPoints = minMaxPoints;
	}

	public abstract void Execute();

	public void Position(bool force = false)
	{
		if (force || minMaxPoints)
		{
			x = Math.Max(x, 0);
			y = Math.Max(y, 0);
			x2 = Math.Min(x2, Main.maxTilesX - 1);
			y2 = Math.Min(y2, Main.maxTilesY - 1);
			if (x > x2)
			{
				int num = x2;
				x2 = x;
				x = num;
			}
			if (y > y2)
			{
				int num = y2;
				y2 = y;
				y = num;
			}
		}
	}

	public void ResetSection()
	{
		int num = Math.Min(x, x2);
		int num2 = Math.Max(x, x2);
		int num3 = Math.Min(y, y2);
		int num4 = Math.Max(y, y2);
		int sectionX = Netplay.GetSectionX(num);
		int sectionX2 = Netplay.GetSectionX(num2);
		int sectionY = Netplay.GetSectionY(num3);
		int sectionY2 = Netplay.GetSectionY(num4);
		int num5 = num2 - num + 1;
		int num6 = num4 - num3 + 1;
		if (num5 > 200 || num6 > 150)
		{
			foreach (RemoteClient item in Netplay.Clients.Where((RemoteClient s) => s.IsActive))
			{
				for (int i = sectionX; i <= sectionX2; i++)
				{
					for (int j = sectionY; j <= sectionY2; j++)
					{
						item.TileSections[i, j] = false;
					}
				}
			}
			return;
		}
		NetMessage.SendData(10, -1, -1, (NetworkText)null, num, (float)num3, (float)num5, (float)num6, 0, 0, 0);
		NetMessage.SendData(11, -1, -1, (NetworkText)null, sectionX, (float)sectionY, (float)sectionX2, (float)sectionY2, 0, 0, 0);
	}

	public void SetTile(int i, int j, int tileType)
	{
		ITile val = Main.tile[i, j];
		switch (tileType)
		{
		case -1:
			val.active(false);
			val.frameX = -1;
			val.frameY = -1;
			val.liquidType(0);
			val.liquid = 0;
			val.type = 0;
			return;
		case -2:
			val.active(false);
			val.liquidType(1);
			val.liquid = byte.MaxValue;
			val.type = 0;
			return;
		case -3:
			val.active(false);
			val.liquidType(2);
			val.liquid = byte.MaxValue;
			val.type = 0;
			return;
		case -4:
			val.active(false);
			val.liquidType(0);
			val.liquid = byte.MaxValue;
			val.type = 0;
			return;
		}
		if (Main.tileFrameImportant[tileType])
		{
			WorldGen.PlaceTile(i, j, tileType, false, false, -1, 0);
			return;
		}
		val.active(true);
		val.frameX = -1;
		val.frameY = -1;
		val.liquidType(0);
		val.liquid = 0;
		val.type = (ushort)tileType;
	}

	public bool TileSolid(int x, int y)
	{
		return x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY || (Main.tile[x, y].active() && Main.tileSolid[Main.tile[x, y].type]);
	}

	public bool CanUseCommand()
	{
		return CanUseCommand(x, y, x2, y2);
	}

	public bool CanUseCommand(int x, int y, int x2, int y2)
	{
						CanEditEventArgs canEditEventArgs = new CanEditEventArgs(plr, x, y, x2, y2);
		WorldEdit.CanEdit.Invoke(canEditEventArgs);
		if (canEditEventArgs.CanEdit.HasValue)
		{
			if (!canEditEventArgs.CanEdit.Value)
			{
				plr.SendErrorMessage("You do not have permission to use this command in this area.");
			}
			return canEditEventArgs.CanEdit.Value;
		}
		if (plr.HasPermission("worldedit.usage.everywhere"))
		{
			return true;
		}
		bool flag = plr.HasPermission("worldedit.usage.noregion");
		if (!flag && !plr.IsLoggedIn)
		{
			plr.SendErrorMessage("You have to be logged in to use this command.");
			return false;
		}
        Rectangle area = new Rectangle(x, y, x2 - x, y2 - y);
        if ((!flag && !TShock.Regions.Regions.Any(r => r.Area.Contains(area))) ||
            !TShock.Regions.CanBuild(x, y, plr) ||
            !TShock.Regions.CanBuild(x2, y, plr) ||
            !TShock.Regions.CanBuild(x2, y2, plr) ||
            !TShock.Regions.CanBuild(x, y2, plr))
        {
            plr.SendErrorMessage("You do not have permission to use this command outside of your regions.");
            return false;
        }
        return true;
	}
}
