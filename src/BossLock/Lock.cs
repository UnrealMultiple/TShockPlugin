using Terraria;

namespace BossLock;

public class Lock
{
    public int LockSeconds = 3600;
    [Newtonsoft.Json.JsonConverter(typeof(NpcIdConverter))]
    public int NpcId = Terraria.ID.NPCID.KingSlime;
    public string Name => Lang.GetNPCNameValue(this.NpcId);
}
