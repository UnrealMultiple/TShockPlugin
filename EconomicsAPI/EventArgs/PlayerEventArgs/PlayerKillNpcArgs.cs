using Terraria;
using TShockAPI;

namespace EconomicsAPI.EventArgs.PlayerEventArgs;

public class PlayerKillNpcArgs : BasePlayerEventArgs
{
    public NPC Npc { get; }

    public int Damage { get; }

    internal PlayerKillNpcArgs(Player player, NPC npc, int damage)
    {
        Player = TShock.Players[player.whoAmI];
        Npc = npc;
        Damage = damage;
    }
}
