using DeltaForce.Core.Modules;
using LazyAPI.Attributes;
using LazyAPI.Utility;
using TShockAPI;

namespace DeltaForce.Core.Commands;

[Command("match")]
[Permissions("deltaforce.match")]
public class SquadMatchCommand
{
    [Alias("start", "s")]
    public static void StartMatch(CommandArgs args)
    {
        SquadMatchManager.AddPlayerToSquad(args.Player);
        TShock.Utils.Broadcast(GetString($"[DeltaForce] {args.Player.Name} 加入了匹配队伍"), Microsoft.Xna.Framework.Color.Green);
    }

    [Alias("leave", "l")]
    public static void LeaveMatch(CommandArgs args)
    {
        SquadMatchManager.RemovePlayerFromSquad(args.Player);
        TShock.Utils.Broadcast(GetString($"[DeltaForce] {args.Player.Name} 离开了匹配队伍"), Microsoft.Xna.Framework.Color.Green);
    }
}
