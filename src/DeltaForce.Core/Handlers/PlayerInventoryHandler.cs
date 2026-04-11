using DeltaForce.Core.Database;
using DeltaForce.Protocol.Packets;
using DeltaForce.Protocol.Processing;

namespace DeltaForce.Core.Handlers;

public class PlayerInventoryHandler : RequestHandlerBase<PlayerInventoryRequestPacket, PlayerInventoryResponsePacket>
{
    public override PlayerInventoryResponsePacket Handle(PlayerInventoryRequestPacket request)
    {
        var inventory = PlayerInventory.GetPlayerInventory(request.PlayerName);

        if (inventory == null)
        {
            return CreateFailureResponse(request, $"Player {request.PlayerName} not found");
        }

        var response = CreateSuccessResponse(request, "Success");
        response.PlayerName = request.PlayerName;
        response.AccountID = inventory.AccountID;
        response.Inventory = inventory.Inventory;
        response.Health = inventory.Health;
        response.MaxHealth = inventory.MaxHealth;
        response.Mana = inventory.Mana;
        response.MaxMana = inventory.MaxMana;
        response.UpdatedAt = inventory.UpdatedAt;
        response.SpawnX = inventory.SpawnX;
        response.SpawnY = inventory.SpawnY;
        response.SkinVariant = inventory.SkinVariant;
        response.Hair = inventory.Hair;
        response.HairDye = inventory.HairDye;
        response.HairColor = inventory.HairColor;
        response.ExtraSlot = inventory.ExtraSlot;
        response.PantsColor = inventory.PantsColor;
        response.ShirtColor = inventory.ShirtColor;
        response.UnderShirtColor = inventory.UnderShirtColor;
        response.ShoeColor = inventory.ShoeColor;
        response.HideVisuals = inventory.HideVisuals;
        response.SkinColor = inventory.SkinColor;
        response.EyeColor = inventory.EyeColor;
        response.QuestsCompleted = inventory.QuestsCompleted;
        response.UsingBiomeTorches = inventory.UsingBiomeTorches;
        response.HappyFunTorchTime = inventory.HappyFunTorchTime;
        response.UnlockedBiomeTorches = inventory.UnlockedBiomeTorches;
        response.CurrentLoadoutIndex = inventory.CurrentLoadoutIndex;
        response.AteArtisanBread = inventory.AteArtisanBread;
        response.UsedAegisCrystal = inventory.UsedAegisCrystal;
        response.UsedAegisFruit = inventory.UsedAegisFruit;
        response.UsedArcaneCrystal = inventory.UsedArcaneCrystal;
        response.UsedGalaxyPearl = inventory.UsedGalaxyPearl;
        response.UsedGummyWorm = inventory.UsedGummyWorm;
        response.UsedAmbrosia = inventory.UsedAmbrosia;
        response.UnlockedSuperCart = inventory.UnlockedSuperCart;
        response.EnabledSuperCart = inventory.EnabledSuperCart;
        response.DeathsPVE = inventory.DeathsPVE;
        response.DeathsPVP = inventory.DeathsPVP;
        response.VoiceVariant = inventory.VoiceVariant;
        response.VoicePitchOffset = inventory.VoicePitchOffset;
        response.Team = inventory.Team;

        return response;
    }
}
