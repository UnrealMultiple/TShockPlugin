using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ShortCommand;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public class CommandCD
    {
        public string Name { get; set; }

        public string Cmd { get; set; }

        public DateTime LastTime { get; set; }

        public CommandCD(string name, string cmd)
        {
            this.Name = name;
            this.Cmd = cmd;
            this.LastTime = DateTime.UtcNow;
        }
    }

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "GK 少司命修复版";

    public override Version Version => new Version(1, 3, 0, 5);

    public override string Description => GetString("由GK改良的简短指令插件！");

    private Config Config { get; set; }

    internal string PATH = Path.Combine(TShock.SavePath, "简短指令.json");

    private readonly Dictionary<string, CMD> ShortCmd = new();

    private readonly HashSet<string> NotSourceCmd = new();

    private List<CommandCD> CmdCD { get; set; }

    public Plugin(Main game)
        : base(game)
    {
        this.CmdCD = new List<CommandCD>();
        this.Config = new Config();
        this.Order += 1000000;
    }

    private void RC()
    {
        try
        {
            if (!File.Exists(this.PATH))
            {
                this.Config.Commands.Add(new CMD());
                TShock.Log.ConsoleError(GetString("未找到简短指令配置，已为您创建！"));
            }
            else
            {
                this.Config = Config.Read(this.PATH);
            }
            this.Config.Write(this.PATH);
            this.ShortCmd.Clear();
            this.NotSourceCmd.Clear();
            this.Config.Commands.ForEach(x =>
            {
                if (x != null)
                {
                    this.ShortCmd[x.NewCommand] = x;
                    if (x.NotSource && !string.IsNullOrEmpty(x.SourceCommand))
                    {
                        var cmdArge = x.SourceCommand.Split(' ');
                        if (cmdArge.Length > 0)
                        {
                            this.NotSourceCmd.Add(cmdArge[0]);
                        }
                    }
                }

            });
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"简短指令配置读取错误:{ex}"));
        }
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);
        PlayerHooks.PlayerCommand += this.OnChat;
        GeneralHooks.ReloadEvent += this.Reload;
    }

    private void Reload(ReloadEventArgs e)
    {
        this.RC();
        e.Player.SendSuccessMessage(GetString("简短指令重读成功!"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnInitialize);
            PlayerHooks.PlayerCommand -= this.OnChat;
            GeneralHooks.ReloadEvent -= this.Reload;
        }
        base.Dispose(disposing);
    }

    private void OnInitialize(EventArgs args)
    {
        this.RC();
    }

    private void OnChat(PlayerCommandEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        if (args.Player.HasPermission("免禁指令"))
        {
            return;
        }

        if (this.NotSourceCmd.Contains(args.CommandName))
        {
            args.Player.SendErrorMessage(GetString("该指令已被禁止使用!"));
            args.Handled = true;
            return;
        }
        if (this.ShortCmd.TryGetValue(args.CommandName, out var cmd) && cmd != null)
        {
            var sourcecmd = cmd.SourceCommand;
            var status = this.SR(ref sourcecmd, args.Player.Name, args.Parameters, cmd.Supplement);
            if (!status)
            {
                return;
            }

            if (args.Player.Index >= 0)
            {
                if (cmd.Condition == ConditionType.Alive && (args.Player.Dead || args.Player.TPlayer.statLife < 1))
                {
                    args.Player.SendErrorMessage(GetString("此指令要求你必须活着才能使用！"));
                    args.Handled = true;
                    return;
                }
                if (cmd.Condition == ConditionType.Death && (!args.Player.Dead || args.Player.TPlayer.statLife > 0))
                {
                    args.Player.SendErrorMessage(GetString("此指令要求你必须死亡才能使用！"));
                    args.Handled = true;
                    return;
                }
                var num = this.GetCD(args.Player.Name, cmd.SourceCommand, cmd.CD, cmd.ShareCD);
                if (num > 0)
                {
                    args.Player.SendErrorMessage(GetString("此指令正在冷却，还有{0}秒才能使用！"), num);
                    args.Handled = true;
                    return;
                }
                if (Commands.HandleCommand(args.Player, args.CommandPrefix + sourcecmd))
                {
                    if (!this.CmdCD.Exists((t) => t.Name == args.Player.Name && t.Cmd == cmd.NewCommand))
                    {
                        this.CmdCD.Add(new CommandCD(args.Player.Name, cmd.NewCommand));
                    }
                }
            }
            else
            {
                Commands.HandleCommand(args.Player, args.CommandPrefix + sourcecmd);
            }
            args.Handled = true;
        }
    }

    private int GetCD(string plyName, string Cmd, int CD, bool share)
    {
        for (var i = 0; i < this.CmdCD.Count; i++)
        {
            if (share)
            {
                if (this.CmdCD[i].Cmd != Cmd)
                {
                    continue;
                }
            }
            else if (this.CmdCD[i].Cmd != Cmd || this.CmdCD[i].Name != plyName)
            {
                continue;
            }
            else
            {
                var num = (int) (DateTime.UtcNow - this.CmdCD[i].LastTime).TotalSeconds;
                num = CD - num;
                if (num > 0)
                {
                    return num;
                }
                this.CmdCD.RemoveAt(i);
                return 0;
            }
        }
        return 0;
    }

    private bool SR(ref string cmd, string plyName, List<string> cmdArgs, bool Supplement)
    {
        var text = "";
        for (var i = 0; i < cmdArgs.Count; i++)
        {
            var text2 = "{" + i + "}";
            if (cmd.Contains(text2))
            {
                cmd = cmd.Replace(text2, cmdArgs[i]);
                continue;
            }
            if (Supplement)
            {
                text = $"{text} {cmdArgs[i]}";
                continue;
            }
            return false;
        }
        var text3 = "{player}";
        if (cmd.Contains(text3))
        {
            cmd = cmd.Replace(text3, plyName);
        }
        if (cmd.Contains("{") && cmd.Contains("}"))
        {
            return false;
        }
        if (Supplement)
        {
            cmd += text;
        }
        return true;
    }
}