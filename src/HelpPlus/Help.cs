using Microsoft.Xna.Framework;
using On.OTAPI;
using System.Text;
using Terraria;
using Terraria.Chat;
using Terraria.Chat.Commands;
using Terraria.GameContent.NetModules;
using Terraria.Net;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Utils = TShockAPI.Utils;

namespace UserCheck;

[ApiVersion(2, 1)]
public class HelpPlus : TerrariaPlugin
{
    private readonly Command Command = new(Help, "help");

    public HelpPlus(Main game)
        : base(game)
    {
        this.Order = int.MaxValue;
    }

    public override string Author => "Cai";

    public override string Description => GetString("更好的Help");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new (2025, 05, 18, 5);

    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += GeneralHooks_ReloadEvent;
        Commands.ChatCommands.RemoveAll(x => x.Name == "help");
        Commands.ChatCommands.Add(this.Command);
        Config.Read();
    }

    private static void GeneralHooks_ReloadEvent(ReloadEventArgs e)
    {
        Config.Read();
        e.Player.SendSuccessMessage(GetString("[HelpPlus]插件配置已重载！"));
    }

    private static void Help(CommandArgs args)
    {
        var specifier = TShock.Config.Settings.CommandSpecifier;
        if (args.Parameters.Count > 1)
        {
            args.Player.SendErrorMessage(GetString("无效用法.正确用法: {0}help <命令/页码>", specifier));
            return;
        }

        if (args.Parameters.Count == 0 || int.TryParse(args.Parameters[0], out var pageNumber))
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out pageNumber))
            {
                return;
            }

            var cmdNames = from cmd in Commands.ChatCommands
                           where cmd.CanRun(args.Player) && (cmd.Name != "setup" || TShock.SetupToken != 0)
                           select specifier + cmd.Name + GetShort(cmd.Name);

            PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(cmdNames),
                new PaginationTools.Settings
                {
                    HeaderFormat = GetString("命令列表 ({0}/{1}):"),
                    FooterFormat = GetString("输入 {0}help {{0}} 翻页.", specifier)
                });
        }
        else
        {
            var commandName = args.Parameters[0].ToLower();
            if (commandName.StartsWith(specifier))
            {
                commandName = commandName[1..];
            }

            var command = Commands.ChatCommands.Find(c => c.Names.Contains(commandName));
            if (command == null)
            {
                args.Player.SendErrorMessage(GetString("无效命令."));
                return;
            }

            if (!command.CanRun(args.Player))
            {
                args.Player.SendErrorMessage(GetString("你没有权限查询此命令."));
                return;
            }

            args.Player.SendSuccessMessage(GetString("{0}{1}的帮助:", specifier, command.Name));
            if (command.HelpDesc == null)
            {
                args.Player.SendWarningMessage(command.HelpText);
            }
            else
            {
                foreach (var line in command.HelpDesc)
                {
                    args.Player.SendInfoMessage(line);
                }
            }

            if (command.Names.Count > 1)
            {
                args.Player.SendInfoMessage(GetString($"别名: [c/00ffff:{string.Join(',', command.Names)}]"));
            }

            args.Player.SendInfoMessage(
                GetString($"权限: {(command.Permissions.Count == 0 || command.Permissions.Count(i => i == "") == command.Permissions.Count ? GetString("[c/c2ff39:无权限限制]") : $"[c/bf0705:{string.Join(',', command.Permissions)}]")}"));
            args.Player.SendInfoMessage(
                GetString($"来源插件: [c/8500ff:{command.CommandDelegate.Method.DeclaringType!.Assembly.FullName!.Split(',').First()}]"));
            if (!command.AllowServer)
            {
                args.Player.SendInfoMessage(GetString("*此命令只能游戏内执行"));
            }

            if (!command.DoLog)
            {
                args.Player.SendInfoMessage(GetString("*此命令不记录命令参数"));
            }

            args.Player.SendInfoMessage(GetString("*本插件只能查询主命令权限，详细权限请使用/whynot查看!"));
        }
    }

    public static string GetShort(string str)
    {
        return Config.config.DisPlayShort && Config.config.ShortCommands.ContainsKey(str)
            ? $"({Config.config.ShortCommands[str].Color(Utils.BoldHighlight)})"
            : "";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= GeneralHooks_ReloadEvent;
        }

        base.Dispose(disposing);
    }
}