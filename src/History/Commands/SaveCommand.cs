using Terraria;
using TShockAPI.DB;

namespace History.Commands;

public class SaveCommand : HCommand
{
    private readonly Action[] actions;

    public SaveCommand(Action[] actions)
    : base(null!)
    {
        this.actions = actions;
    }

    public override void Execute()
    {
        foreach (var a in this.actions)
        {
            History.Database.Query("INSERT INTO History(Time, Account, Action, XY, Data, Style, Paint, WorldID, Text, Alternate, Random, Direction) VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11)",
                a.time, a.account, a.action, (a.x << 16) + a.y, a.data, a.style, a.paint, Main.worldID, a.text, a.alt, a.random, a.direction ? 1 : -1);
        }
    }
}