using TShockAPI;

namespace TimeRate;

internal class Commands
{
    public static void times(CommandArgs args)
    {
        var info = TimeRate.Config.Enabled ? GetString("[c/3ADB2C:启用]") : GetString("[c/F25C71:关闭]");
        var info2 = TimeRate.Config.All ? GetString("[c/FFF261:启用]") : GetString("[c/F25C71:关闭]");
        var info3 = TimeRate.Config.One ? GetString("[c/55A2E0:启用]") : GetString("[c/F25C71:关闭]");

        if (args.Parameters.Count == 0)
        {
            if (args.Player != null)
            {
                args.Player.SendMessage(GetString("[c/EBE9A2:时间加速插件] [C/BFDFEA:by] [c/00FFFF:羽学][i:3459]\n") +
                    GetString("/times —— 时间加速指令菜单\n") +
                    GetString("/reload —— 重载配置文件\n") +
                    GetString("/times one —— 开启|关闭[c/55A2E0:单人睡觉]加速\n") +
                    GetString("/times all —— 开启|关闭[c/FFF261:全员睡觉]加速\n") +
                    GetString("/times on | off —— 开启|关闭[c/3ADB2C:指令]加速\n") +
                    GetString("/times set 数字 —— 设置速率|日晷:[c/F25C71:60] 正常:[c/55A2E0:1]"), 235, 220, 199);
                args.Player.SendMessage(GetString($"速率:[c/C9C7F5:{Terraria.Main.dayRate}] 指令:{info} 全员:{info2} 单人:{info3}"), 247, 244, 150);
            }
            return;
        }

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "on")
        {
            TimeRate.Config.Enabled = true;
            TimeRate.Config.Write();
            info = TimeRate.Config.Enabled ? GetString("启用") : GetString("禁用");
            args.Player.SendSuccessMessage(GetString("已成功[c/C9C7F5:{0}]时间流速! 当前速率为：[c/FFFCCF:{1}]"), info, TimeRate.Config.UpdateRate);
            return;
        }

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "off")
        {
            TimeRate.Config.Enabled = false;
            TimeRate.Config.Write();
            info = TimeRate.Config.Enabled ? GetString("启用") : GetString("禁用");
            args.Player.SendSuccessMessage(GetString("已[c/C9C7F5:{0}]时间流速!当前速率为：[c/FFFCCF:{1}]!"), info, Terraria.Main.dayRate);
            return;
        }

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "all")
        {
            var isSleep = TimeRate.Config.All;
            TimeRate.Config.All = !isSleep;
            TimeRate.Config.One = false;
            TimeRate.Config.Write();
            var Mess = isSleep ? GetString("禁用") : GetString("启用");
            args.Player.SendSuccessMessage(GetString("已[c/C9C7F5:{0}]全员睡觉加速!当前速率为：[c/FFFCCF:{1}]!"), Mess, Terraria.Main.dayRate);
            return;
        }

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "one")
        {
            var isSleep = TimeRate.Config.One;
            TimeRate.Config.One = !isSleep;
            TimeRate.Config.All = false;
            TimeRate.Config.Write();
            var Mess = isSleep ? GetString("禁用") : GetString("启用");
            args.Player.SendSuccessMessage(GetString("已[c/C9C7F5:{0}]单人睡觉加速!当前速率为：[c/FFFCCF:{1}]!"), Mess, Terraria.Main.dayRate);
            return;
        }

        if (args.Parameters.Count == 2)
        {
            switch (args.Parameters[0].ToLower())
            {
                case "s":
                case "set":
                {
                    if (int.TryParse(args.Parameters[1], out var num))
                    {
                        TimeRate.Config.UpdateRate = num;
                        TimeRate.Config.Write();
                        args.Player.SendSuccessMessage(GetString("已成功时间流速设置为: [c/C9C7F5:{0}] !"), num);
                    }
                    break;
                }
            }
        }
    }
}