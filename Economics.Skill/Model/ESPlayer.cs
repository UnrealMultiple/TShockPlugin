using Economics.RPG.Extensions;
using Terraria;
using TShockAPI;

namespace Economics.Skill.Model;

public class ESPlayer
{
    public TaskCompletionSource<NPC?> KillNpc { get; set; }

    public TaskCompletionSource<NPC?> StrikeNpc { get; set; }

    public TSPlayer TSPlayer { get; set; }

    public ESPlayer(TSPlayer player)
    {
        KillNpc = new TaskCompletionSource<NPC?>();
        StrikeNpc = new TaskCompletionSource<NPC?>();
        TSPlayer = player;
    }

    public ESPlayer(int index) : this(TShock.Players[index])
    {

    }

    public async Task<bool> IsKillNpc(TimeSpan timeout)
    {
        var npc = await KillNpc.Task.WaitAsync(timeout).RunCatch(e=>
        {
            return null;
        });
        if(npc != null)
        {
            KillNpc = new TaskCompletionSource<NPC?>();
            return true;
        }
        return false;
    }
}
