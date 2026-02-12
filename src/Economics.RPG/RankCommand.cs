using Economics.Core.Attributes;
using Economics.Core.Command;
using Economics.Core.Extensions;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace Economics.RPG;

    
public class RankCommand : BaseCommand
{
    public override string[] Alias => ["rank", "升级"];

    public override List<string> Permissions => ["economics.rpg.rank"];

    public override string HelpText => GetString("提示RPG等级!");

    public override string ErrorText => GetString("语法错误，请输入/rank升级!");

    public override void Invoke(CommandArgs args)
    {
        if (!args.Player.IsLoggedIn)
        {
            args.Player.SendErrorMessage(GetString("你必须登陆才能使用此命令!"));
            return;
        }
        var level = RPG.PlayerLevelManager.GetLevel(args.Player.Name);
        if (level.RankLevels.Count > 1)
        {
            if (args.Parameters.Count > 0)
            {
                var ranklevel = level.RankLevels.Find(x => x.Name == args.Parameters[0]);

                if (ranklevel == null)
                {
                    args.Player.SendErrorMessage(GetString($"等级 {args.Parameters[0]} 不存在！ "));
                    return;
                }
                if (ranklevel.SoleOccupation && RPG.PlayerLevelManager.HasLevel(ranklevel.Name))
                {
                    args.Player.SendErrorMessage(GetString("此职业全服唯一，已经存在此职业无法升级。"));
                    return;
                }
                if (ranklevel.SelectedWeapon.Count > 0 && !ranklevel.SelectedWeapon.Contains(args.Player.SelectedItem.type))
                {
                    args.Player.SendErrorMessage(GetString($"升级至 {args.Parameters[0]} 需要手持武器{string.Join(",", ranklevel.SelectedWeapon.Select(i => TShock.Utils.GetItemById(i).Name))}！ "));
                    return;
                }
                if (!args.Player.InProgress(ranklevel.Limit))
                {
                    args.Player.SendErrorMessage(GetString($"必须满足进度限制:{string.Join(",", ranklevel.Limit)}"));
                    return;
                }
                if (Core.Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, ranklevel.RedemptionRelationshipsOption))
                {
                    args.Player.SendSuccessMessage(GetString($"成功升级至 {ranklevel.Name}!"));
                    TShock.Utils.Broadcast(string.Format(ranklevel.RankBroadcast, args.Player.Name, ranklevel.Name), Color.Green);
                    RPG.PlayerLevelManager.Update(args.Player.Name, ranklevel);
                    args.Player.ExecCommand(ranklevel.RankCommands);
                    args.Player.GiveItems(ranklevel.RewardGoods);
                }
                else
                {
                    args.Player.SendErrorMessage(GetString($"升级所需 ({string.Join(" ", ranklevel.RedemptionRelationshipsOption.Select(x => $"{x.CurrencyType}x{x.Number}"))})"));
                }
            }
            else
            {
                args.Player.SendInfoMessage(GetString("请选择一个升级:"));
                foreach (var info in level.RankLevels)
                {
                    args.Player.SendInfoMessage(GetString($"/rank {info.Name}({string.Join(" ", info.RedemptionRelationshipsOption.Select(x => $"{x.CurrencyType}x{x.Number}"))})"));
                }
            }
        }
        else if (level.RankLevels.Count == 1)
        {
            var ranklevel = level.RankLevels[0];
            if (ranklevel.SelectedWeapon.Count > 0 && !ranklevel.SelectedWeapon.Contains(args.Player.SelectedItem.type))
            {
                args.Player.SendErrorMessage(GetString($"升级至 {ranklevel} 需要手持武器{string.Join(",", ranklevel.SelectedWeapon.Select(i => TShock.Utils.GetItemById(i).Name))}！ "));
                return;
            }
            if (!args.Player.InProgress(ranklevel.Limit))
            {
                args.Player.SendErrorMessage(GetString($"必须满足进度限制:{string.Join(",", ranklevel.Limit)}"));
                return;
            }
            if (Core.Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, ranklevel.RedemptionRelationshipsOption))
            {
                args.Player.SendSuccessMessage(GetString($"成功升级至 {ranklevel.Name}!"));
                TShock.Utils.Broadcast(string.Format(ranklevel.RankBroadcast, args.Player.Name, ranklevel.Name), Color.Green);
                RPG.PlayerLevelManager.Update(args.Player.Name, ranklevel);
                args.Player.ExecCommand(ranklevel.RankCommands);
                args.Player.GiveItems(ranklevel.RewardGoods);
            }
            else
            {
                args.Player.SendErrorMessage(GetString($"升级所需 {string.Join(" ", ranklevel.RedemptionRelationshipsOption.Select(x => $"{x.CurrencyType}x{x.Number}"))}"));
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("已经满级了，无法继续升级!"));
        }
    }
}
