using Terraria;

namespace MiniGamesAPI
{
    public class MiniNpc
    {
        private NPC realNpc;

        public NPC RealNpc => realNpc;

        public bool Boss => realNpc.boss;

        public bool Friendly => realNpc.friendly;

        public bool TownNpc => realNpc.townNPC;

        public List<MiniItem> Items { get; set; }

        public MiniNpc(NPC npc)
        {
            realNpc = npc;
            Items = new List<MiniItem>();
        }

        public void StoreSlot(int netid, int stack, byte prefix, int slot, bool sendData = false)
        {
        }
    }
}
