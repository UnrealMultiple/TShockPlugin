using Terraria;

namespace BossLock;

public class Lock
{
    public int LockSeconds = 3600;
    public int NpcId = 50;
    public string Name => Lang.GetNPCNameValue(this.NpcId);
}