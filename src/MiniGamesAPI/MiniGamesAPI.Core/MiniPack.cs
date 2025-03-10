using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace MiniGamesAPI;

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
        this.Name = name;
        this.ID = id;
        this.Hair = 0;
        this.SkinVariant = 0;
        this.ExtraSlots = 0;
        this.SpawnY = -1;
        this.SpawnX = -1;
        this.Exists = true;
        this.MaxMana = 20;
        this.Mana = 20;
        this.HP = 100;
        this.MaxHP = 100;
        this.HairDye = 0;
        this.HideVisuals = new bool[10];
        this.UnlockedBiomeTorches = 0;
        this.HappyFunTorchTime = 0;
        this.UsingBiomeTorches = 0;
        this.QuestsCompleted = 0;
        this.EyeColor = new Color(4283128425u);
        this.SkinColor = new Color(4284120575u);
        this.ShoeColor = new Color(4282149280u);
        this.UnderShirtColor = new Color(4292326560u);
        this.ShirtColor = new Color(4287407535u);
        this.HairColor = new Color(4287407535u);
        this.PantsColor = new Color(4287407535u);
        this.Items = new List<MiniItem>();
    }

    public MiniPack GetCopyNoItems(string name, int id)
    {
        return new MiniPack(name, id)
        {
            Exists = this.Exists,
            ExtraSlots = this.ExtraSlots,
            EyeColor = this.EyeColor,
            Hair = this.Hair,
            HairColor = this.HairColor,
            HairDye = this.HairDye,
            HappyFunTorchTime = this.HappyFunTorchTime,
            HideVisuals = this.HideVisuals,
            HP = this.HP,
            Items = new List<MiniItem>(),
            Mana = this.Mana,
            MaxHP = this.MaxHP,
            MaxMana = this.MaxMana,
            PantsColor = this.PantsColor,
            QuestsCompleted = this.QuestsCompleted,
            ShirtColor = this.ShirtColor,
            ShoeColor = this.ShoeColor,
            SkinColor = this.SkinColor,
            SkinVariant = this.SkinVariant,
            SpawnX = this.SpawnX,
            SpawnY = this.SpawnY,
            UnderShirtColor = this.UnderShirtColor,
            UnlockedBiomeTorches = this.UnlockedBiomeTorches,
            UsingBiomeTorches = this.UsingBiomeTorches
        };
    }

    public void RestoreCharacter(TSPlayer player)
    {
        player.IgnoreSSCPackets = true;
        player.TPlayer.statLife = this.HP;
        player.TPlayer.statLifeMax = this.MaxHP;
        player.TPlayer.statMana = this.Mana;
        player.TPlayer.statManaMax = this.MaxMana;
        player.TPlayer.SpawnX = this.SpawnX;
        player.TPlayer.SpawnY = this.SpawnY;
        player.initialServerSpawnX = this.SpawnX;
        player.initialServerSpawnY = this.SpawnY;
        player.TPlayer.hairDye = this.HairDye;
        player.TPlayer.anglerQuestsFinished = this.QuestsCompleted;
        if (this.ExtraSlots.HasValue)
        {
            player.TPlayer.extraAccessory = this.ExtraSlots.Value == 1;
        }
        if (this.SkinVariant.HasValue)
        {
            player.TPlayer.skinVariant = this.SkinVariant.Value;
        }
        if (this.Hair.HasValue)
        {
            player.TPlayer.hair = this.Hair.Value;
        }
        if (this.HairColor.HasValue)
        {
            player.TPlayer.hairColor = this.HairColor.Value;
        }
        if (this.PantsColor.HasValue)
        {
            player.TPlayer.pantsColor = this.PantsColor.Value;
        }
        if (this.ShirtColor.HasValue)
        {
            player.TPlayer.shirtColor = this.ShirtColor.Value;
        }
        if (this.UnderShirtColor.HasValue)
        {
            player.TPlayer.underShirtColor = this.UnderShirtColor.Value;
        }
        if (this.ShoeColor.HasValue)
        {
            player.TPlayer.shoeColor = this.ShoeColor.Value;
        }
        if (this.SkinColor.HasValue)
        {
            player.TPlayer.skinColor = this.SkinColor.Value;
        }
        if (this.EyeColor.HasValue)
        {
            player.TPlayer.eyeColor = this.EyeColor.Value;
        }
        player.TPlayer.hideVisibleAccessory = this.HideVisuals ?? (new bool[player.TPlayer.hideVisibleAccessory.Length]);
        for (var i = 0; i < NetItem.MaxInventory; i++)
        {
            if (i < 59)
            {
                player.TPlayer.inventory[i].netDefaults(0);
            }
            else if (i < 79)
            {
                var num = i - NetItem.ArmorIndex.Item1;
                player.TPlayer.armor[num].netDefaults(0);
            }
            else if (i < 89)
            {
                var num2 = i - NetItem.DyeIndex.Item1;
                player.TPlayer.dye[num2].netDefaults(0);
            }
            else if (i < 94)
            {
                var num3 = i - NetItem.MiscEquipIndex.Item1;
                player.TPlayer.miscEquips[num3].netDefaults(0);
            }
            else if (i < 99)
            {
                var num4 = i - NetItem.MiscDyeIndex.Item1;
                player.TPlayer.miscDyes[num4].netDefaults(0);
            }
            else if (i < 139)
            {
                var num5 = i - NetItem.PiggyIndex.Item1;
                player.TPlayer.bank.item[num5].netDefaults(0);
            }
            else if (i < 179)
            {
                var num6 = i - NetItem.SafeIndex.Item1;
                player.TPlayer.bank2.item[num6].netDefaults(0);
            }
            else if (i < 220)
            {
                if (i == 179)
                {
                    player.TPlayer.trashItem.netDefaults(0);
                    continue;
                }
                var num7 = i - NetItem.ForgeIndex.Item1;
                player.TPlayer.bank3.item[num7].netDefaults(0);
            }
            else
            {
                var num8 = i - NetItem.VoidIndex.Item1;
                player.TPlayer.bank4.item[num8].netDefaults(0);
            }
        }
        for (var j = 0; j < this.Items.Count; j++)
        {
            var miniItem = this.Items[j];
            var itemById = TShock.Utils.GetItemById(miniItem.NetID);
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
        var num9 = 0f;
        for (var k = 0; k < NetItem.InventorySlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].inventory[k].Name), player.Index, num9, Main.player[player.Index].inventory[k].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var l = 0; l < NetItem.ArmorSlots; l++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].armor[l].Name), player.Index, num9, Main.player[player.Index].armor[l].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var m = 0; m < NetItem.DyeSlots; m++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].dye[m].Name), player.Index, num9, Main.player[player.Index].dye[m].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var n = 0; n < NetItem.MiscEquipSlots; n++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].miscEquips[n].Name), player.Index, num9, Main.player[player.Index].miscEquips[n].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var num10 = 0; num10 < NetItem.MiscDyeSlots; num10++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].miscDyes[num10].Name), player.Index, num9, Main.player[player.Index].miscDyes[num10].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var num11 = 0; num11 < NetItem.PiggySlots; num11++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank.item[num11].Name), player.Index, num9, Main.player[player.Index].bank.item[num11].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var num12 = 0; num12 < NetItem.SafeSlots; num12++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank2.item[num12].Name), player.Index, num9, Main.player[player.Index].bank2.item[num12].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        var num13 = -1;
        var num14 = -1;
        var val = NetworkText.FromLiteral(Main.player[player.Index].trashItem.Name);
        var index = player.Index;
        NetMessage.SendData(5, num13, num14, val, index, num9++, Main.player[player.Index].trashItem.prefix, 0f, 0, 0, 0);
        for (var num15 = 0; num15 < NetItem.ForgeSlots; num15++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank3.item[num15].Name), player.Index, num9, Main.player[player.Index].bank3.item[num15].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        NetMessage.SendData(4, -1, -1, NetworkText.FromLiteral(player.Name), player.Index, 0f, 0f, 0f, 0, 0, 0);
        NetMessage.SendData(42, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
        NetMessage.SendData(16, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
        num9 = 0f;
        for (var num16 = 0; num16 < NetItem.InventorySlots; num16++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].inventory[num16].Name), player.Index, num9, Main.player[player.Index].inventory[num16].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var num17 = 0; num17 < NetItem.ArmorSlots; num17++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].armor[num17].Name), player.Index, num9, Main.player[player.Index].armor[num17].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var num18 = 0; num18 < NetItem.DyeSlots; num18++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].dye[num18].Name), player.Index, num9, Main.player[player.Index].dye[num18].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var num19 = 0; num19 < NetItem.MiscEquipSlots; num19++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].miscEquips[num19].Name), player.Index, num9, Main.player[player.Index].miscEquips[num19].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var num20 = 0; num20 < NetItem.MiscDyeSlots; num20++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].miscDyes[num20].Name), player.Index, num9, Main.player[player.Index].miscDyes[num20].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var num21 = 0; num21 < NetItem.PiggySlots; num21++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].bank.item[num21].Name), player.Index, num9, Main.player[player.Index].bank.item[num21].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        for (var num22 = 0; num22 < NetItem.SafeSlots; num22++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].bank2.item[num22].Name), player.Index, num9, Main.player[player.Index].bank2.item[num22].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        var index2 = player.Index;
        var num23 = -1;
        var val2 = NetworkText.FromLiteral(Main.player[player.Index].trashItem.Name);
        var index3 = player.Index;
        NetMessage.SendData(5, index2, num23, val2, index3, num9++, Main.player[player.Index].trashItem.prefix, 0f, 0, 0, 0);
        for (var num24 = 0; num24 < NetItem.ForgeSlots; num24++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].bank3.item[num24].Name), player.Index, num9, Main.player[player.Index].bank3.item[num24].prefix, 0f, 0, 0, 0);
            num9 += 1f;
        }
        NetMessage.SendData(4, player.Index, -1, NetworkText.FromLiteral(player.Name), player.Index, 0f, 0f, 0f, 0, 0, 0);
        NetMessage.SendData(42, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
        NetMessage.SendData(16, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
        for (var num25 = 0; num25 < 22; num25++)
        {
            player.TPlayer.buffType[num25] = 0;
        }
        NetMessage.SendData(50, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
        NetMessage.SendData(50, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
        NetMessage.SendData(76, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
        NetMessage.SendData(76, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0, 0, 0);
        NetMessage.SendData(39, player.Index, -1, NetworkText.Empty, 400, 0f, 0f, 0f, 0, 0, 0);
    }

    public void CopyFromPlayer(TSPlayer? plr)
    {
        if (plr != null)
        {
            this.CopyFromPlayer(plr.TPlayer);
        }
    }

    public void CopyFromPlayer(Player plr)
    {
        if (plr == null)
        {
            return;
        }
        this.MaxHP = plr.statLifeMax;
        this.HP = plr.statLife;
        this.Mana = plr.statMana;
        this.MaxMana = plr.statManaMax;
        this.ExtraSlots = plr.extraAccessorySlots;
        this.EyeColor = plr.eyeColor;
        this.Hair = plr.hair;
        this.HairColor = plr.hairColor;
        this.HairDye = plr.hairDye;
        this.HappyFunTorchTime = plr.happyFunTorchTime.GetHashCode();
        this.HideVisuals = plr.hideVisibleAccessory;
        this.PantsColor = plr.pantsColor;
        this.ShoeColor = plr.shoeColor;
        this.SkinColor = plr.skinColor;
        this.ShirtColor = plr.shirtColor;
        this.SkinVariant = plr.skinVariant;
        this.SpawnX = plr.SpawnX;
        this.SpawnY = plr.SpawnY;
        this.UnderShirtColor = plr.underShirtColor;
        this.UnlockedBiomeTorches = plr.unlockedBiomeTorches.GetHashCode();
        this.UsingBiomeTorches = plr.UsingBiomeTorches.GetHashCode();
        this.Exists = true;
        for (var i = 0; i < 59; i++)
        {
            var val = plr.inventory[i];
            if (val.netID != 0)
            {
                var item = new MiniItem(i, val.prefix, val.netID, val.stack);
                this.Items.Add(item);
            }
        }
        for (var j = 0; j < NetItem.ArmorSlots; j++)
        {
            var val2 = plr.armor[j];
            if (val2.netID != 0)
            {
                var item2 = new MiniItem(j + 59, val2.prefix, val2.netID, val2.stack);
                this.Items.Add(item2);
            }
        }
        for (var k = 0; k < NetItem.DyeSlots; k++)
        {
            var val3 = plr.dye[k];
            if (val3.netID != 0)
            {
                var item3 = new MiniItem(k + 79, val3.prefix, val3.netID, val3.stack);
                this.Items.Add(item3);
            }
        }
        for (var l = 0; l < NetItem.MiscEquipSlots; l++)
        {
            var val4 = plr.miscEquips[l];
            if (val4.netID != 0)
            {
                var item4 = new MiniItem(l + 89, val4.prefix, val4.netID, val4.stack);
                this.Items.Add(item4);
            }
        }
        for (var m = 0; m < NetItem.MiscDyeSlots; m++)
        {
            var val5 = plr.miscDyes[m];
            if (val5.netID != 0)
            {
                var item5 = new MiniItem(m + 94, val5.prefix, val5.netID, val5.stack);
                this.Items.Add(item5);
            }
        }
        for (var n = 0; n < NetItem.PiggySlots; n++)
        {
            var val6 = plr.bank.item[n];
            if (val6.netID != 0)
            {
                var item6 = new MiniItem(n + 99, val6.prefix, val6.netID, val6.stack);
                this.Items.Add(item6);
            }
        }
        for (var num = 0; num < NetItem.SafeSlots; num++)
        {
            var val7 = plr.bank2.item[num];
            if (val7.netID != 0)
            {
                var item7 = new MiniItem(num + 139, val7.prefix, val7.netID, val7.stack);
                this.Items.Add(item7);
            }
        }
        for (var num2 = 0; num2 < NetItem.ForgeSlots; num2++)
        {
            var val8 = plr.bank3.item[num2];
            if (val8.netID != 0)
            {
                var item8 = new MiniItem(num2 + 180, val8.prefix, val8.netID, val8.stack);
                this.Items.Add(item8);
            }
        }
        for (var num3 = 0; num3 < NetItem.VoidSlots; num3++)
        {
            var val9 = plr.bank4.item[num3];
            if (val9.netID != 0)
            {
                var item9 = new MiniItem(num3 + 220, val9.prefix, val9.netID, val9.stack);
                this.Items.Add(item9);
            }
        }
        this.Items.Add(new MiniItem(179, plr.trashItem.prefix, plr.trashItem.netID, plr.trashItem.stack));
    }

    public void RestoreCharacter(MiniPlayer plr)
    {
        if (plr != null)
        {
            this.RestoreCharacter(plr.Player);
        }
    }

public PlayerData TransToPlayerData()
{
    return new PlayerData(true)
    {
        exists = this.Exists,
        extraSlot = this.ExtraSlots,
        eyeColor = this.EyeColor,
        hair = this.Hair,
        hairColor = this.HairColor,
        hairDye = this.HairDye,
        happyFunTorchTime = this.HappyFunTorchTime,
        health = this.HP,
        hideVisuals = this.HideVisuals,
        inventory = this.ItemsToOriginInv(),
        mana = this.Mana,
        maxMana = this.MaxMana,
        pantsColor = this.PantsColor,
        questsCompleted = this.QuestsCompleted,
        shirtColor = this.ShirtColor,
        shoeColor = this.ShoeColor,
        skinColor = this.SkinColor,
        skinVariant = this.SkinVariant,
        spawnX = this.SpawnX,
        spawnY = this.SpawnY,
        underShirtColor = this.UnderShirtColor,
        unlockedBiomeTorches = this.UnlockedBiomeTorches,
        usingBiomeTorches = this.UsingBiomeTorches
    };
}

    public NetItem[] ItemsToOriginInv()
    {
        var array = (NetItem[]) (object) new NetItem[NetItem.MaxInventory];
        for (var i = 0; i < this.Items.Count; i++)
        {
            var miniItem = this.Items[i];
            if (miniItem.Slot == i)
            {
                array[i] = miniItem.ToNetItem();
            }
        }
        return array;
    }

    public void SaveDBWhenLeave(TSPlayer plr)
    {
        TShock.CharacterDB.InsertSpecificPlayerData(plr, this.TransToPlayerData());
    }
}