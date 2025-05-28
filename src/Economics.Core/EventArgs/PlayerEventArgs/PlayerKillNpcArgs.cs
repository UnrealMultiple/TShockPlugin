using Terraria;
using TShockAPI;

namespace Economics.Core.EventArgs.PlayerEventArgs;

public class PlayerKillNpcArgs : BasePlayerEventArgs
{
    public NPC Npc { get; }

    public float Damage { get; }

    internal PlayerKillNpcArgs(Player player, NPC npc, float damage)
    {
        this.Player = TShock.Players[player.whoAmI];
        this.Npc = npc;
        this.Damage = damage;
    }
}