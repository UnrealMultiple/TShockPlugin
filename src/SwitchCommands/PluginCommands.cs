using Microsoft.Xna.Framework;
//using PlaceholderAPI;
using TShockAPI;

namespace SwitchCommands;

public class PluginCommands
{
    public static Database database = null!;
    public static string switchParameters = "/开关 <添加/列表/删除/冷却/权限忽略/取消/重绑/完成>（拼音首字母也可以）\n/switch <add/list/del/cooldown/ignoreperms/cancel/rebind/done>";

    public static void RegisterCommands()
    {
        Commands.ChatCommands.Add(new Command("switch.admin", SwitchCmd, "开关", "kg", "switch"));
    }

    public static void UnregisterCommands()
    {
        Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == SwitchCmd);
    }

    private static void SwitchCmd(CommandArgs args)
    {
        var player = args.Player;

        switch (player.GetData<PlayerState>("PlayerState"))
        {
            case PlayerState.None:
                player.SendSuccessMessage(GetString("激活一个开关以将其绑定,之后可输入/开关 ，查看子命令"));
                player.SetData("PlayerState", PlayerState.SelectingSwitch);
                return;

            case PlayerState.AddingCommands:
                if (args.Parameters.Count == 0)
                {
                    player.SendErrorMessage(GetString("正确指令："));
                    player.SendErrorMessage(switchParameters);
                    return;
                }

                if (player.GetData<CommandInfo>("CommandInfo") == null)
                {
                    player.SetData("CommandInfo", new CommandInfo());
                }

                var cmdInfo = player.GetData<CommandInfo>("CommandInfo");

                switch (args.Parameters[0].ToLower())
                {
                    case "add":
                    case "添加":
                    case "tj":
                        var command = "/" + string.Join(" ", args.Parameters.Skip(1));
                        cmdInfo.commandList.Add(command);
                        player.SendSuccessMessage(GetString("成功添加: {0}").SFormat(command));
                        SwitchCommands.database.Write(Database.databasePath);
                        break;

                    case "list":
                    case "列表":
                    case "lb":
                        player.SendMessage(GetString("当前开关绑定的指令:"), Color.Green);
                        for (var x = 0; x < cmdInfo.commandList.Count; x++)
                        {
                            player.SendMessage("({0}) ".SFormat(x) + cmdInfo.commandList[x], Color.Yellow);
                            SwitchCommands.database.Write(Database.databasePath);
                        }
                        break;

                    case "del":
                    case "删除":
                    case "sc":
                        var commandIndex = 0;

                        if (args.Parameters.Count < 2 || !int.TryParse(args.Parameters[1], out commandIndex))
                        {
                            player.SendErrorMessage(GetString("语法错误：/开关 del <指令>"));
                            SwitchCommands.database.Write(Database.databasePath);
                            return;
                        }

                        var cmdDeleted = cmdInfo.commandList[commandIndex];
                        cmdInfo.commandList.RemoveAt(commandIndex);

                        player.SendSuccessMessage(GetString("成功删除了第{1}条指令：{0}。").SFormat(cmdDeleted, commandIndex));
                        SwitchCommands.database.Write(Database.databasePath);
                        break;

                    case "冷却":
                    case "cooldown":
                    case "lq":
                        float cooldown = 0;

                        if (args.Parameters.Count < 2 || !float.TryParse(args.Parameters[1], out cooldown))
                        {
                            player.SendErrorMessage(GetString("语法错误：/开关 冷却 <秒>"));
                            SwitchCommands.database.Write(Database.databasePath);
                            return;
                        }

                        cmdInfo.cooldown = cooldown;

                        player.SendSuccessMessage(GetString("冷却时间已设置为 {0} 秒").SFormat(cooldown));
                        SwitchCommands.database.Write(Database.databasePath);
                        break;

                    case "说明":
                    case "sm":
                        if (args.Parameters.Count < 2)
                        {
                            player.SendErrorMessage(GetString("语法错误：/开关 说明 <内容>"));
                            SwitchCommands.database.Write(Database.databasePath);
                            return;
                        }
                        var shows = args.Parameters[1];

                        cmdInfo.show = shows;

                        player.SendSuccessMessage(GetString($"开关说明已设置为：{shows}"));
                        SwitchCommands.database.Write(Database.databasePath);
                        break;

                    case "权限忽略":
                    case "ignoreperms":
                    case "qxhl":
                    case "hl":
                        var ignoreperms = false;
                        if (!args.Player.HasPermission("switch.ignoreperms"))//权限控制
                        {
                            player.SendErrorMessage(GetString("你没有权限使用此命令"));
                            return;
                        }
                        if (args.Parameters.Count < 2)
                        {
                            // 如果没有提供第二个参数，默认设置为true
                            ignoreperms = true;
                        }
                        else if (!bool.TryParse(args.Parameters[1], out ignoreperms))
                        {
                            player.SendErrorMessage(GetString("语法错误：/开关 权限忽略 <true/false>"));
                            SwitchCommands.database.Write(Database.databasePath);
                            return;
                        }

                        cmdInfo.ignorePerms = ignoreperms;

                        var statusMessage = ignoreperms ? GetString("是") : GetString("否");
                        player.SendSuccessMessage(GetString("是否忽略玩家权限设置为: {0}.").SFormat(statusMessage));
                        SwitchCommands.database.Write(Database.databasePath);
                        break;

                    case "取消":
                    case "cancel":
                    case "qx":
                        player.SetData("PlayerState", PlayerState.None);
                        player.SetData("CommandInfo", new CommandInfo());
                        player.SendSuccessMessage(GetString("已取消添加要添加的命令"));
                        SwitchCommands.database.Write(Database.databasePath);
                        return;

                    case "重绑":
                    case "rebind":
                    case "zb":
                    case "cb":
                        player.SendSuccessMessage(GetString("重新激活开关后可以重新绑定"));
                        player.SetData("PlayerState", PlayerState.SelectingSwitch);
                        SwitchCommands.database.Write(Database.databasePath);
                        return;

                    case "完成":
                    case "done":
                    case "wc":
                        var switchPos = player.GetData<SwitchPos>("SwitchPos");

                        player.SendSuccessMessage(GetString("设置成功的开关位于 X： {0}， Y： {1}").SFormat(switchPos.X, switchPos.Y));
                        foreach (var cmd in cmdInfo.commandList)
                        {
                            player.SendMessage(cmd, Color.Yellow);
                            SwitchCommands.database.Write(Database.databasePath);
                        }
                        SwitchCommands.database.switchCommandList[player.GetData<SwitchPos>("SwitchPos").ToString()] = cmdInfo;
                        player.SetData("PlayerState", PlayerState.None);
                        player.SetData("SwitchPos", new Vector2());
                        player.SetData("CommandInfo", new CommandInfo());
                        SwitchCommands.database.Write(Database.databasePath);
                        return;

                    default:
                        player.SendErrorMessage(GetString($"语法无效. {switchParameters}"));
                        return;
                }

                player.SetData("CommandInfo", cmdInfo);

                return;
        }
    }

    public enum PlayerState
    {
        None,
        AddingCommands,
        SelectingSwitch
    }
}