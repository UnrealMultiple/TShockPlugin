using LazyAPI.Database;
using LazyAPI.Utility;
using LinqToDB;
using LinqToDB.Mapping;
using TShockAPI;
using TShockAPI.DB;

namespace DeltaForce.Core.Database;

[Table("deltaforce_inventory")]
public class PlayerInventory : PlayerRecordBase<PlayerInventory>
{
    [Column("account")]
    public int AccountID { get; set; }

    [Column("inventory")]
    public string Inventory { get; set; } = string.Empty;

    [Column("health")]
    public int Health { get; set; } = 500;

    [Column("max_health")]
    public int MaxHealth { get; set; } = 500;

    [Column("mana")]
    public int Mana { get; set; } = 100;

    [Column("max_mana")]
    public int MaxMana { get; set; } = 100;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [Column("spawnX")]
    public int SpawnX { get; set; } = -1;

    [Column("spawnY")]
    public int SpawnY { get; set; } = -1;

    [Column("skinVariant")]
    public int SkinVariant { get; set; }

    [Column("hair")]
    public int Hair {  get; set; }

    [Column("hairDye")]
    public int HairDye { get; set; }

    [Column("hairColor")]
    public int? HairColor { get; set; }

    [Column("extraSlot")]
    public int? ExtraSlot { get; set; }

    [Column("pantsColor")]
    public int? PantsColor { get; set; }

    [Column("shirtColor")]
    public int? ShirtColor { get; set; }

    [Column("underShirtColor")]
    public int? UnderShirtColor { get; set; }

    [Column("shoeColor")]
    public int? ShoeColor { get; set; }

    [Column("hideVisuals")]
    public int? HideVisuals { get; set; }

    [Column("skinColor")]
    public int? SkinColor { get; set; }

    [Column("eyeColor")]
    public int? EyeColor { get; set; }

    [Column("questsCompleted")]
    public int QuestsCompleted { get; set; }

    [Column("usingBiomeTorches")]
    public int UsingBiomeTorches { get; set; }

    [Column("happyFunTorchTime")]
    public int HappyFunTorchTime { get; set; }

    [Column("unlockedBiomeTorches")]
    public int UnlockedBiomeTorches { get; set; }

    [Column("currentLoadoutIndex")]
    public int CurrentLoadoutIndex { get; set; }

    [Column("ateArtisanBread")]
    public int AteArtisanBread { get; set; }

    [Column("usedAegisCrystal")]
    public int UsedAegisCrystal { get; set; }

    [Column("usedAegisFruit")]
    public int UsedAegisFruit { get; set; }

    [Column("usedArcaneCrystal")]
    public int UsedArcaneCrystal { get; set; }

    [Column("usedGalaxyPearl")]
    public int UsedGalaxyPearl { get; set; }

    [Column("usedGummyWorm")]
    public int UsedGummyWorm { get; set; }

    [Column("usedAmbrosia")]
    public int UsedAmbrosia { get; set; }

    [Column("unlockedSuperCart")]
    public int UnlockedSuperCart { get; set; }

    [Column("enabledSuperCart")]
    public int EnabledSuperCart { get; set; }

    [Column("deathsPVE")]
    public int DeathsPVE { get; set; }

    [Column("deathsPVP")]
    public int DeathsPVP { get; set; }

    [Column("voiceVariant")]
    public int VoiceVariant { get; set; }

    [Column("voicePitchOffset")]
    public float VoicePitchOffset { get; set; }

    [Column("team")]
    public int Team { get; set; }

    public static PlayerInventory? GetPlayerInventory(string playerName)
    {
        try
        {
            using var context = Db.PlayerContext<PlayerInventory>();
            return context.Get(playerName).FirstOrDefault();
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲背包] 获取玩家 {playerName} 数据时发生错误: {ex}"));
            return null;
        }
    }

    public static void SavePlayerData(TSPlayer player)
    {
        try
        {
            using var context = Db.PlayerContext<PlayerInventory>();
            var existing = context.Get(player.Account.Name).FirstOrDefault();

            player.PlayerData.CopyCharacter(player);

            if (existing == null)
            {
                var data = CreatePlayerInventoryFromData(player.Account.ID, player.PlayerData);
                data.Name = player.Account.Name;
                context.Insert(data);
            }
            else
            {
                UpdatePlayerInventoryFromData(existing, player.PlayerData);
                context.Update(existing);
            }
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲背包] 保存玩家 {player.Name} 数据时发生错误: {ex}"));
            throw;
        }
    }

    public static void InsertSpecificPlayerData(TSPlayer player, PlayerData playerData)
    {
        try
        {
            using var context = Db.PlayerContext<PlayerInventory>();
            var existing = context.Get(player.Account.Name).FirstOrDefault();

            if (existing == null)
            {
                var data = CreatePlayerInventoryFromData(player.Account.ID, playerData);
                data.Name = player.Account.Name;
                context.Insert(data);
            }
            else
            {
                UpdatePlayerInventoryFromData(existing, playerData);
                context.Update(existing);
            }
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲背包] 保存指定玩家数据时发生错误: {ex}"));
            throw;
        }
    }

    public static void SeedInitialData(UserAccount account)
    {
        try
        {
            using var context = Db.PlayerContext<PlayerInventory>();
            var existing = context.Get(account.Name).FirstOrDefault();

            if (existing != null)
            {
                return;
            }

            var items = new List<NetItem>();
            items.AddRange(new NetItem[NetItem.MaxInventory]);

            var data = new PlayerInventory
            {
                Name = account.Name,
                AccountID = account.ID,
                Inventory = string.Join("~", items),
                Health = TShock.ServerSideCharacterConfig.Settings.StartingHealth,
                MaxHealth = TShock.ServerSideCharacterConfig.Settings.StartingHealth,
                Mana = TShock.ServerSideCharacterConfig.Settings.StartingMana,
                MaxMana = TShock.ServerSideCharacterConfig.Settings.StartingMana,
                UpdatedAt = DateTime.Now,
                SpawnX = -1,
                SpawnY = -1,
                QuestsCompleted = 0
            };

            context.Insert(data);
            TShock.Log.ConsoleInfo(GetString($"[三角洲背包] 已初始化玩家 {account.Name} 的空背包数据"));
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲背包] 初始化玩家数据时发生错误: {ex}"));
            throw;
        }
    }

    public static void RemovePlayer(int userid)
    {
        try
        {
            using var context = Db.PlayerContext<PlayerInventory>();
            var account = TShock.UserAccounts.GetUserAccountByID(userid);
            if (account == null)
            {
                return;
            }

            var existing = context.Get(account.Name).FirstOrDefault();
            if (existing != null)
            {
                context.Delete(existing);
                TShock.Log.ConsoleInfo(GetString($"[三角洲背包] 已删除玩家 {account.Name} 的背包数据"));
            }
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲背包] 删除玩家数据时发生错误: {ex}"));
            throw;
        }
    }

    public static void UpdatePlayerAppearance(UserAccount account, TSPlayer player)
    {
        try
        {
            using var context = Db.PlayerContext<PlayerInventory>();
            var existing = context.Get(account.Name).FirstOrDefault();

            if (existing == null)
            {
                return;
            }

            existing.SkinVariant = player.TPlayer.skinVariant;
            existing.Hair = player.TPlayer.hair;
            existing.HairDye = player.TPlayer.hairDye;
            existing.HairColor = TShock.Utils.EncodeColor(player.TPlayer.hairColor);
            existing.PantsColor = TShock.Utils.EncodeColor(player.TPlayer.pantsColor);
            existing.ShirtColor = TShock.Utils.EncodeColor(player.TPlayer.shirtColor);
            existing.UnderShirtColor = TShock.Utils.EncodeColor(player.TPlayer.underShirtColor);
            existing.ShoeColor = TShock.Utils.EncodeColor(player.TPlayer.shoeColor);
            existing.HideVisuals = TShock.Utils.EncodeBoolArray(player.TPlayer.hideVisibleAccessory);
            existing.SkinColor = TShock.Utils.EncodeColor(player.TPlayer.skinColor);
            existing.EyeColor = TShock.Utils.EncodeColor(player.TPlayer.eyeColor);
            existing.VoiceVariant = player.TPlayer.voiceVariant;
            existing.VoicePitchOffset = player.TPlayer.voicePitchOffset;
            existing.Team = player.TPlayer.team;
            existing.UpdatedAt = DateTime.Now;

            context.Update(existing);
            TShock.Log.ConsoleInfo(GetString($"[三角洲背包] 已更新玩家 {account.Name} 的外观数据"));
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲背包] 更新玩家外观时发生错误: {ex}"));
            throw;
        }
    }

    public static PlayerData LoadPlayerData(TSPlayer player)
    {
        try
        {
            using var context = Db.PlayerContext<PlayerInventory>();
            var savedData = context.Get(player.Name).FirstOrDefault();

            if (savedData == null)
            {
                return new PlayerData(false);
            }

            return CreatePlayerDataFromInventory(savedData);
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲背包] 加载玩家 {player.Name} 数据时发生错误: {ex}"));
            return new PlayerData(false);
        }
    }

    public static PlayerData CreateEmptyPlayerData()
    {
        return new PlayerData(false)
        {
            exists = true,
            health = TShock.ServerSideCharacterConfig.Settings.StartingHealth,
            maxHealth = TShock.ServerSideCharacterConfig.Settings.StartingHealth,
            mana = TShock.ServerSideCharacterConfig.Settings.StartingMana,
            maxMana = TShock.ServerSideCharacterConfig.Settings.StartingMana
        };
    }

    private static PlayerInventory CreatePlayerInventoryFromData(int accountId, PlayerData playerData)
    {
        return new PlayerInventory
        {
            AccountID = accountId,
            Inventory = string.Join("~", playerData.inventory),
            Health = playerData.health,
            MaxHealth = playerData.maxHealth,
            Mana = playerData.mana,
            MaxMana = playerData.maxMana,
            UpdatedAt = DateTime.Now,
            SpawnX = playerData.spawnX,
            SpawnY = playerData.spawnY,
            SkinVariant = playerData.skinVariant ?? 0,
            Hair = playerData.hair ?? 0,
            HairDye = playerData.hairDye,
            HairColor = TShock.Utils.EncodeColor(playerData.hairColor),
            PantsColor = TShock.Utils.EncodeColor(playerData.pantsColor),
            ShirtColor = TShock.Utils.EncodeColor(playerData.shirtColor),
            UnderShirtColor = TShock.Utils.EncodeColor(playerData.underShirtColor),
            ShoeColor = TShock.Utils.EncodeColor(playerData.shoeColor),
            HideVisuals = TShock.Utils.EncodeBoolArray(playerData.hideVisuals),
            SkinColor = TShock.Utils.EncodeColor(playerData.skinColor),
            EyeColor = TShock.Utils.EncodeColor(playerData.eyeColor),
            QuestsCompleted = playerData.questsCompleted,
            UsingBiomeTorches = playerData.usingBiomeTorches,
            HappyFunTorchTime = playerData.happyFunTorchTime,
            UnlockedBiomeTorches = playerData.unlockedBiomeTorches,
            CurrentLoadoutIndex = playerData.currentLoadoutIndex,
            AteArtisanBread = playerData.ateArtisanBread,
            UsedAegisCrystal = playerData.usedAegisCrystal,
            UsedAegisFruit = playerData.usedAegisFruit,
            UsedArcaneCrystal = playerData.usedArcaneCrystal,
            UsedGalaxyPearl = playerData.usedGalaxyPearl,
            UsedGummyWorm = playerData.usedGummyWorm,
            UsedAmbrosia = playerData.usedAmbrosia,
            UnlockedSuperCart = playerData.unlockedSuperCart,
            EnabledSuperCart = playerData.enabledSuperCart,
            DeathsPVE = playerData.deathsPVE,
            DeathsPVP = playerData.deathsPVP,
            VoiceVariant = playerData.voiceVariant ?? 0,
            VoicePitchOffset = playerData.voicePitchOffset ?? 0,
            Team = playerData.team
        };
    }

    private static void UpdatePlayerInventoryFromData(PlayerInventory existing, PlayerData playerData)
    {
        existing.Inventory = string.Join("~", playerData.inventory);
        existing.Health = playerData.health;
        existing.MaxHealth = playerData.maxHealth;
        existing.Mana = playerData.mana;
        existing.MaxMana = playerData.maxMana;
        existing.UpdatedAt = DateTime.Now;
        existing.SpawnX = playerData.spawnX;
        existing.SpawnY = playerData.spawnY;
        existing.SkinVariant = playerData.skinVariant ?? 0;
        existing.Hair = playerData.hair ?? 0;
        existing.HairDye = playerData.hairDye;
        existing.HairColor = TShock.Utils.EncodeColor(playerData.hairColor);
        existing.PantsColor = TShock.Utils.EncodeColor(playerData.pantsColor);
        existing.ShirtColor = TShock.Utils.EncodeColor(playerData.shirtColor);
        existing.UnderShirtColor = TShock.Utils.EncodeColor(playerData.underShirtColor);
        existing.ShoeColor = TShock.Utils.EncodeColor(playerData.shoeColor);
        existing.HideVisuals = TShock.Utils.EncodeBoolArray(playerData.hideVisuals);
        existing.SkinColor = TShock.Utils.EncodeColor(playerData.skinColor);
        existing.EyeColor = TShock.Utils.EncodeColor(playerData.eyeColor);
        existing.QuestsCompleted = playerData.questsCompleted;
        existing.UsingBiomeTorches = playerData.usingBiomeTorches;
        existing.HappyFunTorchTime = playerData.happyFunTorchTime;
        existing.UnlockedBiomeTorches = playerData.unlockedBiomeTorches;
        existing.CurrentLoadoutIndex = playerData.currentLoadoutIndex;
        existing.AteArtisanBread = playerData.ateArtisanBread;
        existing.UsedAegisCrystal = playerData.usedAegisCrystal;
        existing.UsedAegisFruit = playerData.usedAegisFruit;
        existing.UsedArcaneCrystal = playerData.usedArcaneCrystal;
        existing.UsedGalaxyPearl = playerData.usedGalaxyPearl;
        existing.UsedGummyWorm = playerData.usedGummyWorm;
        existing.UsedAmbrosia = playerData.usedAmbrosia;
        existing.UnlockedSuperCart = playerData.unlockedSuperCart;
        existing.EnabledSuperCart = playerData.enabledSuperCart;
        existing.DeathsPVE = playerData.deathsPVE;
        existing.DeathsPVP = playerData.deathsPVP;
        existing.VoiceVariant = playerData.voiceVariant ?? 0;
        existing.VoicePitchOffset = playerData.voicePitchOffset ?? 0;
        existing.Team = playerData.team;
    }

    private static PlayerData CreatePlayerDataFromInventory(PlayerInventory savedData)
    {
        var playerData = new PlayerData(false)
        {
            exists = true,
            health = savedData.Health,
            maxHealth = savedData.MaxHealth,
            mana = savedData.Mana,
            maxMana = savedData.MaxMana,
        };

        if (!string.IsNullOrEmpty(savedData.Inventory))
        {
            var inventory = savedData.Inventory.Split('~').Select(NetItem.Parse).ToList();
            if (inventory.Count < NetItem.MaxInventory)
            {
                inventory.InsertRange(67, new NetItem[2]);
                inventory.InsertRange(77, new NetItem[2]);
                inventory.InsertRange(87, new NetItem[2]);
                inventory.AddRange(new NetItem[NetItem.MaxInventory - inventory.Count]);
            }
            playerData.inventory = [.. inventory];
        }

        playerData.extraSlot = savedData.ExtraSlot ?? 0;
        playerData.spawnX = savedData.SpawnX == 0 ? -1 : savedData.SpawnX;
        playerData.spawnY = savedData.SpawnY == 0 ? -1 : savedData.SpawnY;
        playerData.skinVariant = savedData.SkinVariant;
        playerData.hair = savedData.Hair;
        playerData.hairDye = (byte)savedData.HairDye;
        playerData.hairColor = TShock.Utils.DecodeColor(savedData.HairColor);
        playerData.pantsColor = TShock.Utils.DecodeColor(savedData.PantsColor);
        playerData.shirtColor = TShock.Utils.DecodeColor(savedData.ShirtColor);
        playerData.underShirtColor = TShock.Utils.DecodeColor(savedData.UnderShirtColor);
        playerData.shoeColor = TShock.Utils.DecodeColor(savedData.ShoeColor);
        playerData.hideVisuals = TShock.Utils.DecodeBoolArray(savedData.HideVisuals);
        playerData.skinColor = TShock.Utils.DecodeColor(savedData.SkinColor);
        playerData.eyeColor = TShock.Utils.DecodeColor(savedData.EyeColor);
        playerData.questsCompleted = savedData.QuestsCompleted;
        playerData.usingBiomeTorches = savedData.UsingBiomeTorches;
        playerData.happyFunTorchTime = savedData.HappyFunTorchTime;
        playerData.unlockedBiomeTorches = savedData.UnlockedBiomeTorches;
        playerData.currentLoadoutIndex = savedData.CurrentLoadoutIndex;
        playerData.ateArtisanBread = savedData.AteArtisanBread;
        playerData.usedAegisCrystal = savedData.UsedAegisCrystal;
        playerData.usedAegisFruit = savedData.UsedAegisFruit;
        playerData.usedArcaneCrystal = savedData.UsedArcaneCrystal;
        playerData.usedGalaxyPearl = savedData.UsedGalaxyPearl;
        playerData.usedGummyWorm = savedData.UsedGummyWorm;
        playerData.usedAmbrosia = savedData.UsedAmbrosia;
        playerData.unlockedSuperCart = savedData.UnlockedSuperCart;
        playerData.enabledSuperCart = savedData.EnabledSuperCart;
        playerData.deathsPVE = savedData.DeathsPVE;
        playerData.deathsPVP = savedData.DeathsPVP;
        playerData.voiceVariant = savedData.VoiceVariant;
        playerData.voicePitchOffset = savedData.VoicePitchOffset;
        playerData.team = savedData.Team;

        return playerData;
    }
}
