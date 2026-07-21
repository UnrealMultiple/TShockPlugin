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
		int removedSignCount = 0;
		int removedItemFrameCount = 0;
		int removedChestCount = 0;
		int removedLogicSensorCount = 0;
		int removedTrainingDummyCount = 0;
		int removedWeaponRackCount = 0;
		int removedPylonCount = 0;
		int removedDisplayDollCount = 0;
		int removedHatRackCount = 0;
		int removedFoodPlatterCount = 0;
		foreach (Sign sign in Main.sign)
		{
			if (sign != null)
			{
				ushort tileType = Main.tile[sign.x, sign.y].type;
				if (!Main.tile[sign.x, sign.y].active() || (tileType != 55 && tileType != 85 && tileType != 425))
				{
					Sign.KillSign(sign.x, sign.y);
					removedSignCount++;
				}
			}
		}
		foreach (Chest chest in Main.chest)
		{
			if (chest != null)
			{
				ushort tileType = Main.tile[chest.x, chest.y].type;
				if (!Main.tile[chest.x, chest.y].active() || (tileType != 21 && tileType != 467 && tileType != 88))
				{
					Chest.DestroyChest(chest.x, chest.y);
					removedChestCount++;
				}
			}
		}
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				if (TEItemFrame.Find(tileX, tileY) != -1)
				{
					if (!Main.tile[tileX, tileY].active() || Main.tile[tileX, tileY].type != 395)
					{
						TEItemFrame.Kill(tileX, tileY);
						removedItemFrameCount++;
					}
				}
				else if (TELogicSensor.Find(tileX, tileY) != -1)
				{
					if (!Main.tile[tileX, tileY].active() || Main.tile[tileX, tileY].type != 423)
					{
						TELogicSensor.Kill(tileX, tileY);
						removedLogicSensorCount++;
					}
				}
				else if (TETrainingDummy.Find(tileX, tileY) != -1)
				{
					if (!Main.tile[tileX, tileY].active() || Main.tile[tileX, tileY].type != 378)
					{
						TETrainingDummy.Kill(tileX, tileY);
						removedTrainingDummyCount++;
					}
				}
				else if (TEWeaponsRack.Find(tileX, tileY) != -1)
				{
					if (!Main.tile[tileX, tileY].active() || Main.tile[tileX, tileY].type != 471)
					{
						TEWeaponsRack.Kill(tileX, tileY);
						removedWeaponRackCount++;
					}
				}
				else if (TETeleportationPylon.Find(tileX, tileY) != -1)
				{
					if (!Main.tile[tileX, tileY].active() || Main.tile[tileX, tileY].type != 597)
					{
						TETeleportationPylon.Kill(tileX, tileY);
						removedPylonCount++;
					}
				}
				else if (TEDisplayDoll.Find(tileX, tileY) != -1)
				{
					if (!Main.tile[tileX, tileY].active() || Main.tile[tileX, tileY].type != 470)
					{
						TEDisplayDoll.Kill(tileX, tileY);
						removedDisplayDollCount++;
					}
				}
				else if (TEHatRack.Find(tileX, tileY) != -1)
				{
					if (!Main.tile[tileX, tileY].active() || Main.tile[tileX, tileY].type != 475)
					{
						TEHatRack.Kill(tileX, tileY);
						removedHatRackCount++;
					}
				}
				else if (TEFoodPlatter.Find(tileX, tileY) != -1 && (!Main.tile[tileX, tileY].active() || Main.tile[tileX, tileY].type != 520))
				{
					TEFoodPlatter.Kill(tileX, tileY);
					removedFoodPlatterCount++;
				}
			}
		}
		ResetSection();
		List<string> removedObjects = new List<string>();
		if (removedSignCount > 0)
		{
			removedObjects.Add($"signs ({removedSignCount})");
		}
		if (removedItemFrameCount > 0)
		{
			removedObjects.Add($"item frames ({removedItemFrameCount})");
		}
		if (removedChestCount > 0)
		{
			removedObjects.Add($"chests ({removedChestCount})");
		}
		if (removedLogicSensorCount > 0)
		{
			removedObjects.Add($"logic sensors ({removedLogicSensorCount})");
		}
		if (removedTrainingDummyCount > 0)
		{
			removedObjects.Add($"target dummies ({removedTrainingDummyCount})");
		}
		if (removedWeaponRackCount > 0)
		{
			removedObjects.Add($"weapon racks ({removedWeaponRackCount})");
		}
		if (removedPylonCount > 0)
		{
			removedObjects.Add($"pylons ({removedPylonCount})");
		}
		if (removedDisplayDollCount > 0)
		{
			removedObjects.Add($"mannequins ({removedDisplayDollCount})");
		}
		if (removedHatRackCount > 0)
		{
			removedObjects.Add($"hat racks ({removedHatRackCount})");
		}
		if (removedFoodPlatterCount > 0)
		{
			removedObjects.Add($"food plates ({removedFoodPlatterCount})");
		}
		if (removedObjects.Count > 0)
		{
			plr.SendSuccessMessage("Fixed ghost " + string.Join(", ", removedObjects) + ".");
		}
		else
		{
			plr.SendSuccessMessage("There are no ghost objects in this area.");
		}
	}
}
