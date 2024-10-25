using EconomicsAPI.Attributes;
using EconomicsAPI.Extensions;
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
        args.Player.SendGradientMsg(string.Format(Economics.Setting.QueryFormat,
            Economics.Setting.CurrencyName,
            Economics.CurrencyManager.GetUserCurrency(args.Player.Name),
            GetString("你")));
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

        if (ParametersLengthValidator(1))
        {
            return;
        }
        switch (args.Parameters[0].ToLower())
        {
            case "help":
            {
                args.Player.SendInfoMessage(GetString("bank 指令"));
                args.Player.SendInfoMessage(GetString("/bank add <用户> <数量>"));
                args.Player.SendInfoMessage(GetString("/bank deduct <用户> <数量>"));
                args.Player.SendInfoMessage(GetString("/bank pay <用户> <数量>"));
                args.Player.SendInfoMessage(GetString("/bank query [用户]"));
                args.Player.SendInfoMessage(GetString("/bank clear <用户>"));
                args.Player.SendInfoMessage(GetString("/bank reset"));
                return;
            }

            case "reset":
            {
                if (PermissionValidator(EconomicsPerm.CurrencyAdmin)) {
                    return;
                }
                Economics.CurrencyManager.Reset();
                args.Player.SendSuccessMessage(GetString($"Economics 已重置"));
                return;
            }

            case "query":
            {
                if (PermissionValidator(EconomicsPerm.QueryCurrency)) {
                    return;
                }
                if (args.Parameters.Count > 1)
                {
                    break;
                }
                args.Player.SendGradientMsg(string.Format(Economics.Setting.QueryFormat,
                    Economics.Setting.CurrencyName,
                    Economics.CurrencyManager.GetUserCurrency(args.Player.Name),
                    GetString("你")));
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
                if (PermissionValidator(EconomicsPerm.CurrencyAdmin)) {
                    return;
                }
                Economics.CurrencyManager.ClearUserCurrency(args.Parameters[1]);
                args.Player.SendSuccessMessage(GetString($"已清空`{args.Parameters[1]}`的所有{Economics.Setting.CurrencyName}"));
                return;
            }

            case "query":
            {
                if (PermissionValidator(EconomicsPerm.QueryCurrency)) {
                    return;
                }
                var name = args.Parameters[1];
                args.Player.SendGradientMsg(string.Format(Economics.Setting.QueryFormat,
                    Economics.Setting.CurrencyName,
                    Economics.CurrencyManager.GetUserCurrency(name),
                    name));
                return;
            }
            
            default:
                break;
        }
        
        
        if (ParametersLengthValidator(3))
        {
            return;
        }
        if (!long.TryParse(args.Parameters[2], out var num))
        {
            args.Player.SendErrorMessage(GetString("请输入一个有效数值!"));
            return;
        }
        switch (args.Parameters[0].ToLower())
        {
            case "add":
            {
                if (PermissionValidator(EconomicsPerm.CurrencyAdmin)) {
                    return;
                }
                var name = args.Parameters[1];
                Economics.CurrencyManager.AddUserCurrency(name, num);
                args.Player.SendSuccessMessage(GetString($"成功为`{name}`添加 {num} 个{Economics.Setting.CurrencyName}"));
                return;
            }

            case "deduct":
            case "del": // obselete
            {
                if (PermissionValidator(EconomicsPerm.CurrencyAdmin)) {
                    return;
                }
                var name = args.Parameters[1];
                if (Economics.CurrencyManager.DeductUserCurrency(name, num))
                {
                    args.Player.SendSuccessMessage(GetString($"成功减去`{name}`的 {num} 个{Economics.Setting.CurrencyName}"));
                    return;
                }
                args.Player.SendErrorMessage(GetString($"用户`{name}`仅有{Economics.CurrencyManager.GetUserCurrency(name)}个{Economics.Setting.CurrencyName}"));
                return;
            }
            case "pay":
            {
                if (PermissionValidator(EconomicsPerm.PayCurrency)) {
                    return;
                }
                var name = args.Parameters[1];
                if (Economics.CurrencyManager.DeductUserCurrency(args.Player.Name, num))
                {
                    Economics.CurrencyManager.AddUserCurrency(name, num);
                    args.Player.SendSuccessMessage(GetString($"成功转账给`{name}` {num} 个{Economics.Setting.CurrencyName}"));
                    return;
                }
                else
                {
                    args.Player.SendSuccessMessage(GetString($"你的{Economics.Setting.CurrencyName}不足，无法转账!"));
                    return;
                }
            }

            default:
                break;
        }

        args.Player.SendErrorMessage(GetString("语法错误，请输入/bank help查看正确语法"));
    }
    #endregion
}