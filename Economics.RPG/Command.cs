using EconomicsAPI.Attributes;
using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace Economics.RPG;

internal class Command
{
    [CommandMap("rank", "economics.rpg.rank")]
    public void Rank(CommandArgs args)
    {
        var level = RPG.PlayerLevelManager.GetLevel(args.Player.Name);
        if (level.RankLevels.Count > 1)
        {
            if (args.Parameters.Count > 0)
            {
                var ranklevel = level.RankLevels.Find(x => x.Name == args.Parameters[0]);
                if (ranklevel == null)
                {
                    args.Player.SendErrorMessage($"等级 {args.Parameters[0]} 不存在！ ");
                    return;
                }
                if (!args.Player.InProgress(ranklevel.Limit))
                {
                    args.Player.SendErrorMessage($"必须满足进度限制:{string.Join(",", ranklevel.Limit)}");
                    return;
                }
                if (EconomicsAPI.Economics.CurrencyManager.DelUserCurrency(args.Player.Name, ranklevel.Cost))
                {
                    args.Player.SendSuccessMessage($"成功升级至 {ranklevel.Name}!");
                    TShock.Utils.Broadcast(string.Format(ranklevel.RankBroadcast, args.Player.Name, ranklevel.Name), Color.Green);
                    RPG.PlayerLevelManager.Update(args.Player.Name, ranklevel);
                    args.Player.ExecCommand(ranklevel.RankCommands);
                    args.Player.GiveItems(ranklevel.RewardGoods);
                }
                else
                {
                    args.Player.SendErrorMessage($"升级所需 {ranklevel.Cost}，你当前{EconomicsAPI.Economics.Setting.CurrencyName}仅有{EconomicsAPI.Economics.CurrencyManager.GetUserCurrency(args.Player.Name)}!");
                }
            }
            else
            {
                args.Player.SendInfoMessage("请选择一个升级:");
                foreach (var info in level.RankLevels)
                {
                    args.Player.SendInfoMessage($"/rank {info.Name}({info.Cost})");
                }
            }
        }
        else if (level.RankLevels.Count == 1)
        {
            var ranklevel = level.RankLevels[0];
            if (!args.Player.InProgress(ranklevel.Limit))
            {
                args.Player.SendErrorMessage($"必须满足进度限制:{string.Join(",", ranklevel.Limit)}");
                return;
            }
            if (EconomicsAPI.Economics.CurrencyManager.DelUserCurrency(args.Player.Name, ranklevel.Cost))
            {
                args.Player.SendSuccessMessage($"成功升级至 {ranklevel.Name}!");
                TShock.Utils.Broadcast(string.Format(ranklevel.RankBroadcast, args.Player.Name, ranklevel.Name), Color.Green);
                RPG.PlayerLevelManager.Update(args.Player.Name, ranklevel);
                args.Player.ExecCommand(ranklevel.RankCommands);
                args.Player.GiveItems(ranklevel.RewardGoods);
            }
            else
            {
                args.Player.SendErrorMessage($"升级所需 {ranklevel.Cost}，你当前{EconomicsAPI.Economics.Setting.CurrencyName}仅有{EconomicsAPI.Economics.CurrencyManager.GetUserCurrency(args.Player.Name)}!");
            }
        }
        else
        {
            args.Player.SendErrorMessage("已经满级了，无法继续升级!");
        }
    }

    [CommandMap("重置等级", "economics.rpg.reset")]
    public void ResetLevel(CommandArgs args)
    {
        RPG.PlayerLevelManager.ResetPlayerLevel(args.Player.Name);
        args.Player.SendSuccessMessage("您已成功重置等级!");
        foreach (var cmd in RPG.Config.ResetCommand)
        {
            args.Player.ExecCommand(cmd);
        }
        TShock.Utils.Broadcast(string.Format(RPG.Config.ResetBroadcast, args.Player.Name), Color.Green);
        if (RPG.Config.ResetKick)
            args.Player.Disconnect("你因重置等级被踢出!");           
    }
}
