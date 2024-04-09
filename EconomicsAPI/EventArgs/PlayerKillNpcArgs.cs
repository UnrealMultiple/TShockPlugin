using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace EconomicsAPI.EventArgs;

public class PlayerKillNpcArgs:System.EventArgs
{
    public TSPlayer Player { get; }

    public NPC Npc { get; }

    public int Damage { get; }

    public bool Handler { get; set; } = false;

    internal PlayerKillNpcArgs(Player player, NPC npc, int damage)
    {
        Player = TShock.Players[player.whoAmI];
        Npc = npc;
        Damage = damage;
    }
}
