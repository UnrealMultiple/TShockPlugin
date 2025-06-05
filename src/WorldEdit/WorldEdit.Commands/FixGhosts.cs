using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Tile_Entities;
using TShockAPI;

namespace WorldEdit.Commands;

public class FixGhosts : WECommand
{
	public FixGhosts(int x, int y, int x2, int y2, TSPlayer plr)
		: base(x, y, x2, y2, plr)
	{
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		Sign[] sign = Main.sign;
		foreach (Sign val in sign)
		{
			if (val != null)
			{
				ushort type = Main.tile[val.x, val.y].type;
				if (!Main.tile[val.x, val.y].active() || (type != 55 && type != 85 && type != 425))
				{
					Sign.KillSign(val.x, val.y);
					num++;
				}
			}
		}
		Chest[] chest = Main.chest;
		foreach (Chest val2 in chest)
		{
			if (val2 != null)
			{
				ushort type2 = Main.tile[val2.x, val2.y].type;
				if (!Main.tile[val2.x, val2.y].active() || (type2 != 21 && type2 != 467 && type2 != 88))
				{
					Chest.DestroyChest(val2.x, val2.y);
					num3++;
				}
			}
		}
		for (int k = x; k <= x2; k++)
		{
			for (int l = y; l <= y2; l++)
			{
				if (TEItemFrame.Find(k, l) != -1)
				{
					if (!Main.tile[k, l].active() || Main.tile[k, l].type != 395)
					{
						TEItemFrame.Kill(k, l);
						num2++;
					}
				}
				else if (TELogicSensor.Find(k, l) != -1)
				{
					if (!Main.tile[k, l].active() || Main.tile[k, l].type != 423)
					{
						TELogicSensor.Kill(k, l);
						num4++;
					}
				}
				else if (TETrainingDummy.Find(k, l) != -1)
				{
					if (!Main.tile[k, l].active() || Main.tile[k, l].type != 378)
					{
						TETrainingDummy.Kill(k, l);
						num5++;
					}
				}
				else if (TEWeaponsRack.Find(k, l) != -1)
				{
					if (!Main.tile[k, l].active() || Main.tile[k, l].type != 471)
					{
						TEWeaponsRack.Kill(k, l);
						num6++;
					}
				}
				else if (TETeleportationPylon.Find(k, l) != -1)
				{
					if (!Main.tile[k, l].active() || Main.tile[k, l].type != 597)
					{
						TETeleportationPylon.Kill(k, l);
						num7++;
					}
				}
				else if (TEDisplayDoll.Find(k, l) != -1)
				{
					if (!Main.tile[k, l].active() || Main.tile[k, l].type != 470)
					{
						TEDisplayDoll.Kill(k, l);
						num8++;
					}
				}
				else if (TEHatRack.Find(k, l) != -1)
				{
					if (!Main.tile[k, l].active() || Main.tile[k, l].type != 475)
					{
						TEHatRack.Kill(k, l);
						num9++;
					}
				}
				else if (TEFoodPlatter.Find(k, l) != -1 && (!Main.tile[k, l].active() || Main.tile[k, l].type != 520))
				{
					TEFoodPlatter.Kill(k, l);
					num10++;
				}
			}
		}
		ResetSection();
		List<string> list = new List<string>();
		if (num > 0)
		{
			list.Add($"signs ({num})");
		}
		if (num2 > 0)
		{
			list.Add($"item frames ({num2})");
		}
		if (num3 > 0)
		{
			list.Add($"chests ({num3})");
		}
		if (num4 > 0)
		{
			list.Add($"logic sensors ({num4})");
		}
		if (num5 > 0)
		{
			list.Add($"target dummies ({num5})");
		}
		if (num6 > 0)
		{
			list.Add($"weapon racks ({num6})");
		}
		if (num7 > 0)
		{
			list.Add($"pylons ({num7})");
		}
		if (num8 > 0)
		{
			list.Add($"mannequins ({num8})");
		}
		if (num9 > 0)
		{
			list.Add($"hat racks ({num9})");
		}
		if (num10 > 0)
		{
			list.Add($"food plates ({num10})");
		}
		if (list.Count > 0)
		{
			plr.SendSuccessMessage("Fixed ghost " + string.Join(", ", list) + ".");
		}
		else
		{
			plr.SendSuccessMessage("There are no ghost objects in this area.");
		}
	}
}
