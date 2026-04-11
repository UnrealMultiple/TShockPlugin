using DeltaForce.Core.Database;
using DeltaForce.Protocol.Packets;
using DeltaForce.Protocol.Processing;
using LinqToDB;
using TShockAPI;
using TShockAPI.DB;

namespace DeltaForce.Core.Handlers;

public class SaveInventoryHandler : RequestHandlerBase<SaveInventoryRequestPacket, SaveInventoryResponsePacket>
{
    public override SaveInventoryResponsePacket Handle(SaveInventoryRequestPacket request)
    {
        try
        {
            var account = TShock.UserAccounts.GetUserAccountByName(request.PlayerName);
            if (account == null)
            {
                return CreateFailureResponse(request, $"玩家 {request.PlayerName} 不存在");
            }

            var existing = PlayerInventory.GetPlayerInventory(request.PlayerName);

            var playerInventory = new PlayerInventory
            {
                Name = request.PlayerName,
                AccountID = account.ID,
                Inventory = request.Inventory,
                Health = request.Health,
                MaxHealth = request.MaxHealth,
                Mana = request.Mana,
                MaxMana = request.MaxMana,
                UpdatedAt = DateTime.Now,
                SpawnX = request.SpawnX,
                SpawnY = request.SpawnY,
                SkinVariant = request.SkinVariant,
                Hair = request.Hair,
                HairDye = request.HairDye,
                HairColor = request.HairColor,
                ExtraSlot = request.ExtraSlot,
                PantsColor = request.PantsColor,
                ShirtColor = request.ShirtColor,
                UnderShirtColor = request.UnderShirtColor,
                ShoeColor = request.ShoeColor,
                HideVisuals = request.HideVisuals,
                SkinColor = request.SkinColor,
                EyeColor = request.EyeColor,
                QuestsCompleted = request.QuestsCompleted,
                UsingBiomeTorches = request.UsingBiomeTorches,
                HappyFunTorchTime = request.HappyFunTorchTime,
                UnlockedBiomeTorches = request.UnlockedBiomeTorches,
                CurrentLoadoutIndex = request.CurrentLoadoutIndex,
                AteArtisanBread = request.AteArtisanBread,
                UsedAegisCrystal = request.UsedAegisCrystal,
                UsedAegisFruit = request.UsedAegisFruit,
                UsedArcaneCrystal = request.UsedArcaneCrystal,
                UsedGalaxyPearl = request.UsedGalaxyPearl,
                UsedGummyWorm = request.UsedGummyWorm,
                UsedAmbrosia = request.UsedAmbrosia,
                UnlockedSuperCart = request.UnlockedSuperCart,
                EnabledSuperCart = request.EnabledSuperCart,
                DeathsPVE = request.DeathsPVE,
                DeathsPVP = request.DeathsPVP,
                VoiceVariant = request.VoiceVariant,
                VoicePitchOffset = request.VoicePitchOffset,
                Team = request.Team
            };

            using var context = LazyAPI.Database.Db.PlayerContext<PlayerInventory>();

            if (existing == null)
            {
                context.Insert(playerInventory);
            }
            else
            {
                context.Update(playerInventory);
            }

            TShock.Log.ConsoleInfo($"[三角洲Core] 已保存玩家 {request.PlayerName} 的背包数据");

            var response = CreateSuccessResponse(request, "背包数据保存成功");
            response.PlayerName = request.PlayerName;
            response.SavedAt = DateTime.Now;
            return response;
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[三角洲Core] 保存玩家 {request.PlayerName} 背包数据失败: {ex}");
            return CreateFailureResponse(request, $"保存失败: {ex.Message}");
        }
    }
}
