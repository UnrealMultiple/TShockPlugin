using TShockAPI;

namespace History.Commands;

public class UndoCommand : HCommand
{
    public static List<Action> LastRollBack = null!;
    public static bool LastWasReenact = false;

    public UndoCommand(TSPlayer sender)
        : base(sender)
    {
    }

    public override void Execute()
    {
        //Redo saved actions
        if (LastWasReenact)
        {
            for (var i = LastRollBack.Count - 1; i >= 0; i--)
            {
                LastRollBack[i].Rollback();
            }
        }
        else
        {
            foreach (var action in LastRollBack)
            {
                action.Reenact();
            }
        }
        //Resave actions into database
        var undo = new SaveCommand(LastRollBack.ToArray());
        undo.Execute();

        this.sender.SendSuccessMessage(GetString("撤消完成! {0} 个操作被撤销."), LastRollBack.Count);
        LastRollBack = null!;
    }
}