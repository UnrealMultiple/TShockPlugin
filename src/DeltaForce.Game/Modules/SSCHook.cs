using LazyAPI.Utility;
using MonoMod.RuntimeDetour;
using System.Reflection;
using TShockAPI;
using TShockAPI.DB;

namespace DeltaForce.Game.Modules;

public class SSCHook
{
    private static readonly List<Hook> _hooks = [];
    private static bool _isInitialized = false;

    public SSCHook()
    {
    }

    public static void Initialize()
    {
        if (_isInitialized) return;

        try
        {
            var characterManagerType = typeof(CharacterManager);

            var getPlayerDataMethod = characterManagerType.GetMethod("GetPlayerData", BindingFlags.Instance | BindingFlags.Public);
            var insertPlayerDataMethod = characterManagerType.GetMethod("InsertPlayerData", BindingFlags.Instance | BindingFlags.Public);
            var seedInitialDataMethod = characterManagerType.GetMethod("SeedInitialData", BindingFlags.Instance | BindingFlags.Public);
            var isSeededAppearanceMissingMethod = characterManagerType.GetMethod("IsSeededAppearanceMissing", BindingFlags.Instance | BindingFlags.Public);
            var syncSeededAppearanceMethod = characterManagerType.GetMethod("SyncSeededAppearance", BindingFlags.Instance | BindingFlags.Public);
            var removePlayerMethod = characterManagerType.GetMethod("RemovePlayer", BindingFlags.Instance | BindingFlags.Public);
            var insertSpecificPlayerDataMethod = characterManagerType.GetMethod("InsertSpecificPlayerData", BindingFlags.Instance | BindingFlags.Public);

            if (getPlayerDataMethod != null)
            {
                _hooks.Add(new Hook(getPlayerDataMethod, OnGetPlayerData));
                TShock.Log.ConsoleInfo(GetString("[三角洲SSC] 已Hook CharacterManager.GetPlayerData"));
            }

            if (insertPlayerDataMethod != null)
            {
                _hooks.Add(new Hook(insertPlayerDataMethod, OnInsertPlayerData));
                TShock.Log.ConsoleInfo(GetString("[三角洲SSC] 已Hook CharacterManager.InsertPlayerData"));
            }

            if (seedInitialDataMethod != null)
            {
                _hooks.Add(new Hook(seedInitialDataMethod, OnSeedInitialData));
                TShock.Log.ConsoleInfo(GetString("[三角洲SSC] 已Hook CharacterManager.SeedInitialData"));
            }

            if (isSeededAppearanceMissingMethod != null)
            {
                _hooks.Add(new Hook(isSeededAppearanceMissingMethod, OnIsSeededAppearanceMissing));
                TShock.Log.ConsoleInfo(GetString("[三角洲SSC] 已Hook CharacterManager.IsSeededAppearanceMissing"));
            }

            if (syncSeededAppearanceMethod != null)
            {
                _hooks.Add(new Hook(syncSeededAppearanceMethod, OnSyncSeededAppearance));
                TShock.Log.ConsoleInfo(GetString("[三角洲SSC] 已Hook CharacterManager.SyncSeededAppearance"));
            }

            if (removePlayerMethod != null)
            {
                _hooks.Add(new Hook(removePlayerMethod, OnRemovePlayer));
                TShock.Log.ConsoleInfo(GetString("[三角洲SSC] 已Hook CharacterManager.RemovePlayer"));
            }

            if (insertSpecificPlayerDataMethod != null)
            {
                _hooks.Add(new Hook(insertSpecificPlayerDataMethod, OnInsertSpecificPlayerData));
                TShock.Log.ConsoleInfo(GetString("[三角洲SSC] 已Hook CharacterManager.InsertSpecificPlayerData"));
            }

            _isInitialized = true;
            TShock.Log.ConsoleInfo(GetString("[三角洲SSC] 所有Hook已初始化完成"));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"[三角洲SSC] Hook初始化失败: {ex.Message}"));
            TShock.Log.ConsoleError(GetString($"[三角洲SSC] 堆栈: {ex.StackTrace}"));
        }
    }

    public void Dispose()
    {
        foreach (var hook in _hooks)
        {
            hook.Dispose();
        }
        _hooks.Clear();
        _isInitialized = false;
        TShock.Log.ConsoleInfo(GetString("[三角洲SSC] 所有Hook已卸载"));
    }

    private static bool OnIsSeededAppearanceMissing(Func<CharacterManager, PlayerData, bool> orig, CharacterManager self, PlayerData data)
    {
        if (data == null || !data.exists)
            return false;

        return data.skinVariant == null
            && data.hair == null
            && data.hairColor == null
            && data.pantsColor == null
            && data.shirtColor == null
            && data.underShirtColor == null
            && data.shoeColor == null
            && data.skinColor == null
            && data.eyeColor == null
            && data.hideVisuals == null
            && data.voiceVariant == null
            && data.voicePitchOffset == null;
    }

    private static bool OnSyncSeededAppearance(Func<CharacterManager, UserAccount, TSPlayer, bool> orig, CharacterManager self, UserAccount account, TSPlayer player)
    {
        return true;
    }

    private static bool OnSeedInitialData(Func<CharacterManager, UserAccount, bool> orig, CharacterManager self, UserAccount account)
    {
        return true;
    }

    private static bool OnRemovePlayer(Func<CharacterManager, int, bool> orig, CharacterManager self, int userid)
    {
        return true;
    }

    private static bool OnInsertSpecificPlayerData(Func<CharacterManager, TSPlayer, PlayerData, bool> orig, CharacterManager self, TSPlayer player, PlayerData data)
    {
        if (!player.IsLoggedIn)
            return false;

        if (player.HasPermission(Permissions.bypassssc))
        {
            return true;
        }

        return true;
    }

    private static bool OnInsertPlayerData(Func<CharacterManager, TSPlayer, bool, bool> orig, CharacterManager self, TSPlayer player, bool fromCommand)
    {
        if (!player.IsLoggedIn)
            return false;

        if (player.State < (int)ConnectionState.Complete)
            return false;

        if (player.HasPermission(Permissions.bypassssc) && !fromCommand)
        {
            return false;
        }

        return true;
    }

    private static PlayerData OnGetPlayerData(Func<CharacterManager, TSPlayer, int, PlayerData> orig, CharacterManager self, TSPlayer player, int accid)
    {
        try
        {
            if (player == null || string.IsNullOrEmpty(player.Name))
            {
                return new PlayerData(false);
            }

            var inventoryData = RequestInventoryFromCore(player).GetAwaiter().GetResult();

            if (inventoryData != null)
            {
                TShock.Log.ConsoleInfo(GetString($"[三角洲SSC] 玩家 {player.Name} 数据已从Core服务器加载"));
                return inventoryData;
            }
            TShock.Log.ConsoleWarn(GetString($"[三角洲SSC] 无法从Core服务器获取玩家 {player.Name} 数据，返回空数据"));
            return CreateEmptyPlayerData();
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲SSC] 加载玩家数据失败: {ex}"));
            return CreateEmptyPlayerData();
        }
    }

    private static async Task<PlayerData?> RequestInventoryFromCore(TSPlayer player)
    {
        try
        {
            
            var inventoryData = await CommunicationManager.GetPlayerInventory(player);
            if (inventoryData == null)
            {
                return null;
            }

            var playerData = new PlayerData(false)
            {
                exists = true,
                health = inventoryData.Health,
                maxHealth = inventoryData.MaxHealth,
                mana = inventoryData.Mana,
                maxMana = inventoryData.MaxMana,
                spawnX = inventoryData.SpawnX,
                spawnY = inventoryData.SpawnY,
                skinVariant = inventoryData.SkinVariant,
                hair = inventoryData.Hair,
                hairDye = (byte)inventoryData.HairDye,
                hairColor = inventoryData.HairColor.HasValue ? TShock.Utils.DecodeColor(inventoryData.HairColor.Value) : Microsoft.Xna.Framework.Color.White,
                pantsColor = inventoryData.PantsColor.HasValue ? TShock.Utils.DecodeColor(inventoryData.PantsColor.Value) : Microsoft.Xna.Framework.Color.White,
                shirtColor = inventoryData.ShirtColor.HasValue ? TShock.Utils.DecodeColor(inventoryData.ShirtColor.Value) : Microsoft.Xna.Framework.Color.White,
                underShirtColor = inventoryData.UnderShirtColor.HasValue ? TShock.Utils.DecodeColor(inventoryData.UnderShirtColor.Value) : Microsoft.Xna.Framework.Color.White,
                shoeColor = inventoryData.ShoeColor.HasValue ? TShock.Utils.DecodeColor(inventoryData.ShoeColor.Value) : Microsoft.Xna.Framework.Color.White,
                skinColor = inventoryData.SkinColor.HasValue ? TShock.Utils.DecodeColor(inventoryData.SkinColor.Value) : Microsoft.Xna.Framework.Color.White,
                eyeColor = inventoryData.EyeColor.HasValue ? TShock.Utils.DecodeColor(inventoryData.EyeColor.Value) : Microsoft.Xna.Framework.Color.White,
                hideVisuals = inventoryData.HideVisuals.HasValue ? TShock.Utils.DecodeBoolArray(inventoryData.HideVisuals.Value) : new bool[10],
                questsCompleted = inventoryData.QuestsCompleted,
                usingBiomeTorches = inventoryData.UsingBiomeTorches,
                happyFunTorchTime = inventoryData.HappyFunTorchTime,
                unlockedBiomeTorches = inventoryData.UnlockedBiomeTorches,
                currentLoadoutIndex = inventoryData.CurrentLoadoutIndex,
                ateArtisanBread = inventoryData.AteArtisanBread,
                usedAegisCrystal = inventoryData.UsedAegisCrystal,
                usedAegisFruit = inventoryData.UsedAegisFruit,
                usedArcaneCrystal = inventoryData.UsedArcaneCrystal,
                usedGalaxyPearl = inventoryData.UsedGalaxyPearl,
                usedGummyWorm = inventoryData.UsedGummyWorm,
                usedAmbrosia = inventoryData.UsedAmbrosia,
                unlockedSuperCart = inventoryData.UnlockedSuperCart,
                enabledSuperCart = inventoryData.EnabledSuperCart,
                deathsPVE = inventoryData.DeathsPVE,
                deathsPVP = inventoryData.DeathsPVP,
                voiceVariant = inventoryData.VoiceVariant,
                voicePitchOffset = inventoryData.VoicePitchOffset,
                team = inventoryData.Team
            };
            if (!string.IsNullOrEmpty(inventoryData.Inventory))
            {
                var items = inventoryData.Inventory.Split('~').Select(NetItem.Parse).ToList();
                if (items.Count < NetItem.MaxInventory)
                {
                    items.InsertRange(67, new NetItem[2]);
                    items.InsertRange(77, new NetItem[2]);
                    items.InsertRange(87, new NetItem[2]);
                    items.AddRange(new NetItem[NetItem.MaxInventory - items.Count]);
                }
                playerData.inventory = [.. items];
            }

            return playerData;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲SSC] 从Core请求背包数据失败: {ex}"));
            return null;
        }
    }

    private static PlayerData CreateEmptyPlayerData()
    {
        var playerData = new PlayerData(false)
        {
            exists = true,
            health = 500,
            maxHealth = 500,
            mana = 200,
            maxMana = 200
        };
        return playerData;
    }
}
