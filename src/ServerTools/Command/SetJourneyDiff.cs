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
        if (!Main._currentGameModeInfo.IsJourneyMode)
        {
            args.Player.SendErrorMessage(GetString("必须在旅途模式下才能设置难度！"));
            return;
        }
        if (SetJourneyDiffHelps(diff))
        {
            args.Player.SendSuccessMessage(GetString("难度成功设置为 {0}!"), args.Parameters[0]);
        }
        else
        {
            args.Player.SendErrorMessage(GetString("正确语法: /旅途难度 <难度模式>"));
            args.Player.SendErrorMessage(GetString("有效的难度模式: master，journey，normal，expert"));
        }
    }

    public static bool SetJourneyDiffHelps(string diffName)
    {
        float diff;
        switch (diffName.ToLower())
        {
            case "master":
                diff = 1f;
                break;
            case "journey":
                diff = 0f;
                break;
            case "normal":
                diff = 0.33f;
                break;
            case "expert":
                diff = 0.66f;
                break;
            default:
                return false;
        }
        var power = CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
        power._sliderCurrentValueCache = diff;
        power.UpdateInfoFromSliderValueCache();
        power.OnPlayerJoining(0);
        return true;
    }
}
