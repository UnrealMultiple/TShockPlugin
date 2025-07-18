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
    private readonly Command _command = new(Help, "help");

    public HelpPlus(Main game)
        : base(game)
    {
        this.Order = int.MaxValue;
    }

    public override string Author => "Cai, 羽学";

    public override string Description => GetString("更好的Help");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new (2025, 7, 19, 0);

    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += GeneralHooks_ReloadEvent;
        Commands.ChatCommands.RemoveAll(x => x.Name == "help");
        Commands.ChatCommands.Add(this._command);
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
            args.Player.SendErrorMessage("无效用法。正确用法: {0}help <命令/页码>", specifier);
            return;
        }

        if (args.Parameters.Count == 0 || int.TryParse(args.Parameters[0], out var page))
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out page))
            {
                return;
            }

            var pageSize = Config.Settings.PageSize;
            
            var cmdNames = Commands.ChatCommands
                .Where(cmd => cmd.CanRun(args.Player) && (cmd.Name != "setup" || TShock.SetupToken != 0));

            var cmdNamesOrder = Config.Settings.OrderByLetter 
                ? cmdNames.OrderBy(cmd => cmd.Name).ToList() 
                : cmdNames.ToList();

            var count = cmdNamesOrder.Count;
            var pages = (int)Math.Ceiling(count / (double)pageSize);

            if (page > pages)
            {
                page = pages;
            }

            var start = (page - 1) * pageSize;
            var pagedCommands = cmdNamesOrder
                .Skip(start)
                .Take(pageSize)
                .Select(cmd => $"[c/60D6D0:{specifier}][c/F1D06C:{cmd.Name}]{GetShort(cmd.Name)}")
                .ToList();

            var stringBuilder = new StringBuilder();
            var currentLine = new StringBuilder();

            stringBuilder.AppendLine($"[c/FE727D:命令列表] ([c/68A7E8:{page}]/[c/EC6AC9:{pages}]):");
            
            foreach (var cmdWithSpace in pagedCommands.Select(cmd => $"{cmd} "))
            {
                if (currentLine.Length + cmdWithSpace.Length > Config.Settings.WithSize)
                {
                    stringBuilder.AppendLine(currentLine.ToString().Trim());
                    currentLine.Clear();
                }
                currentLine.Append(cmdWithSpace);
            }

            if (currentLine.Length > 0)
            {
                stringBuilder.AppendLine(currentLine.ToString().Trim());
            }
            
            if (page < pages)
            {
                stringBuilder.AppendLine($"请输入[c/68A7E8:/help {page + 1}]查看更多");
            }

            args.Player.SendMessage(stringBuilder.ToString(), 
                255, 244, 150);
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
                args.Player.SendErrorMessage("无效命令。");
                return;
            }

            if (!command.CanRun(args.Player))
            {
                args.Player.SendErrorMessage("你没有权限查询此命令。");
                return;
            }

            args.Player.SendSuccessMessage("{0}{1}的帮助:", specifier, command.Name);
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
                // ReSharper disable once StringLiteralTypo
                args.Player.SendInfoMessage($"别名: [c/00ffff:{string.Join(',', command.Names)}]");
            }

            args.Player.SendInfoMessage(
                $"权限: {(command.Permissions.Count == 0 || command.Permissions.Count(i => i == "") == command.Permissions.Count ? "[c/c2ff39:无权限限制]" : "[c/bf0705:" + string.Join(',', command.Permissions) + "]")}");
            args.Player.SendInfoMessage(
                $"来源插件: [c/8500ff:{command.CommandDelegate.Method.DeclaringType!.Assembly.FullName!.Split(',').First()}]");
            if (!command.AllowServer)
            {
                args.Player.SendInfoMessage("*此命令只能游戏内执行");
            }

            if (!command.DoLog)
            {
                args.Player.SendInfoMessage("*此命令不记录命令参数");
            }

            args.Player.SendInfoMessage("*本插件只能查询主命令权限，详细权限请使用/whynot查看!");
        }
    }

    private static string GetShort(string str)
    {
        return Config.Settings.DisPlayShort && Config.Settings.ShortCommands.TryGetValue(str, out var value)
            ? $"[c/FF5260:@]{value.Color(Utils.BoldHighlight)}"
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