using Terraria;
using Terraria.ID;
namespace MusicPlayer;

internal class VirtualPerformer
{
    public byte Index;
    public short ItemType;

    internal static VirtualPerformer HarpPerformer = new(254, ItemID.Harp);
    internal static VirtualPerformer BellPerformer = new(253, ItemID.Bell);
    internal static VirtualPerformer TheAxePerformer = new(252, ItemID.TheAxe);
    public VirtualPerformer(byte index, short itemType)
    {
        Index = index;
        ItemType = itemType;
    }
    public static VirtualPerformer GetPerformer(string? name = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            return HarpPerformer;
        }
        return name.ToLowerInvariant() switch
        {
            "bell" => BellPerformer,
            "theaxe" => TheAxePerformer,
            _ => HarpPerformer,
        };
    }
    public void Create(int remoteClient)
    {
        NetMessageSender.PlayerActive(remoteClient, Index, true);
        NetMessageSender.SyncEquipment(remoteClient, Index, 0, 1, 0, ItemType);
        NetMessageSender.PlayerBuff(remoteClient, Index, BuffID.Invisibility, BuffID.WitchBroom);
    }
    public void Destroy(int remoteClient)
    {
        NetMessageSender.PlayerActive(remoteClient, Index, false);
        NetMessageSender.SyncEquipment(remoteClient, Index, 0, 0, 0, 0);
        NetMessageSender.PlayerBuff(remoteClient, Index);
    }
    public void PlayNote(int remoteClient, float note)
    {
        NetMessageSender.PlayerControls(remoteClient, Index, Main.player[remoteClient].position);
        NetMessageSender.InstrumentSound(remoteClient, Index, note);
    }
}