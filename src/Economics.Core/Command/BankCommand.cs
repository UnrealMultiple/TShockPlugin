using Economics.Core.ConfigFiles;
using Economics.Core.Extensions;
using EconomicsAPI;
using System.Text;
using TShockAPI;

namespace Economics.Core.Command;

public class BankCommand : BaseCommand
{
    public override string[] Alias => ["bank"];

    public override List<string> Permissions => [EconomicsPerm.CurrencyAdmin, EconomicsPerm.PayCurrency, EconomicsPerm.QueryCurrency];

    public override string ErrorText => GetString("语法错误，请输入/bank help查看正确使用方法!");

    private static bool NumberValidator(CommandArgs args, out long num)
    {
        if (!long.TryParse(args.Parameters[2], out num))
        {
            args.Player.SendErrorMessage(GetString("请输入一个有效数值!"));
            return true;
        }
        return false;
    }

    [SubCommand("cash", 3)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin, EconomicsPerm.CashCurrent)]
    [HelpText("/bank cash <currency> <amount>")]
    [OnlyPlayer]
    public static void BankCash(CommandArgs args)
    {
        var target = Setting.Instance.GetCurrencyOption(args.Parameters[1]);
        if (target == null)
        {
            args.Player.SendErrorMessage(GetString($"不存在可兑换的货币类型:{args.Parameters[1]}"));
            return;
        }
        if (NumberValidator(args, out var num))
        {
            return;
        }
        if (!Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, target.RedemptionRelationships, (int)num))
        {
            args.Player.SendErrorMessage(GetString($"你得货币不足以兑换{num}个{target.Name}"));
            return;
        }
        Economics.CurrencyManager.AddUserCurrency(args.Player.Name, num, target.Name);
        args.Player.SendSuccessMessage(GetString($"成功兑换{target.Name}{num}个!"));
    }

    [SubCommand("pay", 4)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin, EconomicsPerm.PayCurrency)]
    [HelpText("/bank pay <player> <amount> <currency>")]
    [OnlyPlayerAttribute]
    public static void BankPay(CommandArgs args)
    {
        if (NumberValidator(args, out var num))
        {
            return;
        }
        var name = args.Parameters[1];
        if (Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, num, args.Parameters[3]))
        {
            Economics.CurrencyManager.AddUserCurrency(name, num, args.Parameters[3]);
            args.Player.SendSuccessMessage(GetString($"成功转账给`{name}` {num} 个{args.Parameters[3]}"));
            return;
        }
        else
        {
            args.Player.SendErrorMessage(GetString($"你的{args.Parameters[3]}不足，无法转账!"));
            return;
        }
    }

    [SubCommand("deduct", 4)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin)]
    [HelpText("/bank deduct <target> <amount> <currency>")]
    public static void BankDeduct(CommandArgs args)
    {
        if (NumberValidator(args, out var num))
        {
            return;
        }
        var name = args.Parameters[1];
        if (Economics.CurrencyManager.DeductUserCurrency(name, num, args.Parameters[3]))
        {
            args.Player.SendSuccessMessage(GetString($"成功减去`{name}`的 {num} 个{args.Parameters[3]}"));
            return;
        }
        args.Player.SendErrorMessage(GetString($"用户`{name}`仅有{Economics.CurrencyManager.GetUserCurrency(name, args.Parameters[3]).Number}个{args.Parameters[3]}"));
        return;
    }

    [SubCommand("lb", 2)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin)]
    [HelpText("/bank lb <currency> <count>")]
    public static void BankLeaderBoard(CommandArgs args)
    {
        if (Setting.Instance.HasCustomizeCurrency(args.Parameters[1]))
        {
            args.Player.SendErrorMessage(GetString($"不存在的的货币类型`{args.Parameters[1]}`"));
            return;
        }
        if (NumberValidator(args, out var count))
        {
            return;
        }
        
        var currencys = Economics.CurrencyManager.GetCurrencies()
            .Where(c => c.CurrencyType == args.Parameters[1])
            .OrderByDescending(c => c.Number)
            .Take((int)count);
        if (!currencys.Any())
        {
            args.Player.SendSuccessMessage(GetString($"当前无人用有{args.Parameters[1]}"));
            return;
        }
        args.Player.SendSuccessMessage(GetString($"货币`{args.Parameters[1]}`排行榜:"));
        var lines = new List<string>();
        var index = 1;
        foreach (var currency in currencys)
        {
            args.Player.SendSuccessMessage(GetString($"{index}.{currency.PlayerName} 拥有 {currency.CurrencyType} {currency.Number}个。"));
            index++;
        }
    }

    [SubCommand("clear", 2)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin)]
    [HelpText("/bank clear <player>")]
    public static void BankClear(CommandArgs args)
    {
        foreach (var currency in Setting.Instance.CustomizeCurrencys)
        {
            Economics.CurrencyManager.ClearUserCurrency(args.Parameters[1], currency.Name);
        }
        args.Player.SendSuccessMessage(GetString($"已清空`{args.Parameters[1]}`的所有{string.Join(",", Setting.Instance.CustomizeCurrencys.Select(x => x.Name))}"));
        return;
    }

    [SubCommand("query")]
    [CommandPermission(EconomicsPerm.QueryCurrency, EconomicsPerm.CurrencyAdmin)]
    [HelpText("/bank query or /bank quer <player>")]
    public static void BankQuery(CommandArgs args)
    {
        var sb = new StringBuilder();
        string? name;
        if (args.Parameters.Count > 1)
        {
            name = args.Parameters[1];
            return;
        }
        else
        {
            if (!args.Player.RealPlayer)
            {
                args.Player.SendErrorMessage(GetString("在没有名字参数情况下，只能在游戏中使用此命令!"));
                return;
            }
            name = args.Player.Name;
        }
        foreach (var currency in Setting.Instance.CustomizeCurrencys)
        {
            sb.AppendLine(string.Format(currency.QueryFormat,
            currency.Name,
            Economics.CurrencyManager.GetUserCurrency(name, currency.Name).Number));
        }
        args.Player.SendGradientMsg(sb.ToString().Trim());
        return;
    }

    [SubCommand("reset")]
    [CommandPermission(EconomicsPerm.CurrencyAdmin)]
    public static void BankReset(CommandArgs args)
    {
        Economics.CurrencyManager.Reset();
        args.Player.SendSuccessMessage(GetString($"Economics 已重置"));
    }

    [SubCommand("add", 4)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin)]
    [HelpText("/bank add <player> <amount> <currency>")]
    public static void BankAdd(CommandArgs args)
    {
        if (NumberValidator(args, out var num))
        {
            return;
        }
        var name = args.Parameters[1];
        Economics.CurrencyManager.AddUserCurrency(name, num, args.Parameters[3]);
        args.Player.SendSuccessMessage(GetString($"成功为`{name}`添加 {num} 个{args.Parameters[3]}"));
        return;
    }

    [SubCommand("help")]
    public static void BankHelp(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString("bank 指令"));
        args.Player.SendInfoMessage(GetString("/bank add <用户> <数量> <类型>"));
        args.Player.SendInfoMessage(GetString("/bank deduct <用户> <数量> <类型>"));
        args.Player.SendInfoMessage(GetString("/bank pay <用户> <数量> <类型>"));
        args.Player.SendInfoMessage(GetString("/bank cash <目标货币> <数量>"));
        args.Player.SendInfoMessage(GetString("/bank query [用户]"));
        args.Player.SendInfoMessage(GetString("/bank clear <用户>"));
        args.Player.SendInfoMessage(GetString("/bank reset"));
    }
}
