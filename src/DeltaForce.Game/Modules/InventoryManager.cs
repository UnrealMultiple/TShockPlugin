using DeltaForce.Protocol.Packets;
using LazyAPI.Utility;
using TShockAPI;
using TShockAPI.DB;

namespace DeltaForce.Game.Modules;

public static class InventoryManager
{
    public static async Task<SaveInventoryResponsePacket?> SavePlayerInventoryAsync(TSPlayer player)
    {
        try
        {
            if (!player.IsLoggedIn || player.Account == null)
            {
                Console.WriteLine(GetString($"[InventoryManager] 玩家 {player.Name} 未登录，无法保存背包"));
                return null;
            }

            var playerData = player.PlayerData;
            if (!playerData.exists)
            {
                playerData.CopyCharacter(player);
            }

            var request = new SaveInventoryRequestPacket
            {
                PlayerName = player.Name,
                Inventory = string.Join("~", playerData.inventory),
                Health = playerData.health,
                MaxHealth = playerData.maxHealth,
                Mana = playerData.mana,
                MaxMana = playerData.maxMana,
                SpawnX = playerData.spawnX,
                SpawnY = playerData.spawnY,
                SkinVariant = playerData.skinVariant ?? 0,
                Hair = playerData.hair ?? 0,
                HairDye = playerData.hairDye,
                HairColor = TShock.Utils.EncodeColor(playerData.hairColor),
                ExtraSlot = playerData.extraSlot,
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

            var response = await Game.Client.RequestAsync<SaveInventoryRequestPacket, SaveInventoryResponsePacket>(request);

            if (response?.Success == true)
            {
                Console.WriteLine(GetString($"[InventoryManager] 玩家 {player.Name} 背包保存成功"));
            }
            else
            {
                Console.WriteLine(GetString($"[InventoryManager] 玩家 {player.Name} 背包保存失败: {response?.Message}"));
            }

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine(GetString($"[InventoryManager] 保存玩家 {player.Name} 背包时发生错误: {ex.Message}"));
            return null;
        }
    }

    public static SaveInventoryResponsePacket? SavePlayerInventory(TSPlayer player)
    {
        return SavePlayerInventoryAsync(player).GetAwaiter().GetResult();
    }

    public static async Task SaveAllPlayersInventoryAsync()
    {
        var activePlayers = TShock.Players.Where(p => p?.Active == true && p.IsLoggedIn).ToList();

        foreach (var player in activePlayers)
        {
            await SavePlayerInventoryAsync(player);
        }

        Console.WriteLine(GetString($"[InventoryManager] 已保存 {activePlayers.Count} 名玩家的背包数据"));
    }
}
