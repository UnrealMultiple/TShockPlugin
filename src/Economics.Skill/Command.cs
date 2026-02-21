using Economics.Core.Command;
using Economics.Skill.Model;
using Economics.Skill.Setting;
using TShockAPI;

namespace Economics.Skill;

public class Command : BaseCommand
{
    public override string[] Alias => ["skill"];

    public override List<string> Permissions => ["economics.skill.use"];

    public override string HelpText => GetString("你将拥有技能!");

    public override string ErrorText => GetString("语法错误，请输入/skill help查看正确使用方法!");

    [SubCommand("buy", 2)]
    [HelpText("/skill buy <id>")]
    [OnlyPlayer]
    public static void SkillBuy(CommandArgs args)
    {
        if (!int.TryParse(args.Parameters[1], out var index))
        {
            args.Player.SendErrorMessage(GetString("请输入一个正确的序号!"));
            return;
        }
        try
        {
            var skill = Utils.VerifyBindSkill(args.Player, index);
            if (!Core.Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, skill.RedemptionRelationshipsOption))
            {
                args.Player.SendErrorMessage(GetString($"你的货币不足购买此技能!"));
                return;
            }
            Skill.PlayerSKillManager.Add(args.Player.Name, args.Player.SelectedItem.type, index, 1);
            args.Player.SendSuccessMessage(GetString("购买成功，技能已绑定!"));
            return;
        }
        catch (Exception ex)
        {
            args.Player.SendErrorMessage(ex.Message);
        }
    }

    [SubCommand("del", 2)]
    [HelpText("/skill del <id>")]
    [OnlyPlayer]
    public static void SkillDel(CommandArgs args)
    {
        if (args.Parameters.Count >= 3)
        {
            var player = TShock.Players.FirstOrDefault(x => x.Name == args.Parameters[1]);
            if (player == null)
            {
                args.Player.SendErrorMessage(GetString("目标玩家不存在或不在线!"));
                return;
            }
            var playerSkills = Skill.PlayerSKillManager.QuerySkill(player.Name);
            if (!int.TryParse(args.Parameters[2], out var id) || !playerSkills.Any(x => x.ID == id))
            {
                args.Player.SendErrorMessage(GetString("无效得技能序号!"));
                return;
            }
            Skill.PlayerSKillManager.Remove(player.Name, id);
            args.Player.SendSuccessMessage(GetString($"成功删除 {player.Name} 的一个技能"));
            player.SendSuccessMessage(GetString($"{args.Player.Name} 删除了你的一个技能 "));
        }
        else
        {
            if (!int.TryParse(args.Parameters[1], out var index))
            {
                args.Player.SendErrorMessage(GetString("请输入一个正确的序号!"));
                return;
            }
            if (!Skill.PlayerSKillManager.HasSkill(args.Player.Name, args.Player.SelectedItem.type, index)
                            && !Skill.PlayerSKillManager.HasSkill(args.Player.Name, index))
            {
                args.Player.SendErrorMessage(GetString("你未绑定此技能，无需删除！"));
                return;
            }
            Skill.PlayerSKillManager.Remove(args.Player.Name, index);
            args.Player.SendSuccessMessage(GetString("技能移除成功!"));
        }

    }

    [SubCommand("clearh", 2)]
    [HelpText("/skill clearh <player>")]
    public static void SkillClearh(CommandArgs args)
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

    [SubCommand("ms")]
    [OnlyPlayer]
    public static void SkillMy(CommandArgs args)
    {
        var skills = Skill.PlayerSKillManager.QuerySkill(args.Player.Name);
        if (skills.Count == 0)
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
    }

    [SubCommand("removeall")]
    [OnlyPlayer]
    public static void SkillRemoveAll(CommandArgs args)
    {
        var skills = Skill.PlayerSKillManager.QuerySkillByItem(args.Player.Name, args.Player.SelectedItem.type);
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
    }

    [SubCommand("clear")]
    [OnlyPlayer]
    public static void SkillClear(CommandArgs args)
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
    }

    [SubCommand("reset")]
    [CommandPermission(Permission.SkillAdmin)]
    public static void SkillReset(CommandArgs args)
    {
        Skill.PlayerSKillManager.ClearTable();
        args.Player.SendSuccessMessage(GetString("技能重置成功!"));
    }

    [SubCommand("up", 2)]
    [HelpText("/skill up <id>")]
    [OnlyPlayer]
    public static void SkillUp(CommandArgs args)
    {
        if (!int.TryParse(args.Parameters[1], out var index))
        {
            args.Player.SendErrorMessage(GetString("请输入一个正确的序号!"));
            return;
        }
        var skill = Config.Instance.GetSkill(index);
        if (skill == null)
        {
            args.Player.SendErrorMessage(GetString("无效的技能序号!"));
            return;
        }
        var playerSkills = Skill.PlayerSKillManager.QuerySkill(args.Player.Name).FirstOrDefault(x => x.ID == index);
        if (playerSkills == null)
        {
            args.Player.SendErrorMessage(GetString("你未绑定此技能，无法升级!"));
            return;
        }
        if(skill.SkillLevelOptions.TryGetValue(playerSkills.Level + 1, out var nextLevelCost) && nextLevelCost != null)
        {
            if (!Core.Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, nextLevelCost))
            {
                args.Player.SendErrorMessage(GetString("你的货币不足以升级此技能!"));
                return;
            }
            Skill.PlayerSKillManager.UpdateLevel(playerSkills);
            args.Player.SendSuccessMessage(GetString($"技能 {skill.Name} 升级成功! 当前等级: {playerSkills.Level}"));
        }
        else
        {
            args.Player.SendErrorMessage(GetString("技能已达到最高等级，无法升级!"));
        }
    }   

    [SubCommand("give", 3)]
    [HelpText("/skill give <player> <id>")]
    [CommandPermission(Permission.SkillAdmin)]
    public static void SkillGive(CommandArgs args)
    {
        var player = TShock.Players.FirstOrDefault(x => x.Name == args.Parameters[1]);
        if (player == null)
        {
            args.Player.SendErrorMessage(GetString("目标玩家不存在或不在线!"));
            return;
        }
        SkillContext? skill;
        if (!int.TryParse(args.Parameters[2], out var index) || (skill = Config.Instance.GetSkill(index)) == null)
        {
            args.Player.SendErrorMessage(GetString("无效得技能序号!"));
            return;
        }
        Skill.PlayerSKillManager.Add(player.Name, player.SelectedItem.type, index, 1);
        args.Player.SendSuccessMessage(GetString($"成功为 {player.Name} 添加了一个技能 {skill.Name}"));
        player.SendSuccessMessage(GetString($"{args.Player.Name} 为你添加了一个技能 {skill.Name}"));
    }

    [SubCommand("list")]
    public static void List(CommandArgs args)
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
                        MaxLinesPerPage = Config.Instance.PageMax,
                        NothingToDisplayString = GetString("当前技能列表空空如也"),
                        HeaderFormat = GetString("技能列表 ({0}/{1})："),
                        FooterFormat = GetString("输入 {0}skill list {{0}} 查看更多").SFormat(Commands.Specifier)
                    }
                );
        }
        var line = new List<string>();
        for (var i = 0; i < Config.Instance.SkillContexts.Count; i++)
        {
            line.Add(GetString($"{i + 1}. {Config.Instance.SkillContexts[i].Name}  价格 {string.Join(" ", Config.Instance.SkillContexts[i].RedemptionRelationshipsOption.Select(x => $"{x.CurrencyType}x{x.Number}"))})"));
        }

        Show(line);
    }

    [SubCommand("help")]
    public static void Help(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString("/skill buy [技能ID]"));
        args.Player.SendInfoMessage(GetString("/skill del [技能ID]"));
        args.Player.SendInfoMessage(GetString("/skill list [页码]"));
        args.Player.SendInfoMessage(GetString("/skill give [玩家] [技能序号]"));
        args.Player.SendInfoMessage(GetString("/skill clearh [玩家]"));
        args.Player.SendInfoMessage(GetString("/skill del [玩家] [序号]"));
        args.Player.SendInfoMessage(GetString("/skill removeall"));
        args.Player.SendInfoMessage(GetString("/skill clear"));
        args.Player.SendInfoMessage(GetString("/skill reset"));
    }
}