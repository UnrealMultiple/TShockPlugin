using LazyAPI.Attributes;
using Terraria;
using Terraria.GameContent.Creative;
using TShockAPI;

namespace ServerTools.Command;

[Command("journeydiff", "旅途难度")]
[Permissions("servertool.set.journey")]
public class SetJourneyDiff
{
    [Main]
    public static void SetDiff(CommandArgs args, string diff)
    {
        if (!Main.IsJourneyMode)
        {
            args.Player.SendErrorMessage(GetString("必须在旅途模式下才能设置难度！"));
            return;
        }
        if (Utils.SetJourneyDiff(diff))
        {
            args.Player.SendSuccessMessage(GetString("难度成功设置为 {0}!"), args.Parameters[0]);
        }
        else
        {
            args.Player.SendErrorMessage(GetString("正确语法: /旅途难度 <难度模式>"));
            args.Player.SendErrorMessage(GetString("有效的难度模式: master，journey，normal，expert"));
        }
    }
}
