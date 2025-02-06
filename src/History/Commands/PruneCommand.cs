using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace History.Commands;

public class PruneCommand : HCommand
{
    private readonly int time;

    public PruneCommand(int time, TSPlayer sender)
        : base(sender)
    {
        this.time = time;
    }

    public override void Execute()
    {
        var time = (int) (DateTime.UtcNow - History.Date).TotalSeconds - this.time;
        History.Database.Query("DELETE FROM History WHERE Time < @0 AND WorldID = @1", time, Main.worldID);
        History.Actions.RemoveAll(a => a.time < time);
        this.sender.SendSuccessMessage(GetString("历史记录已被删除."));
    }
}