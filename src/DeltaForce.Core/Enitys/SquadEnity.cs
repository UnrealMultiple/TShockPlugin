using DeltaForce.Core.Enums;
using IL.Terraria;
using TShockAPI;

namespace DeltaForce.Core.Enitys;

public struct SquadEnity()
{
    public HashSet<TSPlayer> Members = [];

    public TeamType Team = TeamType.None;
}
