using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Terraria.DataStructures;
using IL.Terraria.ID;
using IL.Terraria.Chat.Commands;
using Microsoft.Xna.Framework;
using System.Timers;
using On.OTAPI;
using static MonoMod.InlineRT.MonoModRule;
using MonoMod.RuntimeDetour;
using Terraria.Localization;
using System.Diagnostics;
using Terraria.Chat;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using MySqlX.XDevAPI;
using TShockAPI.Hooks;

namespace UserCheck
{
    [ApiVersion(2, 1)]
    public class HelpPlus : TerrariaPlugin
    {

        public override string Author => "Cai";

        public override string Description => "更好的Help";

        public override string Name => "Help+(更好的Help)";
        public override Version Version => new Version(1, 0, 0, 0);

        public HelpPlus(Main game)
        : base(game)
        {
            Order = int.MaxValue;
        }
        Command Command = new Command(Help, "help");
        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += GeneralHooks_ReloadEvent;
            On.OTAPI.Hooks.MessageBuffer.InvokeGetData += MessageBuffer_InvokeGetData;
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
        private static List<String> ParseParameters(string str)
        {
            var ret = new List<string>();
            var sb = new StringBuilder();
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
                    sb.Append(c);
            }
            if (sb.Length > 0)
                ret.Add(sb.ToString());

            return ret;
        }
        private bool MessageBuffer_InvokeGetData(Hooks.MessageBuffer.orig_InvokeGetData orig, MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length, ref int messageType, int maxPackets)
        {
            //Console.WriteLine(1);
            if (messageType == 82)
            {

                instance.ResetReader();
                instance.reader.BaseStream.Position = start + 1;

                ushort moduleId = instance.reader.ReadUInt16();
                //LoadNetModule is now used for sending chat text.
                //Read the module ID to determine if this is in fact the text module
                if (moduleId == Terraria.Net.NetManager.Instance.GetId<Terraria.GameContent.NetModules.NetTextModule>())
                {
                    //Then deserialize the message from the reader
                    Terraria.Chat.ChatMessage msg = Terraria.Chat.ChatMessage.Deserialize(instance.reader);
                    if (msg.CommandId._name != "Help")
                    {
                        return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
                    }
                    TSPlayer player = TShock.Players[instance.whoAmI];
                    string text = "/help "+ msg.Text;
                    string Specifier = TShock.Config.Settings.CommandSpecifier;
                    string cmdText = text.Remove(0, 1);
                    string cmdPrefix = text[0].ToString();
                    int index = -1;
                    for (int i = 0; i < cmdText.Length; i++)
                    {
                        if (IsWhiteSpace(cmdText[i]))
                        {
                            index = i;
                            break;
                        }
                    }
                    //player.Account.VerifyPassword("114514");
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
                        TShock.Utils.SendLogs($"{player.Name}执行了/{cmdText}。" ,Color.PaleVioletRed, player);
                        return false;
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
                if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out pageNumber))
                {
                    return;
                }

                IEnumerable<string> cmdNames = from cmd in TShockAPI.Commands.ChatCommands
                                               where cmd.CanRun(args.Player) && (cmd.Name != "setup" || TShock.SetupToken != 0)
                                               select Specifier + cmd.Name+GetShort(cmd.Name);

                PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(cmdNames),
                    new PaginationTools.Settings
                    {
                        HeaderFormat = string.Format("命令列表 ({{0}}/{{1}}):"),
                        FooterFormat = string.Format("输入 {0}help {{0}} 翻页.", Specifier)
                    });
            }
            else
            {
                string commandName = args.Parameters[0].ToLower();
                if (commandName.StartsWith(Specifier))
                {
                    commandName = commandName.Substring(1);
                }

                Command command = TShockAPI.Commands.ChatCommands.Find(c => c.Names.Contains(commandName));
                if (command == null)
                {
                    args.Player.SendErrorMessage(string.Format("无效命令."));
                    return;
                }
                if (!command.CanRun(args.Player))
                {
                    args.Player.SendErrorMessage(string.Format("你没有权限查询此命令."));
                    return;
                }

                args.Player.SendSuccessMessage(string.Format("{0}{1}的帮助:", Specifier, command.Name));
                if (command.HelpDesc == null)
                {
                    args.Player.SendWarningMessage(command.HelpText);
                    
                }
                else
                {
                    foreach (string line in command.HelpDesc)
                    {
                        args.Player.SendInfoMessage(line);
                    }
                }
                if (command.Names.Count > 1)
                    args.Player.SendInfoMessage($"别名: [c/00ffff:{string.Join(',', command.Names)}]");
                args.Player.SendInfoMessage($"权限: {(command.Permissions.Count == 0||command.Permissions.Count(i=>i=="")==command.Permissions.Count ? "[c/c2ff39:无权限限制]" : "[c/bf0705:"+string.Join(',', command.Permissions)+"]")}");
                args.Player.SendInfoMessage($"来源插件: [c/8500ff:{command.CommandDelegate.Method.DeclaringType.Assembly.FullName.Split(',').First()}]");
                if (!command.AllowServer)
                    args.Player.SendInfoMessage("*此命令只能游戏内执行");
                if (!command.DoLog)
                    args.Player.SendInfoMessage("*此命令不记录命令参数");
            }
        }

        public static string GetShort(string str)
        {
            if (Config.config.DisPlayShort && Config.config.ShortCommands.ContainsKey(str))
            {
                return $"({Config.config.ShortCommands[str].Color(TShockAPI.Utils.BoldHighlight)})";
   
            }
            else
            {
                return "";
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                On.OTAPI.Hooks.MessageBuffer.InvokeGetData -= MessageBuffer_InvokeGetData;
                GeneralHooks.ReloadEvent-= GeneralHooks_ReloadEvent;
            }
            base.Dispose(disposing);
        }


    }
}
