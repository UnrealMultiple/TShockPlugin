using EconomicsAPI.Attributes;
using TShockAPI;

namespace EconomicsAPI;

internal class Command
{
    #region 货币查询
    [CommandMap( "查询", EconomicsPerm.QueryCurrency)]
    public void QueryCurrency(CommandArgs args)
    {
        args.Player.SendInfoMessage(string.Format(Economics.Setting.QueryFormat, 
            Economics.Setting.CurrencyName, 
            Economics.CurrencyManager.GetUserCurrency(args.Player.Name)));
    }
    #endregion

    #region bank 命令
    [CommandMap("bank", EconomicsPerm.CurrencyAdmin, EconomicsPerm.PayCurrency)]
    public void Bank(CommandArgs args)
    {
        if (args.Parameters.Count == 3)
        {
            var name = args.Parameters[1];
            if (!long.TryParse(args.Parameters[2], out long num))
            {
                args.Player.SendErrorMessage("请输入一个有效数值!");
                return;
            }
            switch (args.Parameters[0].ToLower())
            {
                case "add":
                    {
                        Economics.CurrencyManager.AddUserCurrency(name, num);
                        args.Player.SendSuccessMessage($"成功为`{name}`添加 {num} 个{Economics.Setting.CurrencyName}");
                        break;
                    }
                case "del":
                    {
                        if (Economics.CurrencyManager.DelUserCurrency(name, num))
                        {
                            args.Player.SendSuccessMessage($"成功删除`{name}`的 {num} 个{Economics.Setting.CurrencyName}");
                            return;
                        }
                        args.Player.SendErrorMessage($"用户`{name}`仅有{Economics.CurrencyManager.GetUserCurrency(name)}个{Economics.Setting.CurrencyName}");
                        break;
                    }
                case "pay":
                    {
                        if (!args.Player.HasPermission(EconomicsPerm.PayCurrency))
                        {
                            args.Player.SendErrorMessage("你无权执行此命令!");
                            return;
                        }
                        if (Economics.CurrencyManager.DelUserCurrency(args.Player.Name, num))
                        {
                            Economics.CurrencyManager.AddUserCurrency(name, num);
                            args.Player.SendSuccessMessage($"成功转账给`{name}` {num} 个{Economics.Setting.CurrencyName}");
                            return;
                        }
                        break;
                    }

                default:
                    args.Player.SendErrorMessage("语法错误，请输入/bank help查看正确语法");
                    break;
            }
        }
        else if (args.Parameters.Count == 2 && args.Parameters[0] == "clear")
        {
            Economics.CurrencyManager.ClearUserCurrency(args.Parameters[1]);
            args.Player.SendSuccessMessage($"已清空`{args.Parameters[1]}`的所有{Economics.Setting.CurrencyName}");
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0] == "reset")
        {
            Economics.CurrencyManager.Reset();
            args.Player.SendSuccessMessage($"Economics 已重置");
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0] == "help")
        {
            args.Player.SendInfoMessage("bank 指令");
            args.Player.SendInfoMessage("/bank add [用户] [数量]");
            args.Player.SendInfoMessage("/bank del [用户] [数量]");
            args.Player.SendInfoMessage("/bank pay [用户] [数量]");
            args.Player.SendInfoMessage("/bank clear [用户]");
            args.Player.SendInfoMessage("/bank reset");
        }
        else
        {
            args.Player.SendErrorMessage("语法错误，请输入/bank help查看正确语法");
        }
    }
    #endregion
}
