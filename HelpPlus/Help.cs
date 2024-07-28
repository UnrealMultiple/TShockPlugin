using System.Text;
using Microsoft.Xna.Framework;
using On.OTAPI;
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
        Order = int.MaxValue;
    }

    public override string Author => "Cai";

    public override string Description => "更好的Help";

    public override string Name => "Help+(更好的Help)";
    public override Version Version => new(2024, 7, 28, 1);

    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += GeneralHooks_ReloadEvent;
        Hooks.MessageBuffer.InvokeGetData += MessageBuffer_InvokeGetData;
        Commands.ChatCommands.RemoveAll(x => x.Name == "help");
        Commands.ChatCommands.Add(Command);
        Config.Read();
    }

    private void GeneralHooks_ReloadEvent(ReloadEventArgs e)
    {
        Config.Read();
        e.Player.SendSuccessMessage("[HelpPlus]插件配置已重载！");
    }

    private static bool IsWhiteSpace(char c)
    {
        return c == ' ' || c == '\t' || c == '\n';
    }

    private static List<string> ParseParameters(string str)
    {
        List<string> ret = new List<string>();
        StringBuilder sb = new StringBuilder();
        bool instr = false;
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];

            if (c == '\\' && ++i < str.Length)
            {
                if (str[i] != '"' && str[i] != ' ' && str[i] != '\\')
                    sb.Append('\\');
                sb.Append(str[i]);
            }
            else if (c == '"')
            {
                instr = !instr;
                if (!instr)
                {
                    ret.Add(sb.ToString());
                    sb.Clear();
                }
                else if (sb.Length > 0)
                {
                    ret.Add(sb.ToString());
                    sb.Clear();
                }
            }
            else if (IsWhiteSpace(c) && !instr)
            {
                if (sb.Length > 0)
                {
                    ret.Add(sb.ToString());
                    sb.Clear();
                }
            }
            else
            {
                sb.Append(c);
            }
        }

        if (sb.Length > 0)
            ret.Add(sb.ToString());

        return ret;
    }

    private bool MessageBuffer_InvokeGetData(Hooks.MessageBuffer.orig_InvokeGetData orig, MessageBuffer instance,
        ref byte packetId, ref int readOffset, ref int start, ref int length, ref int messageType, int maxPackets)
    {
        if (messageType == 82)
        {
            instance.ResetReader();
            instance.reader.BaseStream.Position = start + 1;

            ushort moduleId = instance.reader.ReadUInt16();
            if (moduleId == NetManager.Instance.GetId<NetTextModule>())
            {
                ChatMessage msg = ChatMessage.Deserialize(instance.reader);
                switch (msg.CommandId._name)
                {
                    case "Help":
                        TSPlayer player = TShock.Players[instance.whoAmI];
                        string text = "/help " + msg.Text;
                        string cmdText = text.Remove(0, 1);
                        int index = -1;
                        for (int i = 0; i < cmdText.Length; i++)
                            if (IsWhiteSpace(cmdText[i]))
                            {
                                index = i;
                                break;
                            }

                        string cmdName;
                        if (index < 0)
                            cmdName = cmdText.ToLower();
                        else
                            cmdName = cmdText.Substring(0, index).ToLower();

                        List<string> args;
                        if (index < 0)
                            args = new List<string>();
                        else
                            args = ParseParameters(cmdText.Substring(index));
                        if (cmdName == "help")
                        {
                            Help(new CommandArgs(null, false, player, args));
                            TShock.Utils.SendLogs($"{player.Name}执行了/{cmdText}。", Color.PaleVioletRed, player);
                            return false;
                        }
                        break;
                    case "AllDeath":
                        AllDeathCommand allDeathCommand = new AllDeathCommand();
                        allDeathCommand.ProcessIncomingMessage("", (byte)instance.whoAmI);
                        return false;
                    case "AllPVPDeath":
                        AllPVPDeathCommand allPvpDeathCommand = new AllPVPDeathCommand();
                        allPvpDeathCommand.ProcessIncomingMessage("", (byte)instance.whoAmI);
                        return false;
                    case "Death":
                        DeathCommand deathCommand = new DeathCommand();
                        deathCommand.ProcessIncomingMessage("", (byte)instance.whoAmI);
                        return false;
                    case "PVPDeath":
                        PVPDeathCommand pvpDeathCommand = new PVPDeathCommand();
                        pvpDeathCommand.ProcessIncomingMessage("", (byte)instance.whoAmI);
                        return false;
                    case "Roll":
                        RollCommand rollCommand = new RollCommand();
                        rollCommand.ProcessIncomingMessage("", (byte)instance.whoAmI);
                        return false;
                    default:
                        return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType,
                            maxPackets);
                }
            }
        }


        return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
    }

    private static void Help(CommandArgs args)
    {
        string Specifier = TShock.Config.Settings.CommandSpecifier;
        if (args.Parameters.Count > 1)
        {
            args.Player.SendErrorMessage(string.Format("无效用法.正确用法: {0}help <命令/页码>", Specifier));
            return;
        }

        int pageNumber;
        if (args.Parameters.Count == 0 || int.TryParse(args.Parameters[0], out pageNumber))
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out pageNumber)) return;

            IEnumerable<string> cmdNames = from cmd in Commands.ChatCommands
                where cmd.CanRun(args.Player) && (cmd.Name != "setup" || TShock.SetupToken != 0)
                select Specifier + cmd.Name + GetShort(cmd.Name);

            PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(cmdNames),
                new PaginationTools.Settings
                {
                    HeaderFormat = "命令列表 ({0}/{1}):",
                    FooterFormat = string.Format("输入 {0}help {{0}} 翻页.", Specifier)
                });
        }
        else
        {
            string commandName = args.Parameters[0].ToLower();
            if (commandName.StartsWith(Specifier)) commandName = commandName.Substring(1);

            Command? command = Commands.ChatCommands.Find(c => c.Names.Contains(commandName));
            if (command == null)
            {
                args.Player.SendErrorMessage("无效命令.");
                return;
            }

            if (!command.CanRun(args.Player))
            {
                args.Player.SendErrorMessage("你没有权限查询此命令.");
                return;
            }

            args.Player.SendSuccessMessage(string.Format("{0}{1}的帮助:", Specifier, command.Name));
            if (command.HelpDesc == null)
                args.Player.SendWarningMessage(command.HelpText);
            else
                foreach (string line in command.HelpDesc)
                    args.Player.SendInfoMessage(line);
            if (command.Names.Count > 1)
                args.Player.SendInfoMessage($"别名: [c/00ffff:{string.Join(',', command.Names)}]");
            args.Player.SendInfoMessage(
                $"权限: {(command.Permissions.Count == 0 || command.Permissions.Count(i => i == "") == command.Permissions.Count ? "[c/c2ff39:无权限限制]" : "[c/bf0705:" + string.Join(',', command.Permissions) + "]")}");
            args.Player.SendInfoMessage(
                $"来源插件: [c/8500ff:{command.CommandDelegate.Method.DeclaringType.Assembly.FullName.Split(',').First()}]");
            if (!command.AllowServer)
                args.Player.SendInfoMessage("*此命令只能游戏内执行");
            if (!command.DoLog)
                args.Player.SendInfoMessage("*此命令不记录命令参数");
            args.Player.SendInfoMessage("*本插件只能查询主命令权限，详细权限请使用/whynot查看!");
        }
    }

    public static string GetShort(string str)
    {
        if (Config.config.DisPlayShort && Config.config.ShortCommands.ContainsKey(str))
            return $"({Config.config.ShortCommands[str].Color(Utils.BoldHighlight)})";
        return "";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Hooks.MessageBuffer.InvokeGetData -= MessageBuffer_InvokeGetData;
            GeneralHooks.ReloadEvent -= GeneralHooks_ReloadEvent;
        }

        base.Dispose(disposing);
    }
}