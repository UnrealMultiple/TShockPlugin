using Economics.Core.ConfigFiles;
using Economics.Core.Extensions;
using Economics.Core.Services;
using EconomicsAPI;
using System.Text;
using TShockAPI;

namespace Economics.Core.Command;

public class BankCommand : BaseCommand
{
    public override string[] Alias => ["bank"];

    public override List<string> Permissions => [EconomicsPerm.CurrencyAdmin, EconomicsPerm.PayCurrency, EconomicsPerm.QueryCurrency];

    public override string ErrorText => GetString("语法错误，请输入/bank help查看正确使用方法!");


    #region Exchange Commands

    [SubCommand("exchange", 4)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin, EconomicsPerm.CashCurrent)]
    [HelpText("/bank exchange <source> <target> <amount>")]
    [OnlyPlayer]
    public static void BankExchange(CommandArgs args)
    {
        var fromCurrency = args.Parameters[1];
        var toCurrency = args.Parameters[2];

        if (!TryParseAmount(args.Parameters[3], out var amount, args.Player))
        {
            return;
        }

        var result = Economics.ExchangeService.ExecuteExchange(args.Player.Name, fromCurrency, toCurrency, amount);

        if (result.IsFailure)
        {
            args.Player.SendErrorMessage(GetString(result.Error!));
            return;
        }

        var exchangeResult = result.Value!;
        var message = new StringBuilder()
            .AppendLine(GetString("成功兑换!"))
            .AppendLine(GetString($"消耗: {exchangeResult.FromAmount} {exchangeResult.FromCurrency}"))
            .AppendLine(GetString($"获得: {exchangeResult.ToAmount} {exchangeResult.ToCurrency}"))
            .ToString();

        args.Player.SendSuccessMessage(message);
    }

    [SubCommand("preview", 4)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin, EconomicsPerm.CashCurrent)]
    [HelpText("/bank preview <source> <target> <amount>")]
    [OnlyPlayer]
    public static void BankPreview(CommandArgs args)
    {
        var fromCurrency = args.Parameters[1];
        var toCurrency = args.Parameters[2];

        if (!TryParseAmount(args.Parameters[3], out var amount, args.Player))
        {
            return;
        }

        var balancesResult = Economics.CurrencyService.GetAllBalances(args.Player.Name);
        if (balancesResult.IsFailure)
        {
            args.Player.SendErrorMessage(GetString(balancesResult.Error!));
            return;
        }

        var preview = Economics.ExchangeService.GetExchangePreview(fromCurrency, toCurrency, amount, balancesResult.Value!);
        args.Player.SendInfoMessage(preview);
    }

    [SubCommand("rates", 1)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin, EconomicsPerm.CashCurrent)]
    [HelpText("/bank rates [currency]")]
    public static void BankRates(CommandArgs args)
    {
        var message = args.Parameters.Count > 1
            ? Economics.ExchangeService.GetExchangeOptions(args.Parameters[1])
            : Economics.ExchangeService.GetCurrencyGraph();
        args.Player.SendInfoMessage(message);
    }

    #endregion

    #region Transfer Commands

    [SubCommand("pay", 4)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin, EconomicsPerm.PayCurrency)]
    [HelpText("/bank pay <player> <amount> <currency>")]
    [OnlyPlayer]
    public static void BankPay(CommandArgs args)
    {
        var targetPlayer = args.Parameters[1];
        var currency = args.Parameters[3];

        if (!TryParseAmount(args.Parameters[2], out var amount, args.Player))
        {
            return;
        }

        var result = Economics.CurrencyService.Transfer(args.Player.Name, targetPlayer, currency, amount);

        if (result.IsFailure)
        {
            args.Player.SendErrorMessage(GetString(result.Error!));
            return;
        }

        args.Player.SendSuccessMessage(GetString($"成功转账给 `{targetPlayer}` {amount} 个{currency}"));
    }

    #endregion

    #region Admin Commands

    [SubCommand("add", 4)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin)]
    [HelpText("/bank add <player> <amount> <currency>")]
    public static void BankAdd(CommandArgs args)
    {
        var targetPlayer = args.Parameters[1];
        var currency = args.Parameters[3];

        if (!TryParseAmount(args.Parameters[2], out var amount, args.Player))
        {
            return;
        }

        var result = Economics.CurrencyService.AddCurrency(targetPlayer, currency, amount);

        if (result.IsFailure)
        {
            args.Player.SendErrorMessage(GetString(result.Error!));
            return;
        }

        args.Player.SendSuccessMessage(GetString($"成功为 `{targetPlayer}` 添加 {amount} 个{currency}"));
    }

    [SubCommand("deduct", 4)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin)]
    [HelpText("/bank deduct <target> <amount> <currency>")]
    public static void BankDeduct(CommandArgs args)
    {
        var targetPlayer = args.Parameters[1];
        var currency = args.Parameters[3];

        if (!TryParseAmount(args.Parameters[2], out var amount, args.Player))
        {
            return;
        }

        var result = Economics.CurrencyService.DeductCurrency(targetPlayer, currency, amount);

        if (result.IsFailure)
        {
            args.Player.SendErrorMessage(GetString(result.Error!));
            return;
        }

        args.Player.SendSuccessMessage(GetString($"成功减去 `{targetPlayer}` 的 {amount} 个{currency}"));
    }

    [SubCommand("clear", 2)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin)]
    [HelpText("/bank clear <player>")]
    public static void BankClear(CommandArgs args)
    {
        var targetPlayer = args.Parameters[1];
        var result = Economics.CurrencyService.ClearAllCurrencies(targetPlayer);

        if (result.IsFailure)
        {
            args.Player.SendErrorMessage(GetString(result.Error!));
            return;
        }

        var currencies = string.Join(", ", Setting.Instance.Currencies.Select(x => x.Name));
        args.Player.SendSuccessMessage(GetString($"已清空 `{targetPlayer}` 的所有货币: {currencies}"));
    }

    [SubCommand("reset")]
    [CommandPermission(EconomicsPerm.CurrencyAdmin)]
    public static void BankReset(CommandArgs args)
    {
        Economics.CurrencyService.ResetAllCurrencies();
        args.Player.SendSuccessMessage(GetString("Economics 已重置"));
    }

    #endregion

    #region Query Commands

    [SubCommand("query")]
    [CommandPermission(EconomicsPerm.QueryCurrency, EconomicsPerm.CurrencyAdmin)]
    [HelpText("/bank query or /bank query <player>")]
    public static void BankQuery(CommandArgs args)
    {
        var playerName = GetTargetPlayerName(args, out var errorMessage);
        if (playerName == null)
        {
            args.Player.SendErrorMessage(errorMessage!);
            return;
        }

        var sb = new StringBuilder();
        foreach (var currency in Setting.Instance.Currencies)
        {
            var balanceResult = Economics.CurrencyService.GetBalance(playerName, currency.Name);
            var balance = balanceResult.IsSuccess ? balanceResult.Value : 0;
            sb.AppendLine(string.Format(currency.QueryFormat, currency.Name, balance));
        }

        args.Player.SendGradientMsg(sb.ToString().Trim());
    }

    [SubCommand("lb", 2)]
    [CommandPermission(EconomicsPerm.CurrencyAdmin)]
    [HelpText("/bank lb <currency> <count>")]
    public static void BankLeaderBoard(CommandArgs args)
    {
        var currencyName = args.Parameters[1];

        if (!TryParseAmount(args.Parameters[2], out var count, args.Player))
        {
            return;
        }

        if (!Setting.Instance.HasCurrency(currencyName))
        {
            args.Player.SendErrorMessage(GetString($"不存在的货币类型 `{currencyName}`"));
            return;
        }

        var currencies = Economics.CurrencyService.GetAllCurrencyRecords()
            .Where(c => c.CurrencyType == currencyName)
            .OrderByDescending(c => c.Number)
            .Take((int)count)
            .ToList();

        if (currencies.Count == 0)
        {
            args.Player.SendSuccessMessage(GetString($"当前无人拥有 {currencyName}"));
            return;
        }

        args.Player.SendSuccessMessage(GetString($"货币 `{currencyName}` 排行榜:"));
        for (var i = 0; i < currencies.Count; i++)
        {
            var currency = currencies[i];
            args.Player.SendSuccessMessage(GetString($"{i + 1}. {currency.PlayerName} 拥有 {currency.CurrencyType} {currency.Number} 个"));
        }
    }

    [SubCommand("cycles")]
    [CommandPermission(EconomicsPerm.CurrencyAdmin)]
    [HelpText("/bank cycles")]
    public static void BankCycles(CommandArgs args)
    {
        var cycles = Economics.ExchangeService.GetDetectedCycles();

        if (cycles.Count == 0)
        {
            args.Player.SendInfoMessage(GetString("未检测到任何兑换循环"));
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine(GetString("=== 检测到的兑换循环 ==="));
        foreach (var cycle in cycles)
        {
            sb.AppendLine(cycle.Description);
        }

        args.Player.SendInfoMessage(sb.ToString());
    }

    #endregion

    #region Help

    [SubCommand("help")]
    public static void BankHelp(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString("=== Economics Bank 指令帮助 ==="));
        args.Player.SendInfoMessage(GetString("/bank add <用户> <数量> <类型> - 添加货币"));
        args.Player.SendInfoMessage(GetString("/bank deduct <用户> <数量> <类型> - 扣除货币"));
        args.Player.SendInfoMessage(GetString("/bank pay <用户> <数量> <类型> - 转账"));
        args.Player.SendInfoMessage(GetString("/bank exchange <源货币> <目标货币> <数量> - 兑换货币"));
        args.Player.SendInfoMessage(GetString("/bank preview <源货币> <目标货币> <数量> - 预览兑换"));
        args.Player.SendInfoMessage(GetString("/bank rates [货币] - 查看汇率"));
        args.Player.SendInfoMessage(GetString("/bank cycles - 查看兑换循环"));
        args.Player.SendInfoMessage(GetString("/bank query [用户] - 查询余额"));
        args.Player.SendInfoMessage(GetString("/bank lb <货币> <数量> - 排行榜"));
        args.Player.SendInfoMessage(GetString("/bank clear <用户> - 清空用户所有货币"));
        args.Player.SendInfoMessage(GetString("/bank reset - 重置所有数据"));
    }

    #endregion

    #region Helper Methods

    private static bool TryParseAmount(string input, out long amount, TSPlayer player)
    {
        if (!long.TryParse(input, out amount))
        {
            player.SendErrorMessage(GetString("请输入一个有效数值!"));
            return false;
        }

        if (amount <= 0)
        {
            player.SendErrorMessage(GetString("数值必须大于0!"));
            return false;
        }

        return true;
    }

    private static string? GetTargetPlayerName(CommandArgs args, out string? errorMessage)
    {
        errorMessage = null;

        if (args.Parameters.Count > 1)
        {
            return args.Parameters[1];
        }

        if (!args.Player.RealPlayer)
        {
            errorMessage = GetString("在没有名字参数情况下，只能在游戏中使用此命令!");
            return null;
        }

        return args.Player.Name;
    }

    #endregion
}
