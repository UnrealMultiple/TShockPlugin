using System.Security.Cryptography;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.IO;
using TShockAPI;
using TShockAPI.DB;

namespace CaiLib;

public static class CaiPlayer
{
    /// <summary>
    /// 导出玩家文件(hufang)
    /// </summary>
    /// <param name="account">玩家账号</param>
    /// <param name="path">路径</param>
    /// <param name=" difficulty">玩家角色难度(软核0，中核1，硬核2，旅行3)</param>
    public static void SavePlayerFile(UserAccount account, string path, int difficulty = 0)
    {
        var data = TShock.CharacterDB.GetPlayerData(new TSPlayer(-1), account.ID);
        SavePlayerFile(ModifyData(account.Name, data), path, difficulty);
    }
    /// <summary>
    /// 导出玩家文件(hufang)
    /// </summary>
    /// <param name="player">玩家对象</param>
    /// <param name="path">路径</param>
    /// <param name="difficulty">玩家角色难度(软核0，中核1，硬核2，旅行3)</param>
    public static void SavePlayerFile(Player player, string path, int difficulty = 0)
    {
        // Player.cs Serialize();
        //RijndaelManaged rijndaelManaged = new RijndaelManaged();
        //using (CryptoStream cryptoStream = new CryptoStream(stream, rijndaelManaged.CreateEncryptor(Player.ENCRYPTION_KEY, Player.ENCRYPTION_KEY), CryptoStreamMode.Write))
        var myAes = Aes.Create();
        using (Stream stream = new FileStream(path, FileMode.Create))
        {
            using (CryptoStream cryptoStream = new(stream, myAes.CreateEncryptor(Player.ENCRYPTION_KEY, Player.ENCRYPTION_KEY), CryptoStreamMode.Write))
            {
                PlayerFileData playerFileData = new()
                {
                    Metadata = FileMetadata.FromCurrentSettings(FileType.Player),
                    Player = player,
                    _isCloudSave = false,
                    _path = path
                };
                Main.LocalFavoriteData.ClearEntry(playerFileData);
                using (BinaryWriter binaryWriter = new(cryptoStream))
                {
                    //230 1.4.0.5
                    //269 1.4.4.0
                    binaryWriter.Write(269);
                    playerFileData.Metadata.Write(binaryWriter);
                    binaryWriter.Write(player.name);
                    binaryWriter.Write(difficulty);
                    binaryWriter.Write(playerFileData.GetPlayTime().Ticks);
                    binaryWriter.Write(player.hair);
                    binaryWriter.Write(player.hairDye);
                    BitsByte bitsByte = 0;
                    for (var i = 0; i < 8; i++)
                    {
                        bitsByte[i] = player.hideVisibleAccessory[i];
                    }
                    binaryWriter.Write(bitsByte);
                    bitsByte = 0;
                    for (var j = 0; j < 2; j++)
                    {
                        bitsByte[j] = player.hideVisibleAccessory[j + 8];
                    }
                    binaryWriter.Write(bitsByte);
                    binaryWriter.Write(player.hideMisc);
                    binaryWriter.Write((byte) player.skinVariant);
                    binaryWriter.Write(player.statLife);
                    binaryWriter.Write(player.statLifeMax);
                    binaryWriter.Write(player.statMana);
                    binaryWriter.Write(player.statManaMax);
                    binaryWriter.Write(player.extraAccessory);
                    binaryWriter.Write(player.unlockedBiomeTorches);
                    binaryWriter.Write(player.UsingBiomeTorches);

                    binaryWriter.Write(player.ateArtisanBread);
                    binaryWriter.Write(player.usedAegisCrystal);
                    binaryWriter.Write(player.usedAegisFruit);
                    binaryWriter.Write(player.usedArcaneCrystal);
                    binaryWriter.Write(player.usedGalaxyPearl);
                    binaryWriter.Write(player.usedGummyWorm);
                    binaryWriter.Write(player.usedAmbrosia);

                    binaryWriter.Write(player.downedDD2EventAnyDifficulty);
                    binaryWriter.Write(player.taxMoney);

                    binaryWriter.Write(player.numberOfDeathsPVE);
                    binaryWriter.Write(player.numberOfDeathsPVP);

                    binaryWriter.Write(player.hairColor.R);
                    binaryWriter.Write(player.hairColor.G);
                    binaryWriter.Write(player.hairColor.B);
                    binaryWriter.Write(player.skinColor.R);
                    binaryWriter.Write(player.skinColor.G);
                    binaryWriter.Write(player.skinColor.B);
                    binaryWriter.Write(player.eyeColor.R);
                    binaryWriter.Write(player.eyeColor.G);
                    binaryWriter.Write(player.eyeColor.B);
                    binaryWriter.Write(player.shirtColor.R);
                    binaryWriter.Write(player.shirtColor.G);
                    binaryWriter.Write(player.shirtColor.B);
                    binaryWriter.Write(player.underShirtColor.R);
                    binaryWriter.Write(player.underShirtColor.G);
                    binaryWriter.Write(player.underShirtColor.B);
                    binaryWriter.Write(player.pantsColor.R);
                    binaryWriter.Write(player.pantsColor.G);
                    binaryWriter.Write(player.pantsColor.B);
                    binaryWriter.Write(player.shoeColor.R);
                    binaryWriter.Write(player.shoeColor.G);
                    binaryWriter.Write(player.shoeColor.B);
                    for (var k = 0; k < player.armor.Length; k++)
                    {
                        binaryWriter.Write(player.armor[k].netID);
                        binaryWriter.Write(player.armor[k].prefix);
                    }
                    for (var l = 0; l < player.dye.Length; l++)
                    {
                        binaryWriter.Write(player.dye[l].netID);
                        binaryWriter.Write(player.dye[l].prefix);
                    }
                    for (var m = 0; m < 58; m++)
                    {
                        binaryWriter.Write(player.inventory[m].netID);
                        binaryWriter.Write(player.inventory[m].stack);
                        binaryWriter.Write(player.inventory[m].prefix);
                        binaryWriter.Write(player.inventory[m].favorited);
                    }
                    for (var n = 0; n < player.miscEquips.Length; n++)
                    {
                        binaryWriter.Write(player.miscEquips[n].netID);
                        binaryWriter.Write(player.miscEquips[n].prefix);
                        binaryWriter.Write(player.miscDyes[n].netID);
                        binaryWriter.Write(player.miscDyes[n].prefix);
                    }
                    for (var num = 0; num < 40; num++)
                    {
                        binaryWriter.Write(player.bank.item[num].netID);
                        binaryWriter.Write(player.bank.item[num].stack);
                        binaryWriter.Write(player.bank.item[num].prefix);
                    }
                    for (var num2 = 0; num2 < 40; num2++)
                    {
                        binaryWriter.Write(player.bank2.item[num2].netID);
                        binaryWriter.Write(player.bank2.item[num2].stack);
                        binaryWriter.Write(player.bank2.item[num2].prefix);
                    }
                    for (var num3 = 0; num3 < 40; num3++)
                    {
                        binaryWriter.Write(player.bank3.item[num3].netID);
                        binaryWriter.Write(player.bank3.item[num3].stack);
                        binaryWriter.Write(player.bank3.item[num3].prefix);
                    }
                    for (var num4 = 0; num4 < 40; num4++)
                    {
                        binaryWriter.Write(player.bank4.item[num4].netID);
                        binaryWriter.Write(player.bank4.item[num4].stack);
                        binaryWriter.Write(player.bank4.item[num4].prefix);
                        binaryWriter.Write(player.bank4.item[num4].favorited);
                    }
                    binaryWriter.Write(player.voidVaultInfo);
                    for (var num5 = 0; num5 < 44; num5++)
                    {
                        if (Main.buffNoSave[player.buffType[num5]])
                        {
                            binaryWriter.Write(0);
                            binaryWriter.Write(0);
                        }
                        else
                        {
                            binaryWriter.Write(player.buffType[num5]);
                            binaryWriter.Write(player.buffTime[num5]);
                        }
                    }
                    for (var num6 = 0; num6 < 200; num6++)
                    {
                        if (player.spN[num6] == null)
                        {
                            binaryWriter.Write(-1);
                            break;
                        }
                        binaryWriter.Write(player.spX[num6]);
                        binaryWriter.Write(player.spY[num6]);
                        binaryWriter.Write(player.spI[num6]);
                        binaryWriter.Write(player.spN[num6]);
                    }
                    binaryWriter.Write(player.hbLocked);
                    for (var num7 = 0; num7 < player.hideInfo.Length; num7++)
                    {
                        binaryWriter.Write(player.hideInfo[num7]);
                    }
                    binaryWriter.Write(player.anglerQuestsFinished);
                    for (var num8 = 0; num8 < player.DpadRadial.Bindings.Length; num8++)
                    {
                        binaryWriter.Write(player.DpadRadial.Bindings[num8]);
                    }
                    for (var num9 = 0; num9 < player.builderAccStatus.Length; num9++)
                    {
                        binaryWriter.Write(player.builderAccStatus[num9]);
                    }
                    binaryWriter.Write(player.bartenderQuestLog);
                    binaryWriter.Write(player.dead);
                    if (player.dead)
                    {
                        binaryWriter.Write(player.respawnTimer);
                    }
                    var value = DateTime.UtcNow.ToBinary();
                    binaryWriter.Write(value);
                    binaryWriter.Write(player.golferScoreAccumulated);
                    SaveSacrifice(binaryWriter);
                    player.SaveTemporaryItemSlotContents(binaryWriter);
                    CreativePowerManager.Instance.SaveToPlayer(player, binaryWriter);
                    BitsByte bitsByte2 = default;
                    bitsByte2[0] = player.unlockedSuperCart;
                    bitsByte2[1] = player.enabledSuperCart;
                    binaryWriter.Write(bitsByte2);
                    binaryWriter.Write(player.CurrentLoadoutIndex);
                    for (var num10 = 0; num10 < player.Loadouts.Length; num10++)
                    {
                        player.Loadouts[num10].Serialize(binaryWriter);
                    }

                    binaryWriter.Flush();
                    cryptoStream.FlushFinalBlock();
                    stream.Flush();

                }
            }
        }
    }

    /// <summary>
    /// 导出物品研究(hufang)
    /// </summary>
    /// <param name="writer"></param>
    public static void SaveSacrifice(BinaryWriter writer)
    {
        //player.creativeTracker.Save(binaryWriter);
        var dictionary = TShock.ResearchDatastore.GetSacrificedItems();
        writer.Write(dictionary.Count);
        foreach (var item in dictionary)
        {
            writer.Write(ContentSamples.ItemPersistentIdsByNetIds[item.Key]);
            writer.Write(item.Value);
        }
    }
    public static Item NetItem2Item(NetItem item)
    {
        var i = new Item();
        i.SetDefaults(item.NetId);
        i.stack = item.Stack;
        i.prefix = item.PrefixId;
        return i;
    }
    private static Player ModifyData(string name, PlayerData data)
    {
        Player player = new();
        if (data != null)
        {
            player.name = name;
            player.SpawnX = data.spawnX;
            player.SpawnY = data.spawnY;

            player.hideVisibleAccessory = data.hideVisuals;
            player.skinVariant = data.skinVariant ?? default;
            player.statLife = data.health;
            player.statLifeMax = data.maxHealth;
            player.statMana = data.mana;
            player.statManaMax = data.maxMana;
            player.extraAccessory = data.extraSlot == 1;

            player.difficulty = 0;

            // 火把神
            player.unlockedBiomeTorches = data.unlockedBiomeTorches == 1;

            player.hairColor = data.hairColor ?? default;
            player.skinColor = data.skinColor ?? default;
            player.eyeColor = data.eyeColor ?? default;
            player.shirtColor = data.shirtColor ?? default;
            player.underShirtColor = data.underShirtColor ?? default;
            player.pantsColor = data.pantsColor ?? default;
            player.shoeColor = data.shoeColor ?? default;

            player.hair = data.hair ?? default;
            player.hairDye = data.hairDye;

            player.anglerQuestsFinished = data.questsCompleted;
            player.CurrentLoadoutIndex = data.currentLoadoutIndex;

            //player.numberOfDeathsPVE = data.numberOfDeathsPVE;
            //player.numberOfDeathsPVP = data.numberOfDeathsPVP;


            for (var i = 0; i < NetItem.MaxInventory; i++)
            {
                // 0~49 背包   5*10
                // 50、51、52、53 钱
                // 54、55、56、57 弹药
                // 59 ~68  饰品栏
                // 69 ~78  社交栏
                // 79 ~88  染料1
                // 89 ~93  宠物、照明、矿车、坐骑、钩爪
                // 94 ~98  染料2
                // 99~138 储蓄罐
                // 139~178 保险箱（商人）
                // 179 垃圾桶
                // 180~219 护卫熔炉
                // 220~259 虚空保险箱
                // 260~350 装备123
                if (i < 59)
                {
                    player.inventory[i] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 59 && i < 79)
                {
                    player.armor[i - 59] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 79 && i < 89)
                {
                    player.dye[i - 79] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 89 && i < 94)
                {
                    player.miscEquips[i - 89] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 94 && i < 99)
                {
                    player.miscDyes[i - 94] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 99 && i < 139)
                {
                    player.bank.item[i - 99] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 139 && i < 179)
                {
                    player.bank2.item[i - 139] = NetItem2Item(data.inventory[i]);
                }
                else if (i == 179)
                {
                    player.trashItem = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 180 && i < 220)
                {
                    player.bank3.item[i - 180] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 220 && i < 260)
                {
                    player.bank4.item[i - 220] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 260 && i < 280)
                {
                    player.Loadouts[0].Armor[i - 260] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 280 && i < 290)
                {
                    player.Loadouts[0].Dye[i - 280] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 290 && i < 310)
                {
                    player.Loadouts[1].Armor[i - 290] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 310 && i < 320)
                {
                    player.Loadouts[1].Dye[i - 310] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 320 && i < 340)
                {
                    player.Loadouts[2].Armor[i - 320] = NetItem2Item(data.inventory[i]);
                }
                else if (i >= 340 && i < 350)
                {
                    player.Loadouts[2].Dye[i - 340] = NetItem2Item(data.inventory[i]);
                }
            }
        }
        return player;
    }

}