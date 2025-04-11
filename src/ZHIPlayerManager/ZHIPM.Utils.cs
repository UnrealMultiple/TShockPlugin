using Microsoft.Xna.Framework;
using System.Security.Cryptography;
using System.Text;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using TShockAPI;
using TShockAPI.DB;
using Utils = Terraria.Utils;

namespace ZHIPlayerManager;

public partial class ZHIPM
{
    /// <summary>
    /// 同步玩家的人物数据数据，线上类型，仅同步在线玩家，在内存中的数据
    /// </summary>
    /// <param name="p">你要被同步的玩家</param>
    /// <param name="pd">赋值给他的数据</param>
    /// <returns></returns>
    private bool UpdatePlayerAll(TSPlayer p, PlayerData pd)
    {
        if (!pd.exists)
        {
            return false;
        }

        //正常同步
        if (pd.currentLoadoutIndex == p.TPlayer.CurrentLoadoutIndex)
        {
            for (var i = 0; i < NetItem.MaxInventory; i++)
            {
                if (i < NetItem.InventoryIndex.Item2)
                {
                    p.TPlayer.inventory[i] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.inventory[i].stack = pd.inventory[i].Stack;
                    p.TPlayer.inventory[i].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.ArmorIndex.Item2)
                {
                    var num = i - NetItem.ArmorIndex.Item1;
                    p.TPlayer.armor[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.armor[num].stack = pd.inventory[i].Stack;
                    p.TPlayer.armor[num].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.DyeIndex.Item2)
                {
                    var num2 = i - NetItem.DyeIndex.Item1;
                    p.TPlayer.dye[num2] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.dye[num2].stack = pd.inventory[i].Stack;
                    p.TPlayer.dye[num2].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.MiscEquipIndex.Item2)
                {
                    var num3 = i - NetItem.MiscEquipIndex.Item1;
                    p.TPlayer.miscEquips[num3] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.miscEquips[num3].stack = pd.inventory[i].Stack;
                    p.TPlayer.miscEquips[num3].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.MiscDyeIndex.Item2)
                {
                    var num4 = i - NetItem.MiscDyeIndex.Item1;
                    p.TPlayer.miscDyes[num4] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.miscDyes[num4].stack = pd.inventory[i].Stack;
                    p.TPlayer.miscDyes[num4].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.PiggyIndex.Item2)
                {
                    var num5 = i - NetItem.PiggyIndex.Item1;
                    p.TPlayer.bank.item[num5] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.bank.item[num5].stack = pd.inventory[i].Stack;
                    p.TPlayer.bank.item[num5].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.SafeIndex.Item2)
                {
                    var num6 = i - NetItem.SafeIndex.Item1;
                    p.TPlayer.bank2.item[num6] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.bank2.item[num6].stack = pd.inventory[i].Stack;
                    p.TPlayer.bank2.item[num6].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.TrashIndex.Item2)
                {
                    p.TPlayer.trashItem = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.trashItem.stack = pd.inventory[i].Stack;
                    p.TPlayer.trashItem.prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.ForgeIndex.Item2)
                {
                    var num7 = i - NetItem.ForgeIndex.Item1;
                    p.TPlayer.bank3.item[num7] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.bank3.item[num7].stack = pd.inventory[i].Stack;
                    p.TPlayer.bank3.item[num7].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.VoidIndex.Item2)
                {
                    var num8 = i - NetItem.VoidIndex.Item1;
                    p.TPlayer.bank4.item[num8] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.bank4.item[num8].stack = pd.inventory[i].Stack;
                    p.TPlayer.bank4.item[num8].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout1Armor.Item2)
                {
                    var num9 = i - NetItem.Loadout1Armor.Item1;
                    p.TPlayer.Loadouts[0].Armor[num9] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.Loadouts[0].Armor[num9].stack = pd.inventory[i].Stack;
                    p.TPlayer.Loadouts[0].Armor[num9].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout1Dye.Item2)
                {
                    var num10 = i - NetItem.Loadout1Dye.Item1;
                    p.TPlayer.Loadouts[0].Dye[num10] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.Loadouts[0].Dye[num10].stack = pd.inventory[i].Stack;
                    p.TPlayer.Loadouts[0].Dye[num10].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout2Armor.Item2)
                {
                    var num11 = i - NetItem.Loadout2Armor.Item1;
                    p.TPlayer.Loadouts[1].Armor[num11] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.Loadouts[1].Armor[num11].stack = pd.inventory[i].Stack;
                    p.TPlayer.Loadouts[1].Armor[num11].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout2Dye.Item2)
                {
                    var num12 = i - NetItem.Loadout2Dye.Item1;
                    p.TPlayer.Loadouts[1].Dye[num12] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.Loadouts[1].Dye[num12].stack = pd.inventory[i].Stack;
                    p.TPlayer.Loadouts[1].Dye[num12].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout3Armor.Item2)
                {
                    var num13 = i - NetItem.Loadout3Armor.Item1;
                    p.TPlayer.Loadouts[2].Armor[num13] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.Loadouts[2].Armor[num13].stack = pd.inventory[i].Stack;
                    p.TPlayer.Loadouts[2].Armor[num13].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout3Dye.Item2)
                {
                    var num14 = i - NetItem.Loadout3Dye.Item1;
                    p.TPlayer.Loadouts[2].Dye[num14] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.Loadouts[2].Dye[num14].stack = pd.inventory[i].Stack;
                    p.TPlayer.Loadouts[2].Dye[num14].prefix = pd.inventory[i].PrefixId;
                }
                p.SendData(PacketTypes.PlayerSlot, "", p.Index, i, pd.inventory[i].PrefixId);
            }
        }
        //异常同步
        else
        {
            var notSelected = 0;
            for (var i = 0; i < 3; i++)
            {
                if (p.TPlayer.CurrentLoadoutIndex != i && pd.currentLoadoutIndex != i)
                {
                    notSelected = i;
                }
            }
            for (var i = 0; i < NetItem.MaxInventory; i++)
            {
                if (i < NetItem.InventoryIndex.Item2)
                {
                    p.TPlayer.inventory[i] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.inventory[i].stack = pd.inventory[i].Stack;
                    p.TPlayer.inventory[i].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.ArmorIndex.Item2)
                {
                    var num = i - NetItem.ArmorIndex.Item1;
                    p.TPlayer.Loadouts[pd.currentLoadoutIndex].Armor[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.Loadouts[pd.currentLoadoutIndex].Armor[num].stack = pd.inventory[i].Stack;
                    p.TPlayer.Loadouts[pd.currentLoadoutIndex].Armor[num].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.DyeIndex.Item2)
                {
                    var num = i - NetItem.DyeIndex.Item1;
                    p.TPlayer.Loadouts[pd.currentLoadoutIndex].Dye[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.Loadouts[pd.currentLoadoutIndex].Dye[num].stack = pd.inventory[i].Stack;
                    p.TPlayer.Loadouts[pd.currentLoadoutIndex].Dye[num].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.MiscEquipIndex.Item2)
                {
                    var num = i - NetItem.MiscEquipIndex.Item1;
                    p.TPlayer.miscEquips[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.miscEquips[num].stack = pd.inventory[i].Stack;
                    p.TPlayer.miscEquips[num].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.MiscDyeIndex.Item2)
                {
                    var num = i - NetItem.MiscDyeIndex.Item1;
                    p.TPlayer.miscDyes[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.miscDyes[num].stack = pd.inventory[i].Stack;
                    p.TPlayer.miscDyes[num].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.PiggyIndex.Item2)
                {
                    var num = i - NetItem.PiggyIndex.Item1;
                    p.TPlayer.bank.item[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.bank.item[num].stack = pd.inventory[i].Stack;
                    p.TPlayer.bank.item[num].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.SafeIndex.Item2)
                {
                    var num = i - NetItem.SafeIndex.Item1;
                    p.TPlayer.bank2.item[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.bank2.item[num].stack = pd.inventory[i].Stack;
                    p.TPlayer.bank2.item[num].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.TrashIndex.Item2)
                {
                    p.TPlayer.trashItem = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.trashItem.stack = pd.inventory[i].Stack;
                    p.TPlayer.trashItem.prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.ForgeIndex.Item2)
                {
                    var num = i - NetItem.ForgeIndex.Item1;
                    p.TPlayer.bank3.item[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.bank3.item[num].stack = pd.inventory[i].Stack;
                    p.TPlayer.bank3.item[num].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.VoidIndex.Item2)
                {
                    var num = i - NetItem.VoidIndex.Item1;
                    p.TPlayer.bank4.item[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                    p.TPlayer.bank4.item[num].stack = pd.inventory[i].Stack;
                    p.TPlayer.bank4.item[num].prefix = pd.inventory[i].PrefixId;
                }
                else if (i < NetItem.Loadout1Armor.Item2)
                {
                    var num = i - NetItem.Loadout1Armor.Item1;
                    if (pd.currentLoadoutIndex != 0)
                    {
                        if (notSelected == 0)
                        {
                            p.TPlayer.Loadouts[0].Armor[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.Loadouts[0].Armor[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.Loadouts[0].Armor[num].prefix = pd.inventory[i].PrefixId;
                        }
                        else if (p.TPlayer.CurrentLoadoutIndex != 0)
                        {
                            p.TPlayer.Loadouts[0].Armor[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.Loadouts[0].Armor[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.Loadouts[0].Armor[num].prefix = pd.inventory[i].PrefixId;
                        }
                        else
                        {
                            p.TPlayer.Loadouts[0].Armor[num].TurnToAir();
                            p.TPlayer.armor[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.armor[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.armor[num].prefix = pd.inventory[i].PrefixId;
                        }

                    }
                }
                else if (i < NetItem.Loadout1Dye.Item2)
                {
                    var num = i - NetItem.Loadout1Dye.Item1;
                    if (pd.currentLoadoutIndex != 0)
                    {
                        if (notSelected == 0)
                        {
                            p.TPlayer.Loadouts[0].Dye[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.Loadouts[0].Dye[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.Loadouts[0].Dye[num].prefix = pd.inventory[i].PrefixId;
                        }
                        else if (p.TPlayer.CurrentLoadoutIndex != 0)
                        {
                            p.TPlayer.Loadouts[0].Dye[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.Loadouts[0].Dye[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.Loadouts[0].Dye[num].prefix = pd.inventory[i].PrefixId;
                        }
                        else
                        {
                            p.TPlayer.Loadouts[0].Dye[num].TurnToAir();
                            p.TPlayer.dye[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.dye[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.dye[num].prefix = pd.inventory[i].PrefixId;
                        }
                    }
                }
                else if (i < NetItem.Loadout2Armor.Item2)
                {
                    var num = i - NetItem.Loadout2Armor.Item1;
                    if (pd.currentLoadoutIndex != 1)
                    {
                        if (notSelected == 1)
                        {
                            p.TPlayer.Loadouts[1].Armor[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.Loadouts[1].Armor[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.Loadouts[1].Armor[num].prefix = pd.inventory[i].PrefixId;
                        }
                        else if (p.TPlayer.CurrentLoadoutIndex != 1)
                        {
                            p.TPlayer.Loadouts[1].Armor[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.Loadouts[1].Armor[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.Loadouts[1].Armor[num].prefix = pd.inventory[i].PrefixId;
                        }
                        else
                        {
                            p.TPlayer.Loadouts[1].Armor[num].TurnToAir();
                            p.TPlayer.armor[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.armor[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.armor[num].prefix = pd.inventory[i].PrefixId;
                        }
                    }
                }
                else if (i < NetItem.Loadout2Dye.Item2)
                {
                    var num = i - NetItem.Loadout2Dye.Item1;
                    if (pd.currentLoadoutIndex != 1)
                    {
                        if (notSelected == 1)
                        {
                            p.TPlayer.Loadouts[1].Dye[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.Loadouts[1].Dye[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.Loadouts[1].Dye[num].prefix = pd.inventory[i].PrefixId;
                        }
                        else if (p.TPlayer.CurrentLoadoutIndex != 1)
                        {
                            p.TPlayer.Loadouts[1].Dye[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.Loadouts[1].Dye[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.Loadouts[1].Dye[num].prefix = pd.inventory[i].PrefixId;
                        }
                        else
                        {
                            p.TPlayer.Loadouts[1].Dye[num].TurnToAir();
                            p.TPlayer.dye[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.dye[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.dye[num].prefix = pd.inventory[i].PrefixId;
                        }
                    }
                }
                else if (i < NetItem.Loadout3Armor.Item2)
                {
                    var num = i - NetItem.Loadout3Armor.Item1;
                    if (pd.currentLoadoutIndex != 2)
                    {
                        if (notSelected == 2)
                        {
                            p.TPlayer.Loadouts[2].Armor[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.Loadouts[2].Armor[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.Loadouts[2].Armor[num].prefix = pd.inventory[i].PrefixId;
                        }
                        else
                        {
                            if (p.TPlayer.CurrentLoadoutIndex != 2)
                            {
                                p.TPlayer.Loadouts[2].Armor[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                                p.TPlayer.Loadouts[2].Armor[num].stack = pd.inventory[i].Stack;
                                p.TPlayer.Loadouts[2].Armor[num].prefix = pd.inventory[i].PrefixId;
                            }
                            else
                            {
                                p.TPlayer.Loadouts[2].Armor[num].TurnToAir();
                                p.TPlayer.armor[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                                p.TPlayer.armor[num].stack = pd.inventory[i].Stack;
                                p.TPlayer.armor[num].prefix = pd.inventory[i].PrefixId;
                            }
                        }
                    }
                }
                else if (i < NetItem.Loadout3Dye.Item2)
                {
                    var num = i - NetItem.Loadout3Dye.Item1;
                    if (pd.currentLoadoutIndex != 2)
                    {
                        if (notSelected == 2)
                        {
                            p.TPlayer.Loadouts[2].Dye[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.Loadouts[2].Dye[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.Loadouts[2].Dye[num].prefix = pd.inventory[i].PrefixId;
                        }
                        else if (p.TPlayer.CurrentLoadoutIndex != 2)
                        {
                            p.TPlayer.Loadouts[2].Dye[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.Loadouts[2].Dye[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.Loadouts[2].Dye[num].prefix = pd.inventory[i].PrefixId;
                        }
                        else
                        {
                            p.TPlayer.Loadouts[2].Dye[num].TurnToAir();
                            p.TPlayer.dye[num] = TShock.Utils.GetItemById(pd.inventory[i].NetId);
                            p.TPlayer.dye[num].stack = pd.inventory[i].Stack;
                            p.TPlayer.dye[num].prefix = pd.inventory[i].PrefixId;
                        }
                    }
                }
            }
            for (var i = 0; i < NetItem.MaxInventory; i++)
            {
                p.SendData(PacketTypes.PlayerSlot, "", p.Index, i, pd.inventory[i].PrefixId);
            }
        }

        p.TPlayer.statLife = pd.health;
        p.TPlayer.statLifeMax = pd.maxHealth;
        p.TPlayer.statMana = pd.mana;
        p.TPlayer.statManaMax = pd.maxMana;
        p.TPlayer.extraAccessory = pd.extraSlot == 1;
        p.TPlayer.skinVariant = pd.skinVariant!.Value;
        p.TPlayer.hair = pd.hair!.Value;
        p.TPlayer.hairDye = pd.hairDye;
        p.TPlayer.hairColor = pd.hairColor!.Value;
        p.TPlayer.pantsColor = pd.pantsColor!.Value;
        p.TPlayer.shirtColor = pd.shirtColor!.Value;
        p.TPlayer.underShirtColor = pd.underShirtColor!.Value;
        p.TPlayer.shoeColor = pd.shoeColor!.Value;
        p.TPlayer.hideVisibleAccessory = pd.hideVisuals;
        p.TPlayer.skinColor = pd.skinColor!.Value;
        p.TPlayer.eyeColor = pd.eyeColor!.Value;
        p.TPlayer.anglerQuestsFinished = pd.questsCompleted;
        p.TPlayer.UsingBiomeTorches = pd.usingBiomeTorches == 1;
        p.TPlayer.happyFunTorchTime = pd.happyFunTorchTime == 1;
        p.TPlayer.unlockedBiomeTorches = pd.unlockedBiomeTorches == 1;
        p.TPlayer.ateArtisanBread = pd.ateArtisanBread == 1;
        p.TPlayer.usedAegisCrystal = pd.usedAegisCrystal == 1;
        p.TPlayer.usedAegisFruit = pd.usedAegisFruit == 1;
        p.TPlayer.usedAegisCrystal = pd.usedArcaneCrystal == 1;
        p.TPlayer.usedGalaxyPearl = pd.usedGalaxyPearl == 1;
        p.TPlayer.usedGummyWorm = pd.usedGummyWorm == 1;
        p.TPlayer.usedAmbrosia = pd.usedAmbrosia == 1;
        p.TPlayer.unlockedSuperCart = pd.unlockedSuperCart == 1;
        p.TPlayer.enabledSuperCart = pd.enabledSuperCart == 1;
        //玩家衣服服装，银河珍珠等属性的同步
        p.SendData(PacketTypes.PlayerInfo, "", p.Index);
        //生命值同步，包含最大值上限
        p.SendData(PacketTypes.PlayerHp, "", p.Index);
        //魔力值同步，包括上限
        p.SendData(PacketTypes.PlayerMana, "", p.Index);
        //钓鱼完成任务数目
        p.SendData(PacketTypes.NumberOfAnglerQuestsCompleted, "", p.Index);
        //清空玩家的buff
        ClearAllBuffFromPlayer(p);
        return true;
    }


    /// <summary>
    /// 更新玩家的人物属性数据，线下更新类型，写入数据库，不是更新在线的操作
    /// </summary>
    /// <param name="accId"></param>
    /// <param name="pd"></param>
    /// <returns></returns>
    private bool UpdateTshockDbCharacter(int accId, PlayerData pd)
    {
        if (!pd.exists)
        {
            return false;
        }
        try
        {
            var temp = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), accId);
            if (temp is { exists: true })
            {
                TShock.DB.Query("UPDATE tsCharacter SET Health = @0, MaxHealth = @1, Mana = @2, MaxMana = @3, Inventory = @4, spawnX = @6, spawnY = @7, hair = @8, hairDye = @9, hairColor = @10, pantsColor = @11, shirtColor = @12, underShirtColor = @13, shoeColor = @14, hideVisuals = @15, skinColor = @16, eyeColor = @17, questsCompleted = @18, skinVariant = @19, extraSlot = @20, usingBiomeTorches = @21, happyFunTorchTime = @22, unlockedBiomeTorches = @23, currentLoadoutIndex = @24, ateArtisanBread = @25, usedAegisCrystal = @26, usedAegisFruit = @27, usedArcaneCrystal = @28, usedGalaxyPearl = @29, usedGummyWorm = @30, usedAmbrosia = @31, unlockedSuperCart = @32, enabledSuperCart = @33 WHERE Account = @5;", pd.health, pd.maxHealth, pd.mana, pd.maxMana, string.Join("~", pd.inventory), accId, pd.spawnX, pd.spawnY, pd.hair!, pd.hairDye, TShock.Utils.EncodeColor(pd.hairColor)!, TShock.Utils.EncodeColor(pd.pantsColor)!, TShock.Utils.EncodeColor(pd.shirtColor)!, TShock.Utils.EncodeColor(pd.underShirtColor)!, TShock.Utils.EncodeColor(pd.shoeColor)!, TShock.Utils.EncodeBoolArray(pd.hideVisuals)!, TShock.Utils.EncodeColor(pd.skinColor)!, TShock.Utils.EncodeColor(pd.eyeColor)!, pd.questsCompleted, pd.skinVariant!, pd.extraSlot!, pd.usingBiomeTorches, pd.happyFunTorchTime, pd.unlockedBiomeTorches, pd.currentLoadoutIndex, pd.ateArtisanBread, pd.usedAegisCrystal, pd.usedAegisFruit, pd.usedArcaneCrystal, pd.usedGalaxyPearl, pd.usedGummyWorm, pd.usedAmbrosia, pd.unlockedSuperCart, pd.enabledSuperCart);
            }
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString("错误：UpdateTshockDBCharacter ") + ex);
            return false;
        }
    }


    /// <summary>
    /// 在线重置一个玩家的人物数据，离线自己删数据库去
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    private bool ResetPlayer(TSPlayer p)
    {
        if (!p.IsLoggedIn)
        {
            return false;
        }
        try
        {
            p.TPlayer.statLife = TShock.ServerSideCharacterConfig.Settings.StartingHealth;
            p.TPlayer.statLifeMax = TShock.ServerSideCharacterConfig.Settings.StartingHealth;
            p.TPlayer.statMana = TShock.ServerSideCharacterConfig.Settings.StartingMana;
            p.TPlayer.statManaMax = TShock.ServerSideCharacterConfig.Settings.StartingMana;
            //完全清理背包
            for (var i = 0; i < NetItem.MaxInventory; i++)
            {
                if (i < NetItem.InventoryIndex.Item2)
                {
                    p.TPlayer.inventory[i].TurnToAir();
                }
                else if (i < NetItem.ArmorIndex.Item2)
                {
                    var num = i - NetItem.ArmorIndex.Item1;
                    p.TPlayer.armor[num].TurnToAir();
                }
                else if (i < NetItem.DyeIndex.Item2)
                {
                    var num = i - NetItem.DyeIndex.Item1;
                    p.TPlayer.dye[num].TurnToAir();
                }
                else if (i < NetItem.MiscEquipIndex.Item2)
                {
                    var num = i - NetItem.MiscEquipIndex.Item1;
                    p.TPlayer.miscEquips[num].TurnToAir();
                }
                else if (i < NetItem.MiscDyeIndex.Item2)
                {
                    var num = i - NetItem.MiscDyeIndex.Item1;
                    p.TPlayer.miscDyes[num].TurnToAir();
                }
                else if (i < NetItem.PiggyIndex.Item2)
                {
                    var num = i - NetItem.PiggyIndex.Item1;
                    p.TPlayer.bank.item[num].TurnToAir();
                }
                else if (i < NetItem.SafeIndex.Item2)
                {
                    var num = i - NetItem.SafeIndex.Item1;
                    p.TPlayer.bank2.item[num].TurnToAir();
                }
                else if (i < NetItem.TrashIndex.Item2)
                {
                    p.TPlayer.trashItem.TurnToAir();
                }
                else if (i < NetItem.ForgeIndex.Item2)
                {
                    var num = i - NetItem.ForgeIndex.Item1;
                    p.TPlayer.bank3.item[num].TurnToAir();
                }
                else if (i < NetItem.VoidIndex.Item2)
                {
                    var num = i - NetItem.VoidIndex.Item1;
                    p.TPlayer.bank4.item[num].TurnToAir();
                }
                else if (i < NetItem.Loadout1Armor.Item2)
                {
                    var num = i - NetItem.Loadout1Armor.Item1;
                    p.TPlayer.Loadouts[0].Armor[num].TurnToAir();
                }
                else if (i < NetItem.Loadout1Dye.Item2)
                {
                    var num = i - NetItem.Loadout1Dye.Item1;
                    p.TPlayer.Loadouts[0].Dye[num].TurnToAir();
                }
                else if (i < NetItem.Loadout2Armor.Item2)
                {
                    var num = i - NetItem.Loadout2Armor.Item1;
                    p.TPlayer.Loadouts[1].Armor[num].TurnToAir();
                }
                else if (i < NetItem.Loadout2Dye.Item2)
                {
                    var num = i - NetItem.Loadout2Dye.Item1;
                    p.TPlayer.Loadouts[1].Dye[num].TurnToAir();
                }
                else if (i < NetItem.Loadout3Armor.Item2)
                {
                    var num = i - NetItem.Loadout3Armor.Item1;
                    p.TPlayer.Loadouts[2].Armor[num].TurnToAir();
                }
                else if (i < NetItem.Loadout3Dye.Item2)
                {
                    var num = i - NetItem.Loadout3Dye.Item1;
                    p.TPlayer.Loadouts[2].Dye[num].TurnToAir();
                }
                p.SendData(PacketTypes.PlayerSlot, "", p.Index, i);
            }
            //按照sscConfig的配置进行重置
            for (var i = 0; i < NetItem.MaxInventory; i++)
            {
                if (i < NetItem.InventoryIndex.Item2 && i < TShock.ServerSideCharacterConfig.Settings.StartingInventory.Count && TShock.ServerSideCharacterConfig.Settings.StartingInventory[i].NetId != 0)
                {
                    p.TPlayer.inventory[i] = TShock.Utils.GetItemById(TShock.ServerSideCharacterConfig.Settings.StartingInventory[i].NetId);
                    p.TPlayer.inventory[i].stack = TShock.ServerSideCharacterConfig.Settings.StartingInventory[i].Stack;
                    p.TPlayer.inventory[i].prefix = TShock.ServerSideCharacterConfig.Settings.StartingInventory[i].PrefixId;
                    p.SendData(PacketTypes.PlayerSlot, "", p.Index, i, p.TPlayer.inventory[i].prefix);
                }
            }
            p.TPlayer.unlockedBiomeTorches = false;
            p.TPlayer.extraAccessory = false;
            p.TPlayer.ateArtisanBread = false;
            p.TPlayer.usedAegisCrystal = false;
            p.TPlayer.usedAegisFruit = false;
            p.TPlayer.usedArcaneCrystal = false;
            p.TPlayer.usedGalaxyPearl = false;
            p.TPlayer.usedGummyWorm = false;
            p.TPlayer.usedAmbrosia = false;
            p.TPlayer.unlockedSuperCart = false;
            p.SendData(PacketTypes.PlayerInfo, "", p.Index);
            p.SendData(PacketTypes.PlayerHp, "", p.Index);
            p.SendData(PacketTypes.PlayerMana, "", p.Index);
            p.TPlayer.anglerQuestsFinished = 0;
            p.SendData(PacketTypes.NumberOfAnglerQuestsCompleted, "", p.Index);
            //清理所有buff
            ClearAllBuffFromPlayer(p);
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString("错误 ResetPlayer ：") + ex);
            return false;
        }
    }


    /// <summary>
    /// 返回玩家身上物品的字符串，一般情况slot = items.length
    /// </summary>
    /// <param name="items"></param>
    /// <param name="slots"></param>
    /// <param name="model">0 返回图标文本，1 返回纯文本</param>
    /// <returns></returns>
    private static string GetItemsString(Item[] items, int slots, int model = 0)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < slots; i++)
        {
            var item = items[i];
            if (model == 0 && !item.IsAir)
            {
                sb.Append(item.prefix != 0 ? $"【[i/p{item.prefix}:{item.netID}]】 " : $"【[i/s{item.stack}:{item.netID}]】 ");
            }
            if (model == 1 && !item.IsAir)
            {
                if (item.prefix != 0)
                {
                    sb.Append($"[{Lang.prefix[item.prefix].Value}.{item.Name}]");
                }
                else if (item.stack != 1)
                {
                    sb.Append($"[{item.Name}:{item.stack}]");
                }
                else
                {
                    sb.Append($"[{item.Name}]");
                }
            }
        }
        return sb.ToString();
    }


    /// <summary>
    /// 返回离线玩家身上的字符串，一般情况slot = items.length
    /// </summary>
    /// <param name="items"></param>
    /// <param name="slots"></param>
    /// <param name="model">0 返回图标文本，1 返回纯文本</param>
    /// <returns></returns>
    private static string GetItemsString(NetItem[] items, int slots, int model = 0)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < slots; i++)
        {
            var item = items[i];
            if (model == 0 && item.NetId != 0)
            {
                if (item.PrefixId != 0)
                {
                    sb.Append($"【[i/p{item.PrefixId}:{item.NetId}]】 ");
                }
                else
                {
                    sb.Append($"【[i/s{item.Stack}:{item.NetId}]】 ");
                }
            }
            if (model == 1 && item.NetId != 0)
            {
                if (item.PrefixId != 0)
                {
                    sb.Append($"[{Lang.prefix[item.PrefixId].Value}.{Lang.GetItemName(item.NetId)}]");
                }
                else if (item.Stack != 1)
                {
                    sb.Append($"[{Lang.GetItemName(item.NetId)}:{item.Stack}]");
                }
                else
                {
                    sb.Append($"[{Lang.GetItemName(item.NetId)}]");
                }
            }
        }
        return sb.ToString();
    }


    /// <summary>
    /// 给出一个字符串和每行几个物品数，返回排列好的字符串，按空格进行分割
    /// </summary>
    /// <param name="str"> 需要排列的文本 </param>
    /// <param name="num"> 一行几个 </param>
    /// <param name="block">  </param>
    /// <returns></returns>
    private static string FormatArrangement(string str, int num, string block = "")
    {
        if (!string.IsNullOrWhiteSpace(str))
        {
            var ls = str.Split(' ').ToList();
            for (var i = 0; i < ls.Count; i++)
            {
                if ((i + 1) % (num + 1) == 0)
                {
                    ls.Insert(i, "\n");
                }
            }

            if (block == "")
            {
                return string.Join(block, ls);
            }

            var sb = new StringBuilder();
            foreach (var s in ls)
            {
                if (s != "\n")
                {
                    sb.Append(s);
                    sb.Append(block);
                }
                else
                {
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        return "";
    }


    /// <summary>
    /// 返回随机好看的颜色
    /// </summary>
    /// <returns></returns>
    private static Color TextColor()
    {
        var r = Main.rand.Next(60, 255);
        var g = Main.rand.Next(60, 255);
        var b = Main.rand.Next(60, 255);
        return new Color(r, g, b);
    }


    /// <summary>
    /// 仅仅是回档指令的函数分出的方法，无特殊意义
    /// </summary>
    /// <param name="args"></param>
    /// <param name="slot"></param>
    private void MySscBack2(CommandArgs args, int slot)
    {
        var list = this.BestFindPlayerByNameOrIndex(args.Parameters[0]);
        if (list.Count > 1)
        {
            var names = GetString("检测到符合该条件的玩家数目不唯一，请重新输入\n包含：");
            foreach (var v in list)
            {
                names += v.Name + ", ";
            }
            _ = names.Remove(names.Length - 2, 2);
            args.Player.SendInfoMessage(names);
            return;
        }
        //离线回档
        if (list.Count == 0)
        {
            args.Player.SendInfoMessage(this.playerOfflineWarning);
            var user = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
            if (user == null)
            {
                args.Player.SendInfoMessage(this.nonExistPlayerWarning);
            }
            else
            {
                var playerData = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), user.ID);
                if (playerData == null || !playerData.exists)
                {
                    args.Player.SendInfoMessage(GetString("未在原数据库中查到该玩家，请检查输入是否正确，该玩家是否避免SSC检测，再重新输入"));
                }
                else
                {
                    try
                    {
                        var playerData2 = ZPDataBase.ReadZPlayerDB(new TSPlayer(-1), user.ID, slot);
                        if (!playerData2.exists)
                        {
                            args.Player.SendMessage(GetString($"回档失败！未找到 [{user.ID} - {slot}] 号该备份"), new Color(255, 0, 0));
                        }
                        else
                        {
                            if (this.UpdateTshockDbCharacter(user.ID, playerData2))
                            {
                                args.Player.SendMessage(GetString($"玩家 [{user.Name}] 回档成功！启用备份 [ {user.ID + "-" + slot} ]"), new Color(0, 255, 0));
                            }
                            else
                            {
                                args.Player.SendMessage(GetString("回档失败！"), new Color(255, 0, 0));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TShock.Log.ConsoleError(GetString("错误：BackUp ") + ex);
                    }
                }
            }
            return;
        }
        //在线回档
        if (list.Count == 1)
        {
            //如果这个人不是管理（无ssc避免）,需要同步在线属性UpdatePlayerAll，并且需要原版数据库写入更新才算成功
            if (!list[0].HasPermission(Permissions.bypassssc) && this.UpdatePlayerAll(list[0], ZPDataBase.ReadZPlayerDB(list[0], list[0].Account.ID, slot)) && TShock.CharacterDB.InsertPlayerData(list[0]))
            {
                if (args.Player.Index != list[0].Index)
                {
                    args.Player.SendMessage(GetString($"玩家 [{list[0].Name}] 回档成功！启用备份 [ {list[0].Account.ID + "-" + slot} ]"), new Color(0, 255, 0));
                    list[0].SendMessage(GetString("您已回档成功！"), new Color(0, 255, 0));
                }
                else
                {
                    args.Player.SendMessage(GetString($"玩家 [{list[0].Name}] 回档成功！启用备份 [ {list[0].Account.ID + "-" + slot} ]"), new Color(0, 255, 0));
                }
            }
            //如果他是管理，那就不用向原版数据写入了
            else if (list[0].HasPermission(Permissions.bypassssc) && this.UpdatePlayerAll(list[0], ZPDataBase.ReadZPlayerDB(list[0], list[0].Account.ID, slot)))
            {
                if (args.Player.Index != list[0].Index)
                {
                    args.Player.SendMessage(GetString($"玩家 [{list[0].Name}] 回档成功！启用备份 [ {list[0].Account.ID + "-" + slot} ]"), new Color(0, 255, 0));
                    list[0].SendMessage(GetString("您已回档成功！"), new Color(0, 255, 0));
                }
                else
                {
                    args.Player.SendMessage(GetString($"玩家 [{list[0].Name}] 回档成功！启用备份 [ {list[0].Account.ID + "-" + slot} ]"), new Color(0, 255, 0));
                }
            }
            else
            {
                args.Player.SendMessage(GetString("回档失败！未备份数据或该玩家未登录"), new Color(255, 0, 0));
            }
        }
    }


    /// <summary>
    /// 将 long 秒数时间转化成 xx小时 xx分钟 xx秒 的字符串
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private string Timetostring(long t)
    {
        var s = t % 60L;
        var min = t / 60L;
        var h = 0L;
        if (min >= 60L)
        {
            h = min / 60L;
            min %= 60L;
        }
        return GetString($"{h}小时 {min}分钟 {s}秒");
    }


    /// <summary>
    /// 将 long 硬币数转化成 xx铂 xx金 xx银 xx铜 的字符串
    /// </summary>
    /// <param name="coin"></param>
    /// <param name="model">0代表图标字符串，1代表纯文本</param>
    /// <returns></returns>
    private string Cointostring(long coin, int model = 0)
    {
        var copper = coin % 100;  //71
        coin /= 100;
        var silver = coin % 100; //72
        coin /= 100;
        var gold = coin % 100; //72
        coin /= 100;
        var platinum = coin; //74
        return model == 0
            ? GetString($"{platinum}[i:74] {gold}[i:73] {silver}[i:72] {copper}[i:71]")
            : GetString($"{platinum}铂金币  {gold}金币  {silver}银币  {copper}铜币");
    }


    /// <summary>
    /// 将 50~1,1~25, 这种类型的 id~击杀数, 字符串转化成对应的字典集合
    /// </summary>
    /// <param name="killstring"></param>
    /// <returns></returns>
    private static Dictionary<int, int> KillNpcStringToDictionary(string killstring)
    {
        if (string.IsNullOrWhiteSpace(killstring))
        {
            return new Dictionary<int, int>();
        }

        var keyValues = new Dictionary<int, int>();
        var list1 = new List<string>(killstring.Split(','));
        list1.RemoveAll(string.IsNullOrWhiteSpace);

        foreach (var str in list1)
        {
            var lst = new List<string>(str.Split('~'));
            lst.RemoveAll(x => !int.TryParse(x, out _));
            if (lst.Count == 2)
            {
                keyValues.Add(int.Parse(lst[0]), int.Parse(lst[1]));
            }
        }
        return keyValues;
    }


    /// <summary>
    /// 将字典集合转化成对应的  50~1,1~25, 这种类型的 id~击杀数, 字符串
    /// </summary>
    /// <param name="keyValues"></param>
    /// <returns></returns>
    private static string DictionaryToKillNpcString(Dictionary<int, int> keyValues)
    {
        var sb = new StringBuilder();
        foreach (var v in keyValues)
        {
            sb.Append(v.Key);
            sb.Append('~');
            sb.Append(v.Value);
            sb.Append(',');
        }
        return sb.ToString();
    }


    /// <summary>
    /// 将字典转化成 史莱姆王(2)，金金鱼(12)，宝箱怪(20) 这样的类型
    /// </summary>
    /// <param name="keyValues"> 数据 </param>
    /// <param name="iswrap"> 是否自动换行 </param>
    /// <returns></returns>
    private static string DictionaryToVsString(Dictionary<int, int> keyValues, bool iswrap = true)
    {
        var sb = new StringBuilder();
        var count = 0;
        foreach (var v in keyValues)
        {
            count++;
            switch (v.Key)//处理一下特殊npc
            {
                case 592:
                    sb.Append(GetString($"蹦跶{Lang.GetNPCNameValue(v.Key)}({v.Value})，"));
                    break;
                case 593:
                    sb.Append(GetString($"游雨{Lang.GetNPCNameValue(v.Key)}({v.Value})，"));
                    break;
                case 564:
                    sb.Append($"T1{Lang.GetNPCNameValue(v.Key)}({v.Value})，");
                    break;
                case 565:
                    sb.Append($"T3{Lang.GetNPCNameValue(v.Key)}({v.Value})，");
                    break;
                case 576:
                    sb.Append($"T2{Lang.GetNPCNameValue(v.Key)}({v.Value})，");
                    break;
                case 577:
                    sb.Append($"T3{Lang.GetNPCNameValue(v.Key)}({v.Value})，");
                    break;
                case 398:
                    sb.Append(GetString($"月亮领主({v.Value})"));
                    break;
                default:
                    sb.Append($"{Lang.GetNPCNameValue(v.Key)}({v.Value})，");
                    break;
            }
            if (count % 10 == 0 && iswrap)//防止一行字数过多卡到屏幕边缘看不见了
            {
                sb.AppendLine();
            }
        }
        if (sb.Length == 0)
        {
            sb.Append('无');
        }
        return sb.ToString().Trim().Trim('，');
    }


    /// <summary>
    /// 获得 击杀生物 的总数，由字典计算出
    /// </summary>
    /// <returns></returns>
    private int GetKillNumFromDictionary(Dictionary<int, int> keyValues)
    {
        return keyValues.Sum(v => v.Value);
    }


    /// <summary>
    /// 导出这个用户成存档plr
    /// </summary>
    /// <param name="player"></param>
    /// <param name="time"> 如果你想导出这个玩家的游玩时间就填，单位秒 </param>
    /// <returns></returns>
    private bool ExportPlayer(Player? player, long time = 0L)
    {
        if (player == null)
        {
            return false;
        }
        var text = new string(player.name);
        //移除不合法的字符
        text = this.FormatFileName(text);
        var worldname = new string(Main.worldName);
        //移除不合法的字符
        worldname = this.FormatFileName(worldname);
        var playerFileData = new PlayerFileData
        {
            Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
            Player = player,
            _isCloudSave = false
        };
        FileData fileData = playerFileData;
        fileData._path = TShock.SavePath + $"/Zhipm/{worldname + this.now}/{text}.plr";
        playerFileData.SetPlayTime(new TimeSpan(time * 10000000L));
        Main.LocalFavoriteData.ClearEntry(playerFileData);
        try
        {
            var path = playerFileData.Path;
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            if (!Directory.Exists(TShock.SavePath + "/Zhipm/" + worldname + this.now))
            {
                Directory.CreateDirectory(TShock.SavePath + "/Zhipm/" + worldname + this.now);
            }
            var aes = Aes.Create();
            using Stream stream = new FileStream(path, FileMode.Create);
            using var cryptoStream = new CryptoStream(stream, aes.CreateEncryptor(Player.ENCRYPTION_KEY, Player.ENCRYPTION_KEY), CryptoStreamMode.Write);
            using var binaryWriter = new BinaryWriter(cryptoStream);
            binaryWriter.Write(279);
            playerFileData.Metadata.Write(binaryWriter);
            Player.Serialize(playerFileData, player, binaryWriter);
            binaryWriter.Flush();
            cryptoStream.FlushFinalBlock();
            stream.Flush();

            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString("错误：ExportPlayer ") + ex);
            TShock.Log.ConsoleError(GetString("路径：") + playerFileData.Path + GetString(" 名字：") + text);
            return false;
        }
    }


    /// <summary>
    /// 创造一个玩家，复制其数据，用于导出人物存档
    /// </summary>
    /// <param name="name"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private Player? CreateAPlayer(string name, PlayerData data)
    {
        try
        {
            var player = new Player
            {
                name = name,
                statLife = data.health,
                statLifeMax = data.maxHealth,
                statMana = data.mana,
                statManaMax = data.maxMana,
                extraAccessory = data.extraSlot == 1,
                skinVariant = data.skinVariant!.Value,
                hair = data.hair!.Value,
                hairDye = data.hairDye,
                hairColor = data.hairColor!.Value,
                pantsColor = data.pantsColor!.Value,
                shirtColor = data.shirtColor!.Value,
                underShirtColor = data.underShirtColor!.Value,
                shoeColor = data.shoeColor!.Value,
                hideVisibleAccessory = data.hideVisuals,
                skinColor = data.skinColor!.Value,
                eyeColor = data.eyeColor!.Value,
                anglerQuestsFinished = data.questsCompleted,
                UsingBiomeTorches = data.usingBiomeTorches == 1,
                happyFunTorchTime = data.happyFunTorchTime == 1,
                unlockedBiomeTorches = data.unlockedBiomeTorches == 1,
                ateArtisanBread = data.ateArtisanBread == 1,
                usedAegisCrystal = data.usedAegisCrystal == 1,
                usedAegisFruit = data.usedAegisFruit == 1
            };
            player.usedAegisCrystal = data.usedArcaneCrystal == 1;
            player.usedGalaxyPearl = data.usedGalaxyPearl == 1;
            player.usedGummyWorm = data.usedGummyWorm == 1;
            player.usedAmbrosia = data.usedAmbrosia == 1;
            player.unlockedSuperCart = data.unlockedSuperCart == 1;
            player.enabledSuperCart = data.enabledSuperCart == 1;
            //正常同步
            if (data.currentLoadoutIndex == player.CurrentLoadoutIndex)
            {
                for (var i = 0; i < NetItem.MaxInventory; i++)
                {
                    if (i < NetItem.InventoryIndex.Item2)
                    {
                        player.inventory[i] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.inventory[i].stack = data.inventory[i].Stack;
                        player.inventory[i].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.ArmorIndex.Item2)
                    {
                        var num = i - NetItem.ArmorIndex.Item1;
                        player.armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.armor[num].stack = data.inventory[i].Stack;
                        player.armor[num].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.DyeIndex.Item2)
                    {
                        var num2 = i - NetItem.DyeIndex.Item1;
                        player.dye[num2] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.dye[num2].stack = data.inventory[i].Stack;
                        player.dye[num2].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.MiscEquipIndex.Item2)
                    {
                        var num3 = i - NetItem.MiscEquipIndex.Item1;
                        player.miscEquips[num3] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.miscEquips[num3].stack = data.inventory[i].Stack;
                        player.miscEquips[num3].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.MiscDyeIndex.Item2)
                    {
                        var num4 = i - NetItem.MiscDyeIndex.Item1;
                        player.miscDyes[num4] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.miscDyes[num4].stack = data.inventory[i].Stack;
                        player.miscDyes[num4].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.PiggyIndex.Item2)
                    {
                        var num5 = i - NetItem.PiggyIndex.Item1;
                        player.bank.item[num5] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.bank.item[num5].stack = data.inventory[i].Stack;
                        player.bank.item[num5].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.SafeIndex.Item2)
                    {
                        var num6 = i - NetItem.SafeIndex.Item1;
                        player.bank2.item[num6] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.bank2.item[num6].stack = data.inventory[i].Stack;
                        player.bank2.item[num6].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.TrashIndex.Item2)
                    {
                        player.trashItem = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.trashItem.stack = data.inventory[i].Stack;
                        player.trashItem.prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.ForgeIndex.Item2)
                    {
                        var num7 = i - NetItem.ForgeIndex.Item1;
                        player.bank3.item[num7] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.bank3.item[num7].stack = data.inventory[i].Stack;
                        player.bank3.item[num7].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.VoidIndex.Item2)
                    {
                        var num8 = i - NetItem.VoidIndex.Item1;
                        player.bank4.item[num8] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.bank4.item[num8].stack = data.inventory[i].Stack;
                        player.bank4.item[num8].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.Loadout1Armor.Item2)
                    {
                        var num9 = i - NetItem.Loadout1Armor.Item1;
                        player.Loadouts[0].Armor[num9] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.Loadouts[0].Armor[num9].stack = data.inventory[i].Stack;
                        player.Loadouts[0].Armor[num9].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.Loadout1Dye.Item2)
                    {
                        var num10 = i - NetItem.Loadout1Dye.Item1;
                        player.Loadouts[0].Dye[num10] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.Loadouts[0].Dye[num10].stack = data.inventory[i].Stack;
                        player.Loadouts[0].Dye[num10].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.Loadout2Armor.Item2)
                    {
                        var num11 = i - NetItem.Loadout2Armor.Item1;
                        player.Loadouts[1].Armor[num11] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.Loadouts[1].Armor[num11].stack = data.inventory[i].Stack;
                        player.Loadouts[1].Armor[num11].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.Loadout2Dye.Item2)
                    {
                        var num12 = i - NetItem.Loadout2Dye.Item1;
                        player.Loadouts[1].Dye[num12] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.Loadouts[1].Dye[num12].stack = data.inventory[i].Stack;
                        player.Loadouts[1].Dye[num12].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.Loadout3Armor.Item2)
                    {
                        var num13 = i - NetItem.Loadout3Armor.Item1;
                        player.Loadouts[2].Armor[num13] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.Loadouts[2].Armor[num13].stack = data.inventory[i].Stack;
                        player.Loadouts[2].Armor[num13].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.Loadout3Dye.Item2)
                    {
                        var num14 = i - NetItem.Loadout3Dye.Item1;
                        player.Loadouts[2].Dye[num14] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.Loadouts[2].Dye[num14].stack = data.inventory[i].Stack;
                        player.Loadouts[2].Dye[num14].prefix = data.inventory[i].PrefixId;
                    }
                }
            }
            //异常同步
            else
            {
                var notselected = 0;
                for (var i = 0; i < 3; i++)
                {
                    if (player.CurrentLoadoutIndex != i && data.currentLoadoutIndex != i)
                    {
                        notselected = i;
                    }
                }
                for (var i = 0; i < NetItem.MaxInventory; i++)
                {
                    if (i < NetItem.InventoryIndex.Item2)
                    {
                        player.inventory[i] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.inventory[i].stack = data.inventory[i].Stack;
                        player.inventory[i].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.ArmorIndex.Item2)
                    {
                        var num = i - NetItem.ArmorIndex.Item1;
                        player.Loadouts[data.currentLoadoutIndex].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.Loadouts[data.currentLoadoutIndex].Armor[num].stack = data.inventory[i].Stack;
                        player.Loadouts[data.currentLoadoutIndex].Armor[num].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.DyeIndex.Item2)
                    {
                        var num = i - NetItem.DyeIndex.Item1;
                        player.Loadouts[data.currentLoadoutIndex].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.Loadouts[data.currentLoadoutIndex].Dye[num].stack = data.inventory[i].Stack;
                        player.Loadouts[data.currentLoadoutIndex].Dye[num].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.MiscEquipIndex.Item2)
                    {
                        var num = i - NetItem.MiscEquipIndex.Item1;
                        player.miscEquips[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.miscEquips[num].stack = data.inventory[i].Stack;
                        player.miscEquips[num].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.MiscDyeIndex.Item2)
                    {
                        var num = i - NetItem.MiscDyeIndex.Item1;
                        player.miscDyes[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.miscDyes[num].stack = data.inventory[i].Stack;
                        player.miscDyes[num].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.PiggyIndex.Item2)
                    {
                        var num = i - NetItem.PiggyIndex.Item1;
                        player.bank.item[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.bank.item[num].stack = data.inventory[i].Stack;
                        player.bank.item[num].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.SafeIndex.Item2)
                    {
                        var num = i - NetItem.SafeIndex.Item1;
                        player.bank2.item[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.bank2.item[num].stack = data.inventory[i].Stack;
                        player.bank2.item[num].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.TrashIndex.Item2)
                    {
                        player.trashItem = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.trashItem.stack = data.inventory[i].Stack;
                        player.trashItem.prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.ForgeIndex.Item2)
                    {
                        var num = i - NetItem.ForgeIndex.Item1;
                        player.bank3.item[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.bank3.item[num].stack = data.inventory[i].Stack;
                        player.bank3.item[num].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.VoidIndex.Item2)
                    {
                        var num = i - NetItem.VoidIndex.Item1;
                        player.bank4.item[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                        player.bank4.item[num].stack = data.inventory[i].Stack;
                        player.bank4.item[num].prefix = data.inventory[i].PrefixId;
                    }
                    else if (i < NetItem.Loadout1Armor.Item2)
                    {
                        var num = i - NetItem.Loadout1Armor.Item1;
                        if (data.currentLoadoutIndex != 0)
                        {
                            if (notselected == 0)
                            {
                                player.Loadouts[0].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[0].Armor[num].stack = data.inventory[i].Stack;
                                player.Loadouts[0].Armor[num].prefix = data.inventory[i].PrefixId;
                            }
                            else if (player.CurrentLoadoutIndex != 0)
                            {
                                player.Loadouts[0].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[0].Armor[num].stack = data.inventory[i].Stack;
                                player.Loadouts[0].Armor[num].prefix = data.inventory[i].PrefixId;
                            }
                            else
                            {
                                player.Loadouts[0].Armor[num].TurnToAir();
                                player.armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.armor[num].stack = data.inventory[i].Stack;
                                player.armor[num].prefix = data.inventory[i].PrefixId;
                            }

                        }
                    }
                    else if (i < NetItem.Loadout1Dye.Item2)
                    {
                        var num = i - NetItem.Loadout1Dye.Item1;
                        if (data.currentLoadoutIndex != 0)
                        {
                            if (notselected == 0)
                            {
                                player.Loadouts[0].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[0].Dye[num].stack = data.inventory[i].Stack;
                                player.Loadouts[0].Dye[num].prefix = data.inventory[i].PrefixId;
                            }
                            else if (player.CurrentLoadoutIndex != 0)
                            {
                                player.Loadouts[0].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[0].Dye[num].stack = data.inventory[i].Stack;
                                player.Loadouts[0].Dye[num].prefix = data.inventory[i].PrefixId;
                            }
                            else
                            {
                                player.Loadouts[0].Dye[num].TurnToAir();
                                player.dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.dye[num].stack = data.inventory[i].Stack;
                                player.dye[num].prefix = data.inventory[i].PrefixId;
                            }
                        }
                    }
                    else if (i < NetItem.Loadout2Armor.Item2)
                    {
                        var num = i - NetItem.Loadout2Armor.Item1;
                        if (data.currentLoadoutIndex != 1)
                        {
                            if (notselected == 1)
                            {
                                player.Loadouts[1].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[1].Armor[num].stack = data.inventory[i].Stack;
                                player.Loadouts[1].Armor[num].prefix = data.inventory[i].PrefixId;
                            }
                            else if (player.CurrentLoadoutIndex != 1)
                            {
                                player.Loadouts[1].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[1].Armor[num].stack = data.inventory[i].Stack;
                                player.Loadouts[1].Armor[num].prefix = data.inventory[i].PrefixId;
                            }
                            else
                            {
                                player.Loadouts[1].Armor[num].TurnToAir();
                                player.armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.armor[num].stack = data.inventory[i].Stack;
                                player.armor[num].prefix = data.inventory[i].PrefixId;
                            }
                        }
                    }
                    else if (i < NetItem.Loadout2Dye.Item2)
                    {
                        var num = i - NetItem.Loadout2Dye.Item1;
                        if (data.currentLoadoutIndex != 1)
                        {
                            if (notselected == 1)
                            {
                                player.Loadouts[1].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[1].Dye[num].stack = data.inventory[i].Stack;
                                player.Loadouts[1].Dye[num].prefix = data.inventory[i].PrefixId;
                            }
                            else if (player.CurrentLoadoutIndex != 1)
                            {
                                player.Loadouts[1].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[1].Dye[num].stack = data.inventory[i].Stack;
                                player.Loadouts[1].Dye[num].prefix = data.inventory[i].PrefixId;
                            }
                            else
                            {
                                player.Loadouts[1].Dye[num].TurnToAir();
                                player.dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.dye[num].stack = data.inventory[i].Stack;
                                player.dye[num].prefix = data.inventory[i].PrefixId;
                            }
                        }
                    }
                    else if (i < NetItem.Loadout3Armor.Item2)
                    {
                        var num = i - NetItem.Loadout3Armor.Item1;
                        if (data.currentLoadoutIndex != 2)
                        {
                            if (notselected == 2)
                            {
                                player.Loadouts[2].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[2].Armor[num].stack = data.inventory[i].Stack;
                                player.Loadouts[2].Armor[num].prefix = data.inventory[i].PrefixId;
                            }
                            else
                            {
                                if (player.CurrentLoadoutIndex != 2)
                                {
                                    player.Loadouts[2].Armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                    player.Loadouts[2].Armor[num].stack = data.inventory[i].Stack;
                                    player.Loadouts[2].Armor[num].prefix = data.inventory[i].PrefixId;
                                }
                                else
                                {
                                    player.Loadouts[2].Armor[num].TurnToAir();
                                    player.armor[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                    player.armor[num].stack = data.inventory[i].Stack;
                                    player.armor[num].prefix = data.inventory[i].PrefixId;
                                }
                            }
                        }
                    }
                    else if (i < NetItem.Loadout3Dye.Item2)
                    {
                        var num = i - NetItem.Loadout3Dye.Item1;
                        if (data.currentLoadoutIndex != 2)
                        {
                            if (notselected == 2)
                            {
                                player.Loadouts[2].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[2].Dye[num].stack = data.inventory[i].Stack;
                                player.Loadouts[2].Dye[num].prefix = data.inventory[i].PrefixId;
                            }
                            else if (player.CurrentLoadoutIndex != 2)
                            {
                                player.Loadouts[2].Dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.Loadouts[2].Dye[num].stack = data.inventory[i].Stack;
                                player.Loadouts[2].Dye[num].prefix = data.inventory[i].PrefixId;
                            }
                            else
                            {
                                player.Loadouts[2].Dye[num].TurnToAir();
                                player.dye[num] = TShock.Utils.GetItemById(data.inventory[i].NetId);
                                player.dye[num].stack = data.inventory[i].Stack;
                                player.dye[num].prefix = data.inventory[i].PrefixId;
                            }
                        }
                    }
                }
            }
            return player;
        }
        catch
        {
            TShock.Log.ConsoleError(GetString($"正常的意外因玩家 [ {name} ] 数据残缺而导出人物失败 CreateAPlayer"));
            return null;
        }
    }


    /// <summary>
    /// 最好的 在线 查找，先查找用户索引，索引不会被名字干扰，找不到再匹配名字完全相同的玩家，包括大小写，再找不到就模糊查找，不区分大小写
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private List<TSPlayer> BestFindPlayerByNameOrIndex(string str)
    {
        if (int.TryParse(str, out var num))
        {
            foreach (var tsPlayer in TShock.Players)
            {
                if (tsPlayer != null && tsPlayer.Index == num)
                {
                    return new List<TSPlayer> { tsPlayer };
                }
            }
        }
        foreach (var tsplayer in TShock.Players)
        {
            if (tsplayer != null && tsplayer.Name.Equals(str))
            {
                return new List<TSPlayer> { tsplayer };
            }
        }
        var list = new List<TSPlayer>();
        foreach (var tsplayer in TShock.Players)
        {
            if (tsplayer != null && tsplayer.Name.Contains(str, StringComparison.OrdinalIgnoreCase))
            {
                list.Add(tsplayer);
            }
        }
        return list;
    }


    /// <summary>
    /// 获得这个玩家身上的钱币数目，单位铜币，支持离线和在线，返回-1代表这个玩家不存在或数据错误
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private long GetPlayerCoin(string name)
    {
        //如果在线的话
        foreach (var ts in TShock.Players)
        {
            if (ts != null && ts.Name == name)
            {
                var num = Utils.CoinsCount(out _, ts.TPlayer.inventory, 58, 57, 56, 55, 54);
                var num2 = Utils.CoinsCount(out _, ts.TPlayer.bank.item);
                var num3 = Utils.CoinsCount(out _, ts.TPlayer.bank2.item);
                var num4 = Utils.CoinsCount(out _, ts.TPlayer.bank3.item);
                var num5 = Utils.CoinsCount(out _, ts.TPlayer.bank4.item);
                return num + num2 + num3 + num4 + num5;
            }
        }
        //离线的话
        var user = TShock.UserAccounts.GetUserAccountByName(name);

        if (user == null)
        {
            return -1L;
        }

        var pd = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), user.ID);

        if (!pd.exists)
        {
            return -1L;
        }

        var items = new List<Item>();
        foreach (var t in pd.inventory)
        {
            if (t.NetId is 71 or 72 or 73 or 74)
            {
                var temp = TShock.Utils.GetItemById(t.NetId);
                temp.stack = t.Stack;
                temp.prefix = t.PrefixId;
                items.Add(temp);
            }
        }
        return Utils.CoinsCount(out _, items.ToArray());

    }


    /// <summary>
    /// 清理这个玩家身上所有buff
    /// </summary>
    /// <param name="ts"></param>
    private static void ClearAllBuffFromPlayer(TSPlayer ts)
    {
        for (var i = 0; i < 22; i++)
        {
            ts.TPlayer.buffType[i] = 0;
        }
        ts.SendData(PacketTypes.PlayerBuff, "", ts.Index);
    }


    /// <summary>
    /// 将tshock里的ips 转换成单独的ip字符串数组
    /// </summary>
    /// <param name="knownIps"></param>
    /// <returns></returns>
    private static string[] ToIpStrings(string knownIps)
    {
        if (string.IsNullOrEmpty(knownIps))
        {
            return Array.Empty<string>();
        }

        var ips = knownIps.Split(',');
        for (var i = 0; i < ips.Length; i++)
        {
            ips[i] = ips[i].Replace("\"", "");
            ips[i] = ips[i].Replace("[", "");
            ips[i] = ips[i].Replace("]", "");
            ips[i] = ips[i].Trim();
        }
        return ips;
    }


    /// <summary>
    /// 向某个玩家发送悬浮字体
    /// </summary>
    /// <param name="ts"> 需要发送的玩家 </param>
    /// <param name="text"> 内容文本 </param>
    /// <param name="color"> 颜色 </param>
    /// <param name="pos"> 位置 </param>
    private void SendText(TSPlayer ts, string text, Color color, Vector2 pos)
    {
        ts.SendData(PacketTypes.CreateCombatTextExtended, text, (int) color.packedValue, pos.X, pos.Y);
    }


    /// <summary>
    /// 给所有玩家发送悬浮字体，但是根据发起者区分颜色
    /// </summary>
    /// <param name="ts"> 需要发送的玩家 </param>
    /// <param name="text"> 发送文本 </param>
    /// <param name="color1"> 被发送者所看见的颜色 </param>
    /// <param name="color2"> 除了被发送者其他玩家所看见的颜色 </param>
    /// <param name="pos"> 位置 </param>
    private void SendAllText(TSPlayer ts, string text, Color color1, Color color2, Vector2 pos)
    {
        if (!ts.RealPlayer || ts.ConnectionAlive)
        {
            NetMessage.SendData(119, ts.Index, -1, NetworkText.FromLiteral(text), (int) color1.packedValue, pos.X, pos.Y);
            NetMessage.SendData(119, -1, ts.Index, NetworkText.FromLiteral(text), (int) color2.packedValue, pos.X, pos.Y);
        }
    }


    /// <summary>
    /// 将boss击杀排行榜的信息打印出来
    /// </summary>
    /// <param name="bossName"></param>
    /// <param name="playerAndDamage"></param>
    /// <param name="allDamage"></param>
    private static void SendKillBossMessage(string bossName, Dictionary<string, int> playerAndDamage, int allDamage)
    {
        var sb = new StringBuilder();
        var sortpairs = new Dictionary<string, int>();

        sb.AppendLine(GetString($"共有 [c/74F3C9:{playerAndDamage.Count}] 位玩家击败了 [c/74F3C9:{bossName}]"));
        //简单的排个序
        while (playerAndDamage.Count > 0)
        {
            var key = "";
            var damage = 0;
            foreach (var v in playerAndDamage)
            {
                if (v.Value > damage)
                {
                    key = v.Key;
                    damage = v.Value;
                }
            }
            if (key != "")
            {
                sortpairs.Add(key, damage);
                playerAndDamage.Remove(key);
            }
        }

        foreach (var v in sortpairs)
        {
            sb.AppendLine(GetString($"{v.Key}    伤害: [c/74F3C9:{v.Value}]    比重: {v.Value * 1.0f / allDamage:0.00%} "));
        }
        TSPlayer.All.SendMessage(sb.ToString(), Color.Bisque);
    }


    /// <summary>
    /// 将 string 转化为能直接作用于文件名的 string
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string FormatFileName(string text)
    {
        //移除不合法的字符
        for (var i = 0; i < text.Length; ++i)
        {
            var flag = text[i] == '\\' || text[i] == '/' || text[i] == ':' || text[i] == '*' || text[i] == '?' || text[i] == '"' || text[i] == '<' || text[i] == '>' || text[i] == '|';
            if (flag)
            {
                text = text.Replace(text[i], '-');
            }
        }
        return text;
    }
}