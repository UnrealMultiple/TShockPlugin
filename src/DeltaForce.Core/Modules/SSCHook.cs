using DeltaForce.Core.Database;
using LazyAPI.Utility;
using MonoMod.RuntimeDetour;
using System.Reflection;
using TShockAPI;
using TShockAPI.DB;

namespace DeltaForce.Core.Modules;

public static class SSCHook
{
    private static readonly List<Hook> _hooks = [];

    public static void Initialize()
    {
        var get_player_data = typeof(CharacterManager).GetMethod("GetPlayerData", BindingFlags.Instance | BindingFlags.Public)!;
        var insert_player_data = typeof(CharacterManager).GetMethod("InsertPlayerData", BindingFlags.Instance | BindingFlags.Public)!;
        var seed_initial_data = typeof(CharacterManager).GetMethod("SeedInitialData", BindingFlags.Instance | BindingFlags.Public)!;
        var is_seeded_appearance_missing = typeof(CharacterManager).GetMethod("IsSeededAppearanceMissing", BindingFlags.Instance | BindingFlags.Public)!;
        var sync_seeded_appearance = typeof(CharacterManager).GetMethod("SyncSeededAppearance", BindingFlags.Instance | BindingFlags.Public)!;
        var remove_player = typeof(CharacterManager).GetMethod("RemovePlayer", BindingFlags.Instance | BindingFlags.Public)!;
        var insert_specific_player_data = typeof(CharacterManager).GetMethod("InsertSpecificPlayerData", BindingFlags.Instance | BindingFlags.Public)!;

        _hooks.Add(new Hook(get_player_data, OnGetPlayerData));
        _hooks.Add(new Hook(insert_player_data, OnInsertPlayerData));
        _hooks.Add(new Hook(seed_initial_data, OnSeedInitialData));
        _hooks.Add(new Hook(is_seeded_appearance_missing, OnIsSeededAppearanceMissing));
        _hooks.Add(new Hook(sync_seeded_appearance, OnSyncSeededAppearance));
        _hooks.Add(new Hook(remove_player, OnRemovePlayer));
        _hooks.Add(new Hook(insert_specific_player_data, OnInsertSpecificPlayerData));

        TShock.Log.ConsoleInfo(GetString("[三角洲SSC] 所有Hook已加载"));
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
        if (account == null || player == null)
            return false;

        try
        {
            PlayerInventory.UpdatePlayerAppearance(account, player);
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲SSC] 同步玩家外观失败: {ex}"));
            return false;
        }
    }

    private static bool OnSeedInitialData(Func<CharacterManager, UserAccount, bool> orig, CharacterManager self, UserAccount account)
    {
        try
        {
            PlayerInventory.SeedInitialData(account);
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲SSC] 初始化玩家数据失败: {ex}"));
            return false;
        }
    }

    private static bool OnRemovePlayer(Func<CharacterManager, int, bool> orig, CharacterManager self, int userid)
    {
        try
        {
            PlayerInventory.RemovePlayer(userid);
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲SSC] 删除玩家数据失败: {ex}"));
            return false;
        }
    }

    private static bool OnInsertSpecificPlayerData(Func<CharacterManager, TSPlayer, PlayerData, bool> orig, CharacterManager self, TSPlayer player, PlayerData data)
    {
        if (!player.IsLoggedIn)
            return false;

        try
        {
            PlayerInventory.InsertSpecificPlayerData(player, data);
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲SSC] 保存指定玩家数据失败: {ex}"));
            return false;
        }
    }

    private static bool OnInsertPlayerData(Func<CharacterManager, TSPlayer, bool, bool> orig, CharacterManager self, TSPlayer player, bool fromCommand)
    {
        if (!player.IsLoggedIn)
            return false;

        if (player.State < (int)ConnectionState.Complete)
            return false;

        try
        {
            PlayerInventory.SavePlayerData(player);
            return true;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲SSC] 保存玩家数据失败: {ex}"));
            return false;
        }
    }

    private static PlayerData OnGetPlayerData(Func<CharacterManager, TSPlayer, int, PlayerData> orig, CharacterManager self, TSPlayer player, int accid)
    {
        try
        {
            if (player == null || string.IsNullOrEmpty(player.Name))
            {
                return new PlayerData(false);
            }

            var data = PlayerInventory.LoadPlayerData(player);
            return data;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[三角洲SSC] 加载玩家数据失败: {ex}"));
            return new PlayerData(false);
        }
    }

    public static void Dispose()
    {
        foreach (var hook in _hooks)
        {
            hook.Dispose();
        }
        _hooks.Clear();
        TShock.Log.ConsoleInfo(GetString("[三角洲SSC] 所有Hook已卸载"));
    }
}
