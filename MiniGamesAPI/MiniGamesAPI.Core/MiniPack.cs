using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace MiniGamesAPI
{
    public class MiniPack
    {
        public string Name { get; set; }

        public int ID { get; set; }

        public int UnlockedBiomeTorches { get; set; }

        public int HappyFunTorchTime { get; set; }

        public int UsingBiomeTorches { get; set; }

        public int QuestsCompleted { get; set; }

        public bool[] HideVisuals { get; set; }

        public Color? EyeColor { get; set; }

        public Color? SkinColor { get; set; }

        public Color? ShoeColor { get; set; }

        public Color? UnderShirtColor { get; set; }

        public Color? ShirtColor { get; set; }

        public Color? HairColor { get; set; }

        public Color? PantsColor { get; set; }

        public int? Hair { get; set; }

        public int? SkinVariant { get; set; }

        public int? ExtraSlots { get; set; }

        public int SpawnY { get; set; }

        public int SpawnX { get; set; }

        public bool Exists { get; set; }

        public int MaxMana { get; set; }

        public int Mana { get; set; }

        public int MaxHP { get; set; }

        public int HP { get; set; }

        public byte HairDye { get; set; }

        public List<MiniItem> Items { get; set; }

        public MiniPack(string name, int id)
        {
            Name = name;
            ID = id;
            Hair = 0;
            SkinVariant = 0;
            ExtraSlots = 0;
            SpawnY = -1;
            SpawnX = -1;
            Exists = true;
            MaxMana = 20;
            Mana = 20;
            HP = 100;
            MaxHP = 100;
            HairDye = 0;
            HideVisuals = new bool[10];
            UnlockedBiomeTorches = 0;
            HappyFunTorchTime = 0;
            UsingBiomeTorches = 0;
            QuestsCompleted = 0;
            EyeColor = new Color(4283128425u);
            SkinColor = new Color(4284120575u);
            ShoeColor = new Color(4282149280u);
            UnderShirtColor = new Color(4292326560u);
            ShirtColor = new Color(4287407535u);
            HairColor = new Color(4287407535u);
            PantsColor = new Color(4287407535u);
            Items = new List<MiniItem>();
        }

        public MiniPack GetCopyNoItems(string name, int id)
        {
            return new MiniPack(name, id)
            {
                Exists = Exists,
                ExtraSlots = ExtraSlots,
                EyeColor = EyeColor,
                Hair = Hair,
                HairColor = HairColor,
                HairDye = HairDye,
                HappyFunTorchTime = HappyFunTorchTime,
                HideVisuals = HideVisuals,
                HP = HP,
                Items = new List<MiniItem>(),
                Mana = Mana,
                MaxHP = MaxHP,
                MaxMana = MaxMana,
                PantsColor = PantsColor,
                QuestsCompleted = QuestsCompleted,
                ShirtColor = ShirtColor,
                ShoeColor = ShoeColor,
                SkinColor = SkinColor,
                SkinVariant = SkinVariant,
                SpawnX = SpawnX,
                SpawnY = SpawnY,
                UnderShirtColor = UnderShirtColor,
                UnlockedBiomeTorches = UnlockedBiomeTorches,
                UsingBiomeTorches = UsingBiomeTorches
            };
        }

        public void RestoreCharacter(TSPlayer player)
        {
            player.IgnoreSSCPackets = true;
            player.TPlayer.statLife = HP;
            player.TPlayer.statLifeMax = MaxHP;
            player.TPlayer.statMana = Mana;
            player.TPlayer.statManaMax = MaxMana;
            player.TPlayer.SpawnX = SpawnX;
            player.TPlayer.SpawnY = SpawnY;
            player.sX = SpawnX;
            player.sY = SpawnY;
            player.TPlayer.hairDye = HairDye;
            player.TPlayer.anglerQuestsFinished = QuestsCompleted;
            if (ExtraSlots.HasValue)
            {
                player.TPlayer.extraAccessory = ExtraSlots.Value == 1;
            }
            if (SkinVariant.HasValue)
            {
                player.TPlayer.skinVariant = SkinVariant.Value;
            }
            if (Hair.HasValue)
            {
                player.TPlayer.hair = Hair.Value;
            }
            if (HairColor.HasValue)
            {
                player.TPlayer.hairColor = HairColor.Value;
            }
            if (PantsColor.HasValue)
            {
                player.TPlayer.pantsColor = PantsColor.Value;
            }
            if (ShirtColor.HasValue)
            {
                player.TPlayer.shirtColor = ShirtColor.Value;
            }
            if (UnderShirtColor.HasValue)
            {
                player.TPlayer.underShirtColor = UnderShirtColor.Value;
            }
            if (ShoeColor.HasValue)
            {
                player.TPlayer.shoeColor = ShoeColor.Value;
            }
            if (SkinColor.HasValue)
            {
                player.TPlayer.skinColor = SkinColor.Value;
            }
            if (EyeColor.HasValue)
            {
                player.TPlayer.eyeColor = EyeColor.Value;
            }
            if (HideVisuals != null)
            {
                player.TPlayer.hideVisibleAccessory = HideVisuals;
            }
            else
            {
                player.TPlayer.hideVisibleAccessory = new bool[player.TPlayer.hideVisibleAccessory.Length];
            }
            for (int i = 0; i < NetItem.MaxInventory; i++)
            {
                if (i < 59)
                {
                    player.TPlayer.inventory[i].netDefaults(0);
                }
                else if (i < 79)
                {
                    int num = i - NetItem.ArmorIndex.Item1;
                    player.TPlayer.armor[num].netDefaults(0);
                }
                else if (i < 89)
                {
                    int num2 = i - NetItem.DyeIndex.Item1;
                    player.TPlayer.dye[num2].netDefaults(0);
                }
                else if (i < 94)
                {
                    int num3 = i - NetItem.MiscEquipIndex.Item1;
                    player.TPlayer.miscEquips[num3].netDefaults(0);
                }
                else if (i < 99)
                {
                    int num4 = i - NetItem.MiscDyeIndex.Item1;
                    player.TPlayer.miscDyes[num4].netDefaults(0);
                }
                else if (i < 139)
                {
                    int num5 = i - NetItem.PiggyIndex.Item1;
                    player.TPlayer.bank.item[num5].netDefaults(0);
                }
                else if (i < 179)
                {
                    int num6 = i - NetItem.SafeIndex.Item1;
                    player.TPlayer.bank2.item[num6].netDefaults(0);
                }
                else if (i < 220)
                {
                    if (i == 179)
                    {
                        player.TPlayer.trashItem.netDefaults(0);
                        continue;
                    }
                    int num7 = i - NetItem.ForgeIndex.Item1;
                    player.TPlayer.bank3.item[num7].netDefaults(0);
                }
                else
                {
                    int num8 = i - NetItem.VoidIndex.Item1;
                    player.TPlayer.bank4.item[num8].netDefaults(0);
                }
            }
            for (int j = 0; j < Items.Count; j++)
            {
                MiniItem miniItem = Items[j];
                Item itemById = TShock.Utils.GetItemById(miniItem.NetID);
                itemById.stack = miniItem.Stack;
                itemById.prefix = miniItem.Prefix;
                if (miniItem.Slot >= 0 && miniItem.Slot <= 58)
                {
                    player.TPlayer.inventory[miniItem.Slot] = itemById;
                }
                else if (miniItem.Slot >= 59 && miniItem.Slot <= 78)
                {
                    player.TPlayer.armor[miniItem.Slot - 59] = itemById;
                }
                else if (miniItem.Slot >= 79 && miniItem.Slot <= 88)
                {
                    player.TPlayer.dye[miniItem.Slot - 79] = itemById;
                }
                else if (miniItem.Slot >= 89 && miniItem.Slot <= 93)
                {
                    player.TPlayer.miscEquips[miniItem.Slot - 89] = itemById;
                }
                else if (miniItem.Slot >= 94 && miniItem.Slot <= 98)
                {
                    player.TPlayer.miscDyes[miniItem.Slot - 94] = itemById;
                }
                else if (miniItem.Slot >= 99 && miniItem.Slot <= 138)
                {
                    player.TPlayer.bank.item[miniItem.Slot - 99] = itemById;
                }
                else if (miniItem.Slot >= 139 && miniItem.Slot <= 178)
                {
                    player.TPlayer.bank2.item[miniItem.Slot - 139] = itemById;
                }
                else if (miniItem.Slot >= 180 && miniItem.Slot <= 219)
                {
                    player.TPlayer.bank3.item[miniItem.Slot - 180] = itemById;
                }
                else if (miniItem.Slot >= 220 && miniItem.Slot <= 259)
                {
                    player.TPlayer.bank4.item[miniItem.Slot - 220] = itemById;
                }
                else
                {
                    player.TPlayer.trashItem = itemById;
                }
            }
            float num9 = 0f;
            for (int k = 0; k < NetItem.InventorySlots; k++)
            {
                NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].inventory[k].Name), player.Index, num9, (float)(int)Main.player[player.Index].inventory[k].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int l = 0; l < NetItem.ArmorSlots; l++)
            {
                NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].armor[l].Name), player.Index, num9, (float)(int)Main.player[player.Index].armor[l].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int m = 0; m < NetItem.DyeSlots; m++)
            {
                NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].dye[m].Name), player.Index, num9, (float)(int)Main.player[player.Index].dye[m].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int n = 0; n < NetItem.MiscEquipSlots; n++)
            {
                NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].miscEquips[n].Name), player.Index, num9, (float)(int)Main.player[player.Index].miscEquips[n].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int num10 = 0; num10 < NetItem.MiscDyeSlots; num10++)
            {
                NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].miscDyes[num10].Name), player.Index, num9, (float)(int)Main.player[player.Index].miscDyes[num10].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int num11 = 0; num11 < NetItem.PiggySlots; num11++)
            {
                NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank.item[num11].Name), player.Index, num9, (float)(int)Main.player[player.Index].bank.item[num11].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int num12 = 0; num12 < NetItem.SafeSlots; num12++)
            {
                NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank2.item[num12].Name), player.Index, num9, (float)(int)Main.player[player.Index].bank2.item[num12].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            int num13 = -1;
            int num14 = -1;
            NetworkText val = NetworkText.FromLiteral(Main.player[player.Index].trashItem.Name);
            int index = player.Index;
            NetMessage.SendData(5, num13, num14, val, index, num9++, (float)(int)Main.player[player.Index].trashItem.prefix, 0f, 0, 0, 0);
            for (int num15 = 0; num15 < NetItem.ForgeSlots; num15++)
            {
                NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank3.item[num15].Name), player.Index, num9, (float)(int)Main.player[player.Index].bank3.item[num15].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            NetMessage.SendData(4, -1, -1, NetworkText.FromLiteral(player.Name), player.Index, 0f, 0f, 0f, 0, 0, 0);
            NetMessage.SendData(42, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
            NetMessage.SendData(16, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
            num9 = 0f;
            for (int num16 = 0; num16 < NetItem.InventorySlots; num16++)
            {
                NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].inventory[num16].Name), player.Index, num9, (float)(int)Main.player[player.Index].inventory[num16].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int num17 = 0; num17 < NetItem.ArmorSlots; num17++)
            {
                NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].armor[num17].Name), player.Index, num9, (float)(int)Main.player[player.Index].armor[num17].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int num18 = 0; num18 < NetItem.DyeSlots; num18++)
            {
                NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].dye[num18].Name), player.Index, num9, (float)(int)Main.player[player.Index].dye[num18].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int num19 = 0; num19 < NetItem.MiscEquipSlots; num19++)
            {
                NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].miscEquips[num19].Name), player.Index, num9, (float)(int)Main.player[player.Index].miscEquips[num19].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int num20 = 0; num20 < NetItem.MiscDyeSlots; num20++)
            {
                NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].miscDyes[num20].Name), player.Index, num9, (float)(int)Main.player[player.Index].miscDyes[num20].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int num21 = 0; num21 < NetItem.PiggySlots; num21++)
            {
                NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].bank.item[num21].Name), player.Index, num9, (float)(int)Main.player[player.Index].bank.item[num21].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            for (int num22 = 0; num22 < NetItem.SafeSlots; num22++)
            {
                NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].bank2.item[num22].Name), player.Index, num9, (float)(int)Main.player[player.Index].bank2.item[num22].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            int index2 = player.Index;
            int num23 = -1;
            NetworkText val2 = NetworkText.FromLiteral(Main.player[player.Index].trashItem.Name);
            int index3 = player.Index;
            NetMessage.SendData(5, index2, num23, val2, index3, num9++, (float)(int)Main.player[player.Index].trashItem.prefix, 0f, 0, 0, 0);
            for (int num24 = 0; num24 < NetItem.ForgeSlots; num24++)
            {
                NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].bank3.item[num24].Name), player.Index, num9, (float)(int)Main.player[player.Index].bank3.item[num24].prefix, 0f, 0, 0, 0);
                num9 += 1f;
            }
            NetMessage.SendData(4, player.Index, -1, NetworkText.FromLiteral(player.Name), player.Index, 0f, 0f, 0f, 0, 0, 0);
            NetMessage.SendData(42, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
            NetMessage.SendData(16, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
            for (int num25 = 0; num25 < 22; num25++)
            {
                player.TPlayer.buffType[num25] = 0;
            }
            NetMessage.SendData(50, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
            NetMessage.SendData(50, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
            NetMessage.SendData(76, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
            NetMessage.SendData(76, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
            NetMessage.SendData(39, player.Index, -1, NetworkText.Empty, 400, 0f, 0f, 0f, 0, 0, 0);
        }

        public void CopyFromPlayer(TSPlayer plr)
        {
            if (plr != null)
            {
                CopyFromPlayer(plr.TPlayer);
            }
        }

        public void CopyFromPlayer(Player plr)
        {
            if (plr == null)
            {
                return;
            }
            MaxHP = plr.statLifeMax;
            HP = plr.statLife;
            Mana = plr.statMana;
            MaxMana = plr.statManaMax;
            ExtraSlots = plr.extraAccessorySlots;
            EyeColor = plr.eyeColor;
            Hair = plr.hair;
            HairColor = plr.hairColor;
            HairDye = plr.hairDye;
            HappyFunTorchTime = plr.happyFunTorchTime.GetHashCode();
            HideVisuals = plr.hideVisibleAccessory;
            PantsColor = plr.pantsColor;
            ShoeColor = plr.shoeColor;
            SkinColor = plr.skinColor;
            ShirtColor = plr.shirtColor;
            SkinVariant = plr.skinVariant;
            SpawnX = plr.SpawnX;
            SpawnY = plr.SpawnY;
            UnderShirtColor = plr.underShirtColor;
            UnlockedBiomeTorches = plr.unlockedBiomeTorches.GetHashCode();
            UsingBiomeTorches = plr.UsingBiomeTorches.GetHashCode();
            Exists = true;
            for (int i = 0; i < 59; i++)
            {
                Item val = plr.inventory[i];
                if (val.netID != 0)
                {
                    MiniItem item = new MiniItem(i, val.prefix, val.netID, val.stack);
                    Items.Add(item);
                }
            }
            for (int j = 0; j < NetItem.ArmorSlots; j++)
            {
                Item val2 = plr.armor[j];
                if (val2.netID != 0)
                {
                    MiniItem item2 = new MiniItem(j + 59, val2.prefix, val2.netID, val2.stack);
                    Items.Add(item2);
                }
            }
            for (int k = 0; k < NetItem.DyeSlots; k++)
            {
                Item val3 = plr.dye[k];
                if (val3.netID != 0)
                {
                    MiniItem item3 = new MiniItem(k + 79, val3.prefix, val3.netID, val3.stack);
                    Items.Add(item3);
                }
            }
            for (int l = 0; l < NetItem.MiscEquipSlots; l++)
            {
                Item val4 = plr.miscEquips[l];
                if (val4.netID != 0)
                {
                    MiniItem item4 = new MiniItem(l + 89, val4.prefix, val4.netID, val4.stack);
                    Items.Add(item4);
                }
            }
            for (int m = 0; m < NetItem.MiscDyeSlots; m++)
            {
                Item val5 = plr.miscDyes[m];
                if (val5.netID != 0)
                {
                    MiniItem item5 = new MiniItem(m + 94, val5.prefix, val5.netID, val5.stack);
                    Items.Add(item5);
                }
            }
            for (int n = 0; n < NetItem.PiggySlots; n++)
            {
                Item val6 = plr.bank.item[n];
                if (val6.netID != 0)
                {
                    MiniItem item6 = new MiniItem(n + 99, val6.prefix, val6.netID, val6.stack);
                    Items.Add(item6);
                }
            }
            for (int num = 0; num < NetItem.SafeSlots; num++)
            {
                Item val7 = plr.bank2.item[num];
                if (val7.netID != 0)
                {
                    MiniItem item7 = new MiniItem(num + 139, val7.prefix, val7.netID, val7.stack);
                    Items.Add(item7);
                }
            }
            for (int num2 = 0; num2 < NetItem.ForgeSlots; num2++)
            {
                Item val8 = plr.bank3.item[num2];
                if (val8.netID != 0)
                {
                    MiniItem item8 = new MiniItem(num2 + 180, val8.prefix, val8.netID, val8.stack);
                    Items.Add(item8);
                }
            }
            for (int num3 = 0; num3 < NetItem.VoidSlots; num3++)
            {
                Item val9 = plr.bank4.item[num3];
                if (val9.netID != 0)
                {
                    MiniItem item9 = new MiniItem(num3 + 220, val9.prefix, val9.netID, val9.stack);
                    Items.Add(item9);
                }
            }
            Items.Add(new MiniItem(179, plr.trashItem.prefix, plr.trashItem.netID, plr.trashItem.stack));
        }

        public void RestoreCharacter(MiniPlayer plr)
        {
            if (plr != null)
            {
                RestoreCharacter(plr.Player);
            }
        }

        public PlayerData TransToPlayerData()
        {
            return new PlayerData(new TSPlayer(255))
            {
                exists = Exists,
                extraSlot = ExtraSlots,
                eyeColor = EyeColor,
                hair = Hair,
                hairColor = HairColor,
                hairDye = HairDye,
                happyFunTorchTime = HappyFunTorchTime,
                health = HP,
                hideVisuals = HideVisuals,
                inventory = ItemsToOriginInv(),
                mana = Mana,
                maxMana = MaxMana,
                pantsColor = PantsColor,
                questsCompleted = QuestsCompleted,
                shirtColor = ShirtColor,
                shoeColor = ShoeColor,
                skinColor = SkinColor,
                skinVariant = SkinVariant,
                spawnX = SpawnX,
                spawnY = SpawnY,
                underShirtColor = UnderShirtColor,
                unlockedBiomeTorches = UnlockedBiomeTorches,
                usingBiomeTorches = UsingBiomeTorches
            };
        }

        public NetItem[] ItemsToOriginInv()
        {
            NetItem[] array = (NetItem[])(object)new NetItem[NetItem.MaxInventory];
            for (int i = 0; i < Items.Count; i++)
            {
                MiniItem miniItem = Items[i];
                if (miniItem.Slot == i)
                {
                    array[i] = miniItem.ToNetItem();
                }
            }
            return array;
        }

        public void SaveDBWhenLeave(TSPlayer plr)
        {
            TShock.CharacterDB.InsertSpecificPlayerData(plr, TransToPlayerData());
        }
    }
}
