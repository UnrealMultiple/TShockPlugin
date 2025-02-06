using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace CaiBot;

internal class LookBag
{
    public string Name = "";
    public int Health;
    public int MaxHealth;
    public int Mana;
    public int MaxMana;
    public int QuestsCompleted;

    public readonly List<List<int>> ItemList = new ();
    public readonly List<int> Enhances = new ();
    public List<int> Buffs = new ();

    public static LookBag LookOffline(UserAccount acc, PlayerData data)
    {
        var lookBagData = new LookBag
        {
            Name = acc.Name,
            Health = data.health,
            MaxHealth = data.maxHealth,
            Mana = data.mana,
            MaxMana = data.maxMana,
            QuestsCompleted = data.questsCompleted
        };
        if (data.extraSlot == 1)
        {
            lookBagData.Enhances.Add(3335); // 3335 恶魔之心
        }

        if (data.unlockedBiomeTorches == 1)
        {
            lookBagData.Enhances.Add(5043); // 5043 火把神徽章
        }

        if (data.ateArtisanBread == 1)
        {
            lookBagData.Enhances.Add(5326); // 5326	工匠面包
        }

        if (data.usedAegisCrystal == 1)
        {
            lookBagData.Enhances.Add(5337); // 5337 生命水晶	永久强化生命再生 
        }

        if (data.usedAegisFruit == 1)
        {
            lookBagData.Enhances.Add(5338); // 5338 埃癸斯果	永久提高防御力 
        }

        if (data.usedArcaneCrystal == 1)
        {
            lookBagData.Enhances.Add(5339); // 5339 奥术水晶	永久提高魔力再生 
        }

        if (data.usedGalaxyPearl == 1)
        {
            lookBagData.Enhances.Add(5340); // 5340	银河珍珠	永久增加运气 
        }

        if (data.usedGummyWorm == 1)
        {
            lookBagData.Enhances.Add(5341); // 5341	黏性蠕虫	永久提高钓鱼技能  
        }

        if (data.usedAmbrosia == 1)
        {
            lookBagData.Enhances.Add(5342); // 5342	珍馐	永久提高采矿和建造速度 
        }

        if (data.unlockedSuperCart == 1)
        {
            lookBagData.Enhances.Add(5289); // 5289	矿车升级包
        }

        foreach (var i in data.inventory)
        {
            lookBagData.ItemList.Add(new List<int> { i.NetId, i.Stack });
        }

        lookBagData.Buffs = Utils.GetActiveBuffs(TShock.DB, acc.ID, acc.Name);
        return lookBagData;
    }


    public static LookBag LookOnline(Player plr)
    {
        var lookBagData = new LookBag
        {
            Name = plr.name,
            Health = plr.statLife,
            MaxHealth = plr.statLifeMax,
            Mana = plr.statMana,
            MaxMana = plr.statManaMax,
            QuestsCompleted = plr.anglerQuestsFinished
        };
        if (plr.extraAccessory)
        {
            lookBagData.Enhances.Add(3335); // 3335 恶魔之心
        }

        if (plr.unlockedBiomeTorches)
        {
            lookBagData.Enhances.Add(5043); // 5043 火把神徽章
        }

        if (plr.ateArtisanBread)
        {
            lookBagData.Enhances.Add(5326); // 5326	工匠面包
        }

        if (plr.usedAegisCrystal)
        {
            lookBagData.Enhances.Add(5337); // 5337 生命水晶	永久强化生命再生 
        }

        if (plr.usedAegisFruit)
        {
            lookBagData.Enhances.Add(5338); // 5338 埃癸斯果	永久提高防御力 
        }

        if (plr.usedArcaneCrystal)
        {
            lookBagData.Enhances.Add(5339); // 5339 奥术水晶	永久提高魔力再生 
        }

        if (plr.usedGalaxyPearl)
        {
            lookBagData.Enhances.Add(5340); // 5340	银河珍珠	永久增加运气 
        }

        if (plr.usedGummyWorm)
        {
            lookBagData.Enhances.Add(5341); // 5341	黏性蠕虫	永久提高钓鱼技能  
        }

        if (plr.usedAmbrosia)
        {
            lookBagData.Enhances.Add(5342); // 5342	珍馐	 永久提高采矿和建造速度 
        }

        if (plr.unlockedSuperCart)
        {
            lookBagData.Enhances.Add(5289); // 5289	矿车升级包
        }

        lookBagData.Buffs = plr.buffType.ToList();
        var netItems = new NetItem[NetItem.MaxInventory];
        var inventory = plr.inventory;
        var armor = plr.armor;
        var dye = plr.dye;
        var miscEqups = plr.miscEquips;
        var miscDyes = plr.miscDyes;
        var piggy = plr.bank.item;
        var safe = plr.bank2.item;
        var forge = plr.bank3.item;
        var voidVault = plr.bank4.item;
        var trash = plr.trashItem;
        var loadout1Armor = plr.Loadouts[0].Armor;
        var loadout1Dye = plr.Loadouts[0].Dye;
        var loadout2Armor = plr.Loadouts[1].Armor;
        var loadout2Dye = plr.Loadouts[1].Dye;
        var loadout3Armor = plr.Loadouts[2].Armor;
        var loadout3Dye = plr.Loadouts[2].Dye;
        for (var i = 0; i < NetItem.MaxInventory; i++)
        {
            if (i < NetItem.InventoryIndex.Item2)
            {
                //0-58
                netItems[i] = (NetItem) inventory[i];
            }
            else if (i < NetItem.ArmorIndex.Item2)
            {
                //59-78
                var index = i - NetItem.ArmorIndex.Item1;
                netItems[i] = (NetItem) armor[index];
            }
            else if (i < NetItem.DyeIndex.Item2)
            {
                //79-88
                var index = i - NetItem.DyeIndex.Item1;
                netItems[i] = (NetItem) dye[index];
            }
            else if (i < NetItem.MiscEquipIndex.Item2)
            {
                //89-93
                var index = i - NetItem.MiscEquipIndex.Item1;
                netItems[i] = (NetItem) miscEqups[index];
            }
            else if (i < NetItem.MiscDyeIndex.Item2)
            {
                //93-98
                var index = i - NetItem.MiscDyeIndex.Item1;
                netItems[i] = (NetItem) miscDyes[index];
            }
            else if (i < NetItem.PiggyIndex.Item2)
            {
                //98-138
                var index = i - NetItem.PiggyIndex.Item1;
                netItems[i] = (NetItem) piggy[index];
            }
            else if (i < NetItem.SafeIndex.Item2)
            {
                //138-178
                var index = i - NetItem.SafeIndex.Item1;
                netItems[i] = (NetItem) safe[index];
            }
            else if (i < NetItem.TrashIndex.Item2)
            {
                //179-219
                netItems[i] = (NetItem) trash;
            }
            else if (i < NetItem.ForgeIndex.Item2)
            {
                //220
                var index = i - NetItem.ForgeIndex.Item1;
                netItems[i] = (NetItem) forge[index];
            }
            else if (i < NetItem.VoidIndex.Item2)
            {
                //220
                var index = i - NetItem.VoidIndex.Item1;
                netItems[i] = (NetItem) voidVault[index];
            }
            else if (i < NetItem.Loadout1Armor.Item2)
            {
                var index = i - NetItem.Loadout1Armor.Item1;
                netItems[i] = (NetItem) loadout1Armor[index];
            }
            else if (i < NetItem.Loadout1Dye.Item2)
            {
                var index = i - NetItem.Loadout1Dye.Item1;
                netItems[i] = (NetItem) loadout1Dye[index];
            }
            else if (i < NetItem.Loadout2Armor.Item2)
            {
                var index = i - NetItem.Loadout2Armor.Item1;
                netItems[i] = (NetItem) loadout2Armor[index];
            }
            else if (i < NetItem.Loadout2Dye.Item2)
            {
                var index = i - NetItem.Loadout2Dye.Item1;
                netItems[i] = (NetItem) loadout2Dye[index];
            }
            else if (i < NetItem.Loadout3Armor.Item2)
            {
                var index = i - NetItem.Loadout3Armor.Item1;
                netItems[i] = (NetItem) loadout3Armor[index];
            }
            else if (i < NetItem.Loadout3Dye.Item2)
            {
                var index = i - NetItem.Loadout3Dye.Item1;
                netItems[i] = (NetItem) loadout3Dye[index];
            }
        }

        foreach (var i in netItems)
        {
            lookBagData.ItemList.Add(new List<int> { i.NetId, i.Stack });
        }

        return lookBagData;
    }
}