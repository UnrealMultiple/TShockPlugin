using DeltaForce.Protocol.Enums;
using DeltaForce.Protocol.Interfaces;

namespace DeltaForce.Protocol.Packets;

public class SaveInventoryRequestPacket : IRequestPacket
{
    public PacketType PacketID => PacketType.SaveInventoryRequest;
    public Guid RequestId { get; set; } = Guid.NewGuid();

    public string PlayerName { get; set; } = string.Empty;

    public string Inventory { get; set; } = string.Empty;

    public int Health { get; set; }

    public int MaxHealth { get; set; }

    public int Mana { get; set; }

    public int MaxMana { get; set; }

    public int SpawnX { get; set; }

    public int SpawnY { get; set; }

    public int SkinVariant { get; set; }

    public int Hair { get; set; }

    public int HairDye { get; set; }

    public int? HairColor { get; set; }

    public int? ExtraSlot { get; set; }

    public int? PantsColor { get; set; }

    public int? ShirtColor { get; set; }

    public int? UnderShirtColor { get; set; }

    public int? ShoeColor { get; set; }

    public int? HideVisuals { get; set; }

    public int? SkinColor { get; set; }

    public int? EyeColor { get; set; }

    public int QuestsCompleted { get; set; }

    public int UsingBiomeTorches { get; set; }

    public int HappyFunTorchTime { get; set; }

    public int UnlockedBiomeTorches { get; set; }

    public int CurrentLoadoutIndex { get; set; }

    public int AteArtisanBread { get; set; }

    public int UsedAegisCrystal { get; set; }

    public int UsedAegisFruit { get; set; }

    public int UsedArcaneCrystal { get; set; }

    public int UsedGalaxyPearl { get; set; }

    public int UsedGummyWorm { get; set; }

    public int UsedAmbrosia { get; set; }

    public int UnlockedSuperCart { get; set; }

    public int EnabledSuperCart { get; set; }

    public int DeathsPVE { get; set; }

    public int DeathsPVP { get; set; }

    public int VoiceVariant { get; set; }

    public float VoicePitchOffset { get; set; }

    public int Team { get; set; }
}

public class SaveInventoryResponsePacket : IResponsePacket
{
    public PacketType PacketID => PacketType.SaveInventoryResponse;
    public Guid RequestId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
}
