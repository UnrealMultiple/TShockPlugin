using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace History.Commands;

public class HistoryCommand : HCommand
{
    private readonly int x;
    private readonly int y;

    public HistoryCommand(int x, int y, TSPlayer sender)
        : base(sender)
    {
        this.x = x;
        this.y = y;
    }

    public override void Execute()
    {
        var actions = new List<Action>();

        using (var reader =
            History.Database.QueryReader("SELECT Account, Action, Data, Time FROM History WHERE XY = @0 AND WorldID = @1",
            (this.x << 16) + this.y, Main.worldID))
        {
            while (reader.Read())
            {
                actions.Add(new Action
                {
                    account = reader.Get<string>("Account"),
                    action = (byte) reader.Get<int>("Action"),
                    data = (ushort) reader.Get<int>("Data"),
                    time = reader.Get<int>("Time")
                });
            }
        }

        actions.AddRange(History.Actions.Where(a => a.x == this.x && a.y == this.y));
        this.sender.SendSuccessMessage(GetString("图格历史记录 ({0}, {1}):"), this.x, this.y);
        foreach (var a in actions)
        {
            this.sender.SendInfoMessage(a.ToString());
        }

        if (actions.Count == 0)
        {
            this.sender.SendErrorMessage(GetString("没有查询到这个图格的修改历史."));
        }
    }

    // 清空数据表
    public static bool ClearData()
    {
        return History.Database.Query("DELETE FROM History") != 0;
    }
}