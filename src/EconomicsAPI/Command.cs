using EconomicsAPI.Attributes;
using EconomicsAPI.Configured;
using EconomicsAPI.Extensions;
using System.Text;
using TShockAPI;

namespace EconomicsAPI;

[RegisterSeries]
public class Command
{
    #region 货币查询
    [Obsolete]
    [CommandMap("查询", EconomicsPerm.ObseleteQueryCurrency)]
    public void QueryCurrency(CommandArgs args)
    {
        var sb = new StringBuilder();
        foreach (var currency in Economics.Setting.CustomizeCurrencys)
        {
            sb.AppendLine(string.Format(currency.QueryFormat,
            currency.Name,
            Economics.CurrencyManager.GetUserCurrency(args.Player.Name, currency.Name).Number));
        }
        args.Player.SendGradientMsg(sb.ToString().Trim());
    }
    #endregion

    #region bank 命令
    [CommandMap("bank", EconomicsPerm.CurrencyAdmin, EconomicsPerm.PayCurrency, EconomicsPerm.QueryCurrency)]
    public void Bank(CommandArgs args)
    {
        bool ParametersLengthValidator(int length)
        {
            if (args.Parameters.Count < length)
            {
                args.Player.SendErrorMessage(GetString("语法错误，请输入/bank help查看正确语法"));
                return true; // is_return
            }
            return false;
        }

        bool PermissionValidator(params string[] permissions)
        {
            if (!permissions.All(x => args.Player.HasPermission(x)))
            {
                args.Player.SendErrorMessage(GetString("你没有权限执行此命令!"));
                return true;
            }
            return false;
        }

        bool CurrencyValidator(string type, out CustomizeCurrency? currencyOption)
        {
            if (Economics.Setting.GetCurrencyOption(type) is CustomizeCurrency option)
            {
                currencyOption = option;
                return false;
            }
            currencyOption = null;
            args.Player.SendErrorMessage(GetString($"货币类型`{type}`不存在!"));
            return true;
        }

        if (ParametersLengthValidator(1))
        {
            return;
        }
        switch (args.Parameters[0].ToLower())
        {
            case "help":
            {
                args.Player.SendInfoMessage(GetString("bank 指令"));
                args.Player.SendInfoMessage(GetString("/bank add <用户> <数量> <类型>"));
                args.Player.SendInfoMessage(GetString("/bank deduct <用户> <数量> <类型>"));
                args.Player.SendInfoMessage(GetString("/bank pay <用户> <数量> <类型>"));
                args.Player.SendInfoMessage(GetString("/bank query [用户]"));
                args.Player.SendInfoMessage(GetString("/bank clear <用户>"));
                args.Player.SendInfoMessage(GetString("/bank reset"));
                return;
            }

            case "reset":
            {
                if (PermissionValidator(EconomicsPerm.CurrencyAdmin))
                {
                    return;
                }
                Economics.CurrencyManager.Reset();
                args.Player.SendSuccessMessage(GetString($"Economics 已重置"));
                return;
            }

            case "query":
            {
                if (PermissionValidator(EconomicsPerm.QueryCurrency))
                {
                    return;
                }
                if (args.Parameters.Count > 1)
                {
                    break;
                }
                var sb = new StringBuilder();
                foreach (var currency in Economics.Setting.CustomizeCurrencys)
                {
                    sb.AppendLine(string.Format(currency.QueryFormat,
                    currency.Name,
                    Economics.CurrencyManager.GetUserCurrency(args.Player.Name, currency.Name).Number));
                }
                args.Player.SendGradientMsg(sb.ToString().Trim());
                return;
            }

            default:
                break;
        }


        if (ParametersLengthValidator(2))
        {
            return;
        }
        switch (args.Parameters[0].ToLower())
        {
            case "clear":
            {
                if (PermissionValidator(EconomicsPerm.CurrencyAdmin))
                {
                    return;
                }
                foreach (var currency in Economics.Setting.CustomizeCurrencys)
                {
                    Economics.CurrencyManager.ClearUserCurrency(args.Parameters[1], currency.Name);
                }

                args.Player.SendSuccessMessage(GetString($"已清空`{args.Parameters[1]}`的所有{string.Join(",", Economics.Setting.CustomizeCurrencys.Select(x => x.Name))}"));
                return;
            }

            case "query":
            {
                if (PermissionValidator(EconomicsPerm.QueryCurrency))
                {
                    return;
                }
                var name = args.Parameters[1];
                var sb = new StringBuilder();
                foreach (var currency in Economics.Setting.CustomizeCurrencys)
                {
                    sb.AppendLine(string.Format(currency.QueryFormat,
                    currency.Name,
                    Economics.CurrencyManager.GetUserCurrency(name, currency.Name).Number));
                }
                args.Player.SendGradientMsg(sb.ToString().Trim());
                return;
            }

            default:
                break;
        }


        if (ParametersLengthValidator(4))
        {
            return;
        }
        if (!long.TryParse(args.Parameters[2], out var num))
        {
            args.Player.SendErrorMessage(GetString("请输入一个有效数值!"));
            return;
        }
        if (CurrencyValidator(args.Parameters[3], out var targetCurrency) || targetCurrency == null)
        {
            return;
        }
        switch (args.Parameters[0].ToLower())
        {
            case "add":
            {
                if (PermissionValidator(EconomicsPerm.CurrencyAdmin))
                {
                    return;
                }
                var name = args.Parameters[1];
                Economics.CurrencyManager.AddUserCurrency(name, num, args.Parameters[3]);
                args.Player.SendSuccessMessage(GetString($"成功为`{name}`添加 {num} 个{args.Parameters[3]}"));
                return;
            }

            case "deduct":
            case "del": // obselete
            {
                if (PermissionValidator(EconomicsPerm.CurrencyAdmin))
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
            case "pay":
            {
                if (PermissionValidator(EconomicsPerm.PayCurrency))
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
            case "cash":
            {
                if (PermissionValidator(EconomicsPerm.CashCurrent))
                {
                    return;
                }
                if (CurrencyValidator(args.Parameters[1], out var sourceCurrency) || sourceCurrency == null)
                {
                    return;
                }
                var currency = Economics.CurrencyManager.GetUserCurrency(args.Player.Name, targetCurrency.Name);
                var Relationships = sourceCurrency.RedemptionRelationships.Find(x => x.CurrencyType == targetCurrency.Name);
                if (Relationships == null || currency == null)
                {
                    args.Player.SendErrorMessage(GetString($"货币{sourceCurrency.Name}无法兑换为{targetCurrency.Name}!"));
                    return;
                }
                var deduct = Relationships.Number * num;
                if (deduct > currency.Number)
                {
                    args.Player.SendErrorMessage(GetString($"兑换{num}个{targetCurrency.Name}需要{deduct}个{sourceCurrency.Name}，而您只有{currency.Number}个!"));
                    return;
                }
                Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, deduct, sourceCurrency.Name);
                Economics.CurrencyManager.AddUserCurrency(args.Player.Name, num, targetCurrency.Name);
                args.Player.SendSuccessMessage(GetString($"成功兑换{targetCurrency.Name}{num}个!"));
                break;
            }
            default:
                break;
        }
        args.Player.SendErrorMessage(GetString("语法错误，请输入/bank help查看正确语法"));
    }
    #endregion
}