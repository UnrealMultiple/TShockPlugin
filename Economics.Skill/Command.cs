using EconomicsAPI.Attributes;
using TShockAPI;

namespace Economics.Skill;

public class Command
{
    [CommandMap("skill", Permission.SkillUse)]
    public void CSkill(CommandArgs args)
    {
        void Show(List<string> line)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber))
                return;

            PaginationTools.SendPage(
                    args.Player,
                    pageNumber,
                    line,
                    new PaginationTools.Settings
                    {
                        MaxLinesPerPage = Skill.Config.PageMax,
                        NothingToDisplayString = "当前技能列表空空如也",
                        HeaderFormat = "技能列表 ({0}/{1})：",
                        FooterFormat = "输入 {0}skill list {{0}} 查看更多".SFormat(Commands.Specifier)
                    }
                );
        }
        if (!args.Player.IsLoggedIn)
        {
            args.Player.SendErrorMessage("你必须登陆游戏才能购买技能!");
            return;
        }

        if (args.Parameters.Count >= 1 && args.Parameters[0].ToLower() == "list")
        {
            var line = new List<string>();
            for (int i = 0; i < Skill.Config.SkillContexts.Count; i++)
                line.Add($"{i + 1}. {Skill.Config.SkillContexts[i].Name}  价格 {Skill.Config.SkillContexts[i].Cost}");
            Show(line);
        }
        switch (args.Parameters.Count)
        {
            case 2:
                {
                    if (!int.TryParse(args.Parameters[1], out var index))
                    {
                        args.Player.SendErrorMessage("请输入一个正确的序号!");
                        return;
                    }
                    if (args.Parameters[0].ToLower() == "buy")
                    {
                        try
                        {
                            var skill = Utils.VerifyBindSkill(args.Player, index);
                            if (!EconomicsAPI.Economics.CurrencyManager.DelUserCurrency(args.Player.Name, skill.Cost))
                            {
                                args.Player.SendErrorMessage($"你的{EconomicsAPI.Economics.Setting.CurrencyName} 不足购买此技能!");
                                return;
                            }
                            Skill.PlayerSKillManager.Add(args.Player.Name, args.Player.SelectedItem.netID, index);
                            args.Player.SendSuccessMessage("购买成功，技能已帮打!");
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
                        if (!Skill.PlayerSKillManager.HasSkill(args.Player.Name, args.Player.SelectedItem.netID, index))
                        {
                            args.Player.SendErrorMessage("手持武器未绑定此技能，无需删除！");
                            return;
                        }
                        Skill.PlayerSKillManager.Remove(args.Player.Name, args.Player.SelectedItem.netID, index);
                        args.Player.SendSuccessMessage("技能移除成功!");
                        return;
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
                            args.Player.SendErrorMessage("你并未绑定技能!");
                            return;
                        }
                        args.Player.SendSuccessMessage("查询成功!");
                        foreach(var skill in skills)
                        args.Player.SendSuccessMessage($"[i:{skill.BindItem}] 绑定技能  {skill.Name}");
                        return;
                    }

                    if (args.Parameters[0].ToLower() == "delall")
                    {
                        var skills = Skill.PlayerSKillManager.QuerySkillByItem(args.Player.Name, args.Player.SelectedItem.netID);
                        if (!skills.Any())
                        {
                            args.Player.SendErrorMessage("手持物品并未绑定技能!");
                            return;
                        }
                        foreach (var skill in skills)
                        {
                            Skill.PlayerSKillManager.Remove(args.Player.Name, args.Player.SelectedItem.netID, skill.ID);
                        }
                        args.Player.SendSuccessMessage("成功移除了手持武器的所有技能!");
                        return;
                    }
                    else if (args.Parameters[0].ToLower() == "clear")
                    {
                        var skills = Skill.PlayerSKillManager.QuerySkill(args.Player.Name);
                        if (!skills.Any())
                        {
                            args.Player.SendErrorMessage("你并未绑定技能!");
                            return;
                        }
                        foreach (var skill in skills)
                        {
                            Skill.PlayerSKillManager.Remove(args.Player.Name, args.Player.SelectedItem.netID, skill.ID);
                        }
                        args.Player.SendSuccessMessage("成功移除了绑定的所有技能!");
                        return;
                    }
                    else if (args.Parameters[0].ToLower() == "reset")
                    {
                        if (!args.Player.HasPermission(Permission.SkillAdmin))
                        {
                            args.Player.SendErrorMessage("你没有权限执行此命令!");
                            return;
                        }
                        Skill.PlayerSKillManager.ClearTable();
                        args.Player.SendSuccessMessage("技能重置成功!");
                        return;
                    }
                    break;
                }
            default:
                args.Player.SendInfoMessage("/skill buy [技能ID]");
                args.Player.SendInfoMessage("/skill del [技能ID]");
                args.Player.SendInfoMessage("/skill list [页码]");
                args.Player.SendInfoMessage("/skill delall");
                args.Player.SendInfoMessage("/skill clear");
                args.Player.SendInfoMessage("/skill reset");
                break;
        }
    }
}
