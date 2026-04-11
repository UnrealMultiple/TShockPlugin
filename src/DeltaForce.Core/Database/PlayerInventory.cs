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
            existing.Under