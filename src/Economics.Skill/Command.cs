using Economics.Skill.Model;
using EconomicsAPI.Attributes;
using TShockAPI;

namespace Economics.Skill;

[RegisterSeries]
public class Command
{
    [CommandMap("skill", Permission.SkillUse)]
    public static void CSkill(CommandArgs args)
    {
        void Show(List<string> line)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out var pageNumber))
            {
                return;
            }

            PaginationTools.SendPage(
                    args.Player,
                    pageNumber,
                    line,
                    new PaginationTools.Settings
                    {
                        MaxLinesPerPage = Skill.Config.PageMax,
                        NothingToDisplayString = GetString("当前技能列表空空如也"),
                        HeaderFormat = GetString("技能列表 ({0}/{1})："),
                        FooterFormat = GetString("输入 {0}skill list {{0}} 查看更多").SFormat(Commands.Specifier)
                    }
                );
        }
        if (!args.Player.IsLoggedIn && args.Parameters.Count == 1 && args.Parameters[0].ToLower() != "reset")
        {
            args.Player.SendErrorMessage(GetString("你必须登陆游戏才能购买技能!"));
            return;
        }

        if (args.Parameters.Count >= 1 && args.Parameters[0].ToLower() == "list")
        {
            var line = new List<string>();
            for (var i = 0; i < Skill.Config.SkillContexts.Count; i++)
            {
                line.Add(GetString($"{i + 1}. {Skill.Config.SkillContexts[i].Name}  价格 {string.Join(" ", Skill.Config.SkillContexts[i].RedemptionRelationshipsOption.Select(x => $"{x.CurrencyType}x{x.Number}"))})"));
            }

            Show(line);
        }
        switch (args.Parameters.Count)
        {
            case 2:
            {
                if (!int.TryParse(args.Parameters[1], out var index))
                {
                    args.Player.SendErrorMessage(GetString("请输入一个正确的序号!"));
                    return;
                }
                if (args.Parameters[0].ToLower() == "buy")
                {
                    try
                    {
                        var skill = Utils.VerifyBindSkill(args.Player, index);
                        if (!EconomicsAPI.Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, skill.RedemptionRelationshipsOption))
                        {
                            args.Player.SendErrorMessage(GetString($"你的货币不足购买此技能!"));
                            return;
                        }
                        Skill.PlayerSKillManager.Add(args.Player.Name, args.Player.SelectedItem.netID, index);
                        args.Player.SendSuccessMessage(GetString("购买成功，技能已绑定!"));
                        return;
                    }
                    catch (Exception ex)
                    {
                        args.Player.SendErrorMessage(ex.Message);
                        return;
                    }
                }
                else if (args.Parameters[0].ToLower() == "del")
                {
                    if (!Skill.PlayerSKillManager.HasSkill(args.Player.Name, args.Player.SelectedItem.netID, index)
                        && !Skill.PlayerSKillManager.HasSkill(args.Player.Name, index))
                    {
                        args.Player.SendErrorMessage(GetString("你未绑定此技能，无需删除！"));
                        return;
                    }
                    Skill.PlayerSKillManager.Remove(args.Player.Name, index);
                    args.Player.SendSuccessMessage(GetString("技能移除成功!"));
                    return;
                }
                else if (args.Parameters[0].ToLower() == "clearh")
                {
                    var playerName = args.Parameters[1];
                    var skills = Skill.PlayerSKillManager.QuerySkill(playerName);
                    foreach (var skill in skills)
                    {
                        if (skill.Skill != null && skill.Skill.Hidden)
                        {
                            Skill.PlayerSKillManager.Remove(playerName, skill.ID);
                        }
                    }
                    args.Player.SendSuccessMessage(GetString("已清除目标玩家绑定的隐藏技能!"));
                }
                break;
            }
            case 1:
            {
                if (args.Parameters[0].ToLower() == "ms")
                {
                    var skills = Skill.PlayerSKillManager.QuerySkill(args.Player.Name);
                    if (!skills.Any())
                    {
                        args.Player.SendErrorMessage(GetString("你并未绑定技能!"));
                        return;
                    }
                    args.Player.SendSuccessMessage(GetString("查询成功!"));
                    foreach (var skill in skills)
                    {
                        if (skill.Skill != null)
                        {
                            args.Player.SendSuccessMessage(skill.Skill.SkillSpark.SparkMethod.Contains(Enumerates.SkillSparkType.Take)
                                ? GetString($"[{skill.ID}] 主动技能 [i:{skill.BindItem}] 绑定 {skill.Skill.Name}")
                                : GetString($"[{skill.ID}] 被动技能 {skill.Skill.Name}"));
                        }
                        else
                        {
                            args.Player.SendErrorMessage(GetString($"无法溯源的技能序号: {skill.ID}"));
                        }
                    }

                    return;
                }
                if (args.Parameters[0].ToLower() == "removeall")
                {
                    var skills = Skill.PlayerSKillManager.QuerySkillByItem(args.Player.Name, args.Player.SelectedItem.netID);
                    if (!skills.Any())
                    {
                        args.Player.SendErrorMessage(GetString("手持物品并未绑定技能!"));
                        return;
                    }
                    foreach (var skill in skills)
                    {
                        if (skill.Skill != null && !skill.Skill.Hidden)
                        {
                            Skill.PlayerSKillManager.Remove(args.Player.Name, skill.ID);
                        }
                    }
                    args.Player.SendSuccessMessage(GetString("成功移除了手持武器的所有技能!"));
                    return;
                }
                else if (args.Parameters[0].ToLower() == "clear")
                {
                    var skills = Skill.PlayerSKillManager.QuerySkill(args.Player.Name);
                    if (!skills.Any())
                    {
                        args.Player.SendErrorMessage(GetString("你并未绑定技能!"));
                        return;
                    }
                    foreach (var skill in skills)
                    {
                        if (skill.Skill != null && !skill.Skill.Hidden)
                        {
                            Skill.PlayerSKillManager.Remove(args.Player.Name, skill.ID);
                        }

                    }
                    args.Player.SendSuccessMessage(GetString("成功移除了绑定的所有技能!"));
                    return;
                }
                else if (args.Parameters[0].ToLower() == "reset")
                {
                    if (!args.Player.HasPermission(Permission.SkillAdmin))
                    {
                        args.Player.SendErrorMessage(GetString("你没有权限执行此命令!"));
                        return;
                    }
                    Skill.PlayerSKillManager.ClearTable();
                    args.Player.SendSuccessMessage(GetString("技能重置成功!"));
                    return;
                }
                break;

            }
            case 3:
            {
                var player = TShock.Players.FirstOrDefault(x => x.Name == args.Parameters[1]);
                if (player == null)
                {
                    args.Player.SendErrorMessage(GetString("目标玩家不存在或不在线!"));
                    return;
                }
                if (args.Parameters[0].ToLower() == "give" && args.Player.HasPermission(Permission.SkillAdmin))
                {
                    SkillContext? skill;
                    if (!int.TryParse(args.Parameters[2], out var index) || (skill = Skill.Config.GetSkill(index)) == null)
                    {
                        args.Player.SendErrorMessage(GetString("无效得技能序号!"));
                        return;
                    }
                    Skill.PlayerSKillManager.Add(player.Name, player.SelectedItem.netID, index);
                    args.Player.SendSuccessMessage(GetString($"成功为 {player.Name} 添加了一个技能 {skill.Name}"));
                    player.SendSuccessMessage(GetString($"{args.Player.Name} 为你添加了一个技能 {skill.Name}"));
                }
                else if (args.Parameters[0].ToLower() == "del" && args.Player.HasPermission(Permission.SkillAdmin))
                {
                    var playerSkills = Skill.PlayerSKillManager.QuerySkill(player.Name);
                    if (!int.TryParse(args.Parameters[2], out var index) || !playerSkills.Any(x => x.ID == index))
                    {
                        args.Player.SendErrorMessage(GetString("无效得技能序号!"));
                        return;
                    }
                    Skill.PlayerSKillManager.Remove(player.Name, index);
                    args.Player.SendSuccessMessage(GetString($"成功删除 {player.Name} 的一个技能"));
                    player.SendSuccessMessage(GetString($"{args.Player.Name} 删除了你的一个技能 "));
                }
                break;
            }
            default:
                args.Player.SendInfoMessage(GetString("/skill buy [技能ID]"));
                args.Player.SendInfoMessage(GetString("/skill del [技能ID]"));
                args.Player.SendInfoMessage(GetString("/skill list [页码]"));
                args.Player.SendInfoMessage(GetString("/skill give [玩家] [技能序号]"));
                args.Player.SendInfoMessage(GetString("/skill clearh [玩家]"));
                args.Player.SendInfoMessage(GetString("/skill del [玩家] [序号]"));
                args.Player.SendInfoMessage(GetString("/skill removeall"));
                args.Player.SendInfoMessage(GetString("/skill clear"));
                args.Player.SendInfoMessage(GetString("/skill reset"));
                break;
        }
    }
}