using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using TShockAPI;

namespace WorldEdit.Commands;

public class Activate : WECommand
{
	private readonly int _action;

	public Activate(int x, int y, int x2, int y2, TSPlayer plr, byte action)
		: base(x, y, x2, y2, null, plr)
	{
		_action = action;
	}

	public override void Execute()
	{
						if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int num = 0;
		if (_action == 255 || _action == 0)
		{
			int num2 = 0;
			int num3 = 0;
			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					if ((Main.tile[i, j].type == 55 || Main.tile[i, j].type == 85 || Main.tile[i, j].type == 425) && Main.tile[i, j].frameX % 36 == 0 && Main.tile[i, j].frameY == 0 && Sign.ReadSign(i, j, false) == -1)
					{
						int num4 = Sign.ReadSign(i, j, true);
						if (num4 == -1)
						{
							num3++;
						}
						else
						{
							num2++;
						}
					}
				}
			}
			if (num2 > 0 || num3 > 0)
			{
				plr.SendSuccessMessage("Activated signs. ({0}){1}", num2, (num3 > 0) ? (" Failed to activate signs. (" + num3 + ")") : "");
			}
			else
			{
				num++;
			}
		}
		if (_action == 255 || _action == 1)
		{
			int num5 = 0;
			int num6 = 0;
			for (int k = x; k <= x2; k++)
			{
				for (int l = y; l <= y2; l++)
				{
					if ((Main.tile[k, l].type == 21 || Main.tile[k, l].type == 467 || Main.tile[k, l].type == 88) && Main.tile[k, l].frameX % 36 == 0 && Main.tile[k, l].frameY == 0 && Chest.FindChest(k, l) == -1)
					{
						int num7 = Chest.CreateChest(k, l, -1);
						if (num7 == -1)
						{
							num6++;
						}
						else
						{
							num5++;
						}
					}
				}
			}
			if (num5 > 0 || num6 > 0)
			{
				plr.SendSuccessMessage("Activated chests. ({0}){1}", num5, (num6 > 0) ? (" Failed to activate chests. (" + num6 + ")") : "");
			}
			else
			{
				num++;
			}
		}
		if (_action == 255 || _action == 2)
		{
			int num8 = 0;
			int num9 = 0;
			for (int m = x; m <= x2; m++)
			{
				for (int n = y; n <= y2; n++)
				{
					if (Main.tile[m, n].type == 395 && Main.tile[m, n].frameX % 36 == 0 && Main.tile[m, n].frameY == 0 && TEItemFrame.Find(m, n) == -1)
					{
						int num10 = TEItemFrame.Place(m, n);
						if (num10 == -1)
						{
							num9++;
						}
						else
						{
							num8++;
						}
					}
				}
			}
			if (num8 > 0 || num9 > 0)
			{
				plr.SendSuccessMessage("Activated item frames. ({0}){1}", num8, (num9 > 0) ? (" Failed to activate item frames. (" + num9 + ")") : "");
			}
			else
			{
				num++;
			}
		}
		if (_action == 255 || _action == 3)
		{
			int num11 = 0;
			int num12 = 0;
			for (int num13 = x; num13 <= x2; num13++)
			{
				for (int num14 = y; num14 <= y2; num14++)
				{
					if (Main.tile[num13, num14].type == 423 && TELogicSensor.Find(num13, num14) == -1)
					{
						int num15 = TELogicSensor.Place(num13, num14);
						if (num15 == -1)
						{
							num12++;
							continue;
						}
						((TELogicSensor)TileEntity.ByID[num15]).logicCheck = (TELogicSensor.LogicCheckType)(Main.tile[num13, num14].frameY / 18 + 1);
						num11++;
					}
				}
			}
			if (num11 > 0 || num12 > 0)
			{
				plr.SendSuccessMessage("Activated logic sensors. ({0}){1}", num11, (num12 > 0) ? (" Failed to activate logic sensors. (" + num12 + ")") : "");
			}
			else
			{
				num++;
			}
		}
		if (_action == 255 || _action == 4)
		{
			int num16 = 0;
			int num17 = 0;
			for (int num18 = x; num18 <= x2; num18++)
			{
				for (int num19 = y; num19 <= y2; num19++)
				{
					if (Main.tile[num18, num19].type == 378 && Main.tile[num18, num19].frameX % 36 == 0 && Main.tile[num18, num19].frameY == 0 && TETrainingDummy.Find(num18, num19) == -1)
					{
						int num20 = TETrainingDummy.Place(num18, num19);
						if (num20 == -1)
						{
							num17++;
						}
						else
						{
							num16++;
						}
					}
				}
			}
			if (num16 > 0 || num17 > 0)
			{
				plr.SendSuccessMessage("Activated target dummies. ({0}){1}", num16, (num17 > 0) ? (" Failed to activate target dummies. (" + num17 + ")") : "");
			}
			else
			{
				num++;
			}
		}
		if (_action == 255 || _action == 5)
		{
			int num21 = 0;
			int num22 = 0;
			for (int num23 = x; num23 <= x2; num23++)
			{
				for (int num24 = y; num24 <= y2; num24++)
				{
					if (Main.tile[num23, num24].type == 471 && Main.tile[num23, num24].frameX % 54 == 0 && Main.tile[num23, num24].frameY == 0 && TEWeaponsRack.Find(num23, num24) == -1)
					{
						if (TEWeaponsRack.Place(num23, num24) == -1)
						{
							num22++;
						}
						else
						{
							num21++;
						}
					}
				}
			}
			if (num21 > 0 || num22 > 0)
			{
				plr.SendSuccessMessage("Activated weapon racks. ({0}){1}", num21, (num22 > 0) ? (" Failed to activate weapon racks. (" + num22 + ")") : "");
			}
			else
			{
				num++;
			}
		}
		if (_action == 255 || _action == 6)
		{
			int num25 = 0;
			int num26 = 0;
			for (int num27 = x; num27 <= x2; num27++)
			{
				for (int num28 = y; num28 <= y2; num28++)
				{
					if (Main.tile[num27, num28].type == 597 && Main.tile[num27, num28].frameX % 54 == 0 && Main.tile[num27, num28].frameY == 0 && TETeleportationPylon.Find(num27, num28) == -1)
					{
						if (TETeleportationPylon.Place(num27, num28) == -1)
						{
							num26++;
						}
						else
						{
							num25++;
						}
					}
				}
			}
			if (num25 > 0 || num26 > 0)
			{
				plr.SendSuccessMessage("Activated pylons. ({0}){1}", num25, (num26 > 0) ? (" Failed to activate pylons. (" + num26 + ")") : "");
			}
			else
			{
				num++;
			}
		}
		if (_action == 255 || _action == 7)
		{
			int num29 = 0;
			int num30 = 0;
			for (int num31 = x; num31 <= x2; num31++)
			{
				for (int num32 = y; num32 <= y2; num32++)
				{
					if (Main.tile[num31, num32].type == 470 && Main.tile[num31, num32].frameX % 36 == 0 && Main.tile[num31, num32].frameY == 0 && TEDisplayDoll.Find(num31, num32) == -1)
					{
						if (TEDisplayDoll.Place(num31, num32) == -1)
						{
							num30++;
						}
						else
						{
							num29++;
						}
					}
				}
			}
			if (num29 > 0 || num30 > 0)
			{
				plr.SendSuccessMessage("Activated mannequins. ({0}){1}", num29, (num30 > 0) ? (" Failed to activate mannequins. (" + num30 + ")") : "");
			}
			else
			{
				num++;
			}
		}
		if (_action == 255 || _action == 8)
		{
			int num33 = 0;
			int num34 = 0;
			for (int num35 = x; num35 <= x2; num35++)
			{
				for (int num36 = y; num36 <= y2; num36++)
				{
					if (Main.tile[num35, num36].type == 475 && Main.tile[num35, num36].frameX % 54 == 0 && Main.tile[num35, num36].frameY == 0 && TEHatRack.Find(num35, num36) == -1)
					{
						if (TEHatRack.Place(num35, num36) == -1)
						{
							num34++;
						}
						else
						{
							num33++;
						}
					}
				}
			}
			if (num33 > 0 || num34 > 0)
			{
				plr.SendSuccessMessage("Activated hat racks. ({0}){1}", num33, (num34 > 0) ? (" Failed to activate hat racks. (" + num34 + ")") : "");
			}
			else
			{
				num++;
			}
		}
		if (_action == 255 || _action == 9)
		{
			int num37 = 0;
			int num38 = 0;
			for (int num39 = x; num39 <= x2; num39++)
			{
				for (int num40 = y; num40 <= y2; num40++)
				{
					if (Main.tile[num39, num40].type == 520 && TEFoodPlatter.Find(num39, num40) == -1)
					{
						if (TEFoodPlatter.Place(num39, num40) == -1)
						{
							num38++;
						}
						else
						{
							num37++;
						}
					}
				}
			}
			if (num37 > 0 || num38 > 0)
			{
				plr.SendSuccessMessage("Activated food plates. ({0}){1}", num37, (num38 > 0) ? (" Failed to activate food plates. (" + num38 + ")") : "");
			}
			else
			{
				num++;
			}
		}
		if (num == 10)
		{
			plr.SendSuccessMessage("There are no objects to activate in this area.");
		}
		ResetSection();
	}
}
