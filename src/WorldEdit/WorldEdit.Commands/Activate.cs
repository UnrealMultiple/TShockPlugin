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
		int emptyObjectTypes = 0;

		if (_action == 255 || _action == 0)
		{
			int activatedCount = 0;
			int failedCount = 0;
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if ((tile.type == 55 || tile.type == 85 || tile.type == 425) && tile.frameX % 36 == 0 && tile.frameY == 0 && Sign.ReadSign(tileX, tileY, false) == -1)
					{
						if (Sign.ReadSign(tileX, tileY, true) == -1)
						{
							failedCount++;
						}
						else
						{
							activatedCount++;
						}
					}
				}
			}
			emptyObjectTypes += ReportActivation("signs", activatedCount, failedCount);
		}

		if (_action == 255 || _action == 1)
		{
			int activatedCount = 0;
			int failedCount = 0;
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if ((tile.type == 21 || tile.type == 467 || tile.type == 88) && tile.frameX % 36 == 0 && tile.frameY % 36 == 0 && Chest.FindChest(tileX, tileY) == -1)
					{
						if (Chest.CreateChest(tileX, tileY, -1) == -1)
						{
							failedCount++;
						}
						else
						{
							activatedCount++;
						}
					}
				}
			}
			emptyObjectTypes += ReportActivation("chests", activatedCount, failedCount);
		}

		if (_action == 255 || _action == 2)
		{
			int activatedCount = 0;
			int failedCount = 0;
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (tile.type == 395 && tile.frameX % 36 == 0 && tile.frameY == 0 && TEItemFrame.Find(tileX, tileY) == -1)
					{
						if (TEItemFrame.Place(tileX, tileY) == -1)
						{
							failedCount++;
						}
						else
						{
							activatedCount++;
						}
					}
				}
			}
			emptyObjectTypes += ReportActivation("item frames", activatedCount, failedCount);
		}

		if (_action == 255 || _action == 3)
		{
			int activatedCount = 0;
			int failedCount = 0;
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					if (Main.tile[tileX, tileY].type != 423 || TELogicSensor.Find(tileX, tileY) != -1)
					{
						continue;
					}

					int entityId = TELogicSensor.Place(tileX, tileY);
					if (entityId == -1)
					{
						failedCount++;
						continue;
					}

					((TELogicSensor)TileEntity.ByID[entityId]).logicCheck = (TELogicSensor.LogicCheckType)(Main.tile[tileX, tileY].frameY / 18 + 1);
					activatedCount++;
				}
			}
			emptyObjectTypes += ReportActivation("logic sensors", activatedCount, failedCount);
		}

		emptyObjectTypes += ActivateTileEntities(4, 378, 36, TETrainingDummy.Find, TETrainingDummy.Place, "target dummies");
		emptyObjectTypes += ActivateTileEntities(5, 471, 54, TEWeaponsRack.Find, TEWeaponsRack.Place, "weapon racks");
		emptyObjectTypes += ActivateTileEntities(6, 597, 54, TETeleportationPylon.Find, TETeleportationPylon.Place, "pylons");
		emptyObjectTypes += ActivateTileEntities(7, 470, 36, TEDisplayDoll.Find, TEDisplayDoll.Place, "mannequins");
		emptyObjectTypes += ActivateTileEntities(8, 475, 54, TEHatRack.Find, TEHatRack.Place, "hat racks");
		emptyObjectTypes += ActivateTileEntities(9, 520, null, TEFoodPlatter.Find, TEFoodPlatter.Place, "food plates");

		if (emptyObjectTypes == 10)
		{
			plr.SendSuccessMessage("There are no objects to activate in this area.");
		}

		ResetSection();
	}

	private int ActivateTileEntities(byte action, int tileType, int? frameWidth, Func<int, int, int> findEntity, Func<int, int, int> placeEntity, string objectName)
	{
		if (_action != 255 && _action != action)
		{
			return 0;
		}

		int activatedCount = 0;
		int failedCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				ITile tile = Main.tile[tileX, tileY];
				if (tile.type != tileType || (frameWidth.HasValue && (tile.frameX % frameWidth.Value != 0 || tile.frameY != 0)) || findEntity(tileX, tileY) != -1)
				{
					continue;
				}

				if (placeEntity(tileX, tileY) == -1)
				{
					failedCount++;
				}
				else
				{
					activatedCount++;
				}
			}
		}

		return ReportActivation(objectName, activatedCount, failedCount);
	}

	private int ReportActivation(string objectName, int activatedCount, int failedCount)
	{
		if (activatedCount == 0 && failedCount == 0)
		{
			return 1;
		}

		plr.SendSuccessMessage("Activated {0}. ({1}){2}", objectName, activatedCount, failedCount > 0 ? $" Failed to activate {objectName}. ({failedCount})" : string.Empty);
		return 0;
	}
}
