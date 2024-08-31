using Terraria;

namespace MiniGamesAPI;

public class MiniNpc
{
    public NPC RealNpc { get; }

    public bool Boss => this.RealNpc.boss;

    public bool Friendly => this.RealNpc.friendly;

    public bool TownNpc => this.RealNpc.townNPC;

    public List<MiniItem> Items { get; set; }

    public MiniNpc(NPC npc)
    {
        this.RealNpc = npc;
        this.Items = new List<MiniItem>();
    }

    public void StoreSlot(int netid, int stack, byte prefix, int slot, bool sendData = false)
    {
    }
}