using Terraria;
using TShockAPI;

namespace EconomicsAPI.EventArgs.PlayerEventArgs;

public class PlayerKillNpcArgs : BasePlayerEventArgs
{
    public NPC Npc { get; }

    public float Damage { get; }

    internal PlayerKillNpcArgs(Player player, NPC npc, float damage)
    {
        Player = TShock.Players[player.whoAmI];
        Npc = npc;
        Damage = damage;
    }
}
