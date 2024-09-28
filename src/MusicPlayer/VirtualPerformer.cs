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
        this.Index = index;
        this.ItemType = itemType;
    }
    public static VirtualPerformer GetPerformer(string? name = null)
    {
        return string.IsNullOrEmpty(name)
            ? HarpPerformer
            : name.ToLowerInvariant() switch
            {
                "bell" => BellPerformer,
                "theaxe" => TheAxePerformer,
                _ => HarpPerformer,
            };
    }
    public void Create(int remoteClient)
    {
        NetMessageSender.PlayerActive(remoteClient, this.Index, true);
        NetMessageSender.SyncEquipment(remoteClient, this.Index, 0, 1, 0, this.ItemType);
        NetMessageSender.PlayerBuff(remoteClient, this.Index, BuffID.Invisibility, BuffID.WitchBroom);
    }
    public void Destroy(int remoteClient)
    {
        NetMessageSender.PlayerActive(remoteClient, this.Index, false);
        NetMessageSender.SyncEquipment(remoteClient, this.Index, 0, 0, 0, 0);
        NetMessageSender.PlayerBuff(remoteClient, this.Index);
    }
    public void PlayNote(int remoteClient, float note)
    {
        NetMessageSender.PlayerControls(remoteClient, this.Index, Main.player[remoteClient].position);
        NetMessageSender.InstrumentSound(remoteClient, this.Index, note);
    }
}