using Terraria;
using Terraria.Utilities;
using TShockAPI;
using TShockAPI.DB;

namespace History.Commands;

public class RollbackCommand : HCommand
{
    private readonly string account;
    private readonly int radius;
    private readonly bool reenact;
    private readonly int time;

    public RollbackCommand(string account, int time, int radius, TSPlayer sender, bool reenact = false)
        : base(sender)
    {
        this.account = account;
        this.time = time;
        this.radius = radius;
        this.reenact = reenact;
    }

    public override void Execute()
    {
        var actions = new List<Action>();
        var rollbackTime = (int) (DateTime.UtcNow - History.Date).TotalSeconds - this.time;

        var plrX = this.sender.TileX;
        var plrY = this.sender.TileY;
        var lowX = plrX - this.radius;
        var highX = plrX + this.radius;
        var lowY = plrY - this.radius;
        var highY = plrY + this.radius;
        var XYReq = string.Format("XY / 65536 BETWEEN {0} AND {1} AND XY & 65535 BETWEEN {2} AND {3}", lowX, highX, lowY, highY);

        using (var reader =
            History.Database.QueryReader("SELECT * FROM History WHERE Account = @0 AND Time >= @1 AND " + XYReq + " AND WorldID = @2",
            this.account, rollbackTime, Main.worldID))
        {
            while (reader.Read())
            {
                actions.Add(new Action
                {
                    account = reader.Get<string>("Account"),
                    action = (byte) reader.Get<int>("Action"),
                    data = (ushort) reader.Get<int>("Data"),
                    style = (byte) reader.Get<int>("Style"),
                    paint = (short) reader.Get<int>("Paint"),
                    time = reader.Get<int>("Time"),
                    x = reader.Get<int>("XY") >> 16,
                    y = reader.Get<int>("XY") & 0xffff,
                    text = reader.Get<string>("Text"),
                    alt = reader.Get<int>("Alternate"),
                    random = reader.Get<int>("Random"),
                    direction = reader.Get<int>("Direction") == 1 ? true : false
                });
            }
        }
        if (!this.reenact)
        {
            History.Database.Query("DELETE FROM History WHERE Account = @0 AND Time >= @1 AND " + XYReq + " AND WorldID = @2",
                this.account, rollbackTime, Main.worldID);
        }
        Main.rand ??= new UnifiedRandom();
        /*
              if (WorldGen.genRand == null)
                  WorldGen.genRand = new Random();
        */
        for (var i = 0; i >= 0 && i < History.Actions.Count; i++)
        {
            var action = History.Actions[i];
            if (action.account == this.account && action.time >= rollbackTime &&
                lowX <= action.x && lowY <= action.y && action.x <= highX && action.y <= highY)
            {
                actions.Add(action);
                if (!this.reenact)
                {
                    History.Actions.RemoveAt(i);
                    i--;
                }
            }
        }
        if (!this.reenact)
        {
            for (var i = actions.Count - 1; i >= 0; i--)
            {
                actions[i].Rollback();
            }

            UndoCommand.LastWasReenact = false;
        }
        else
        {
            foreach (var action in actions)
            {
                action.Reenact();
            }

            UndoCommand.LastWasReenact = true;
        }
        UndoCommand.LastRollBack = actions;
        this.sender.SendInfoMessage(this.reenact
            ? GetString($"重现 {actions.Count} 个操作.")
            : GetString($"回溯 {actions.Count} 个操作."));
    }
}