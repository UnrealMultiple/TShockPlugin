using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace TeleportRequest;

[ApiVersion(2, 1)]
public class TeleportRequest : TerrariaPlugin
{
    private System.Timers.Timer Timer = null!;

    private readonly bool[] TPAllows = new bool[256];

    private readonly bool[] TPacpt = new bool[256];

    private readonly TPRequest[] TPRequests = new TPRequest[256];

    public override string Author => "原作者: MarioE, 修改者: Dr.Toxic，肝帝熙恩";

    public static Config tpConfig { get; set; } = null!;

    internal static string tpConfigPath => Path.Combine(TShock.SavePath, "tpconfig.json");

    public override string Description => GetString("传送前需要被传送者接受或拒绝请求");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 4);

    public TeleportRequest(Main game)
        : base(game)
    {
        tpConfig = new Config();
        for (var i = 0; i < this.TPRequests.Length; i++)
        {
            this.TPRequests[i] = new TPRequest();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, this.OnInitialize);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            GeneralHooks.ReloadEvent += this.ReloadTPR;

            this.Timer.Dispose();
        }
    }

    public override void Initialize()
    {
        ServerApi.Hooks.GameInitialize.Register(this, this.OnInitialize);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        GeneralHooks.ReloadEvent -= this.ReloadTPR;
    }

    private void OnElapsed(object? sender, ElapsedEventArgs e)
    {
        for (var i = 0; i < this.TPRequests.Length; i++)
        {
            var tPRequest = this.TPRequests[i];
            if (tPRequest.timeout <= 0)
            {
                continue;
            }
            var tSPlayer = TShock.Players[tPRequest.dst];
            var tSPlayer2 = TShock.Players[i];
            tPRequest.timeout--;
            if (tPRequest.timeout == 0)
            {
                tSPlayer2.SendErrorMessage(GetString("传送请求已超时."));
                tSPlayer.SendInfoMessage(GetString("玩家[{0}]的传送请求已超时.", tSPlayer2.Name));
                continue;
            }
            var format = GetString("玩家[{{0}}]要求传送到你当前位置. ({0}接受tp ({0}atp) 或 {0}拒绝tp ({0}dtp))", Commands.Specifier);
            if (tPRequest.dir)
            {
                format = GetString("你被请求传送到玩家[{{0}}]的当前位置. ({0}接受tp ({0}atp) 或 {0}拒绝tp ({0}dtp))", Commands.Specifier);
            }
            tSPlayer.SendInfoMessage(format, tSPlayer2.Name);
        }
    }

    private void OnInitialize(EventArgs e)
    {
        Commands.ChatCommands.Add(new Command("tprequest.gettpr", this.TPAccept, "接受tp", "atp")
        {
            AllowServer = false,
            HelpText = GetString("接受传送请求.")
        });
        Commands.ChatCommands.Add(new Command("tprequest.tpauto", this.TPAutoDeny, "自动拒绝tp", "autodeny")
        {
            AllowServer = false,
            HelpText = GetString("自动拒绝所有人的传送请求.")
        });
        Commands.ChatCommands.Add(new Command("tprequest.tpauto", this.TPAutoAccept, "自动接受tp", "autoaccept")
        {
            AllowServer = false,
            HelpText = GetString("自动接受所有人的传送请求.")
        });
        Commands.ChatCommands.Add(new Command("tprequest.gettpr", this.TPDeny, "拒绝tp", "dtp")
        {
            AllowServer = false,
            HelpText = GetString("拒绝传送请求.")
        });
        Commands.ChatCommands.Add(new Command("tprequest.tpat", this.TPAHere, "tpahere")
        {
            AllowServer = false,
            HelpText = GetString("发出把指定玩家传送到你当前位置的请求.")
        });
        Commands.ChatCommands.Add(new Command("tprequest.tpat", this.TPA, "tpa")
        {
            AllowServer = false,
            HelpText = GetString("发出传送到指定玩家当前位置的请求.")
        });
        this.SetupConfig();
        this.Timer = new System.Timers.Timer(tpConfig.IntervalInSeconds * 1000);
        this.Timer.Elapsed += this.OnElapsed;
        this.Timer.Start();
    }

    private void OnLeave(LeaveEventArgs e)
    {
        this.TPAllows[e.Who] = false;
        this.TPacpt[e.Who] = false;
        this.TPRequests[e.Who].timeout = 0;
    }

    private void TPA(CommandArgs e)
    {
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage(GetString("格式错误! 正确格式为: {0}tpa <玩家>", Commands.Specifier));
            return;
        }
        var search = string.Join(" ", e.Parameters.ToArray());
        var list = TSPlayer.FindByNameOrID(search);
        if (list.Count == 0)
        {
            e.Player.SendErrorMessage(GetString("找不到这位玩家!"));
            return;
        }
        if (list.Count > 1)
        {
            e.Player.SendErrorMessage(GetString("匹对到多于一位玩家!"));
            return;
        }
        if (list[0].Equals(e.Player))
        {
            e.Player.SendErrorMessage(GetString("禁止向自己发送传送请求！"));
            return;
        }
        if ((!list[0].TPAllow || this.TPAllows[list[0].Index]) && !e.Player.Group.HasPermission(Permissions.tpoverride))
        {
            e.Player.SendErrorMessage(GetString("你无法传送到玩家[{0}].", list[0].Name));
            return;
        }
        if ((list[0].TPAllow && this.TPacpt[list[0].Index]) || e.Player.Group.HasPermission(Permissions.tpoverride))
        {
            var flag = false;
            var tSPlayer = flag ? TShock.Players[list[0].Index] : e.Player;
            var tSPlayer2 = flag ? e.Player : TShock.Players[list[0].Index];
            if (tSPlayer.Teleport(tSPlayer2.X, tSPlayer2.Y, 1))
            {
                tSPlayer.SendSuccessMessage(GetString("已经传送到玩家[{0}]的当前位置.", tSPlayer2.Name));
                tSPlayer2.SendSuccessMessage(GetString("玩家[{0}]已传送到你的当前位置.", tSPlayer.Name));
            }
            return;
        }
        for (var i = 0; i < this.TPRequests.Length; i++)
        {
            var tPRequest = this.TPRequests[i];
            if (tPRequest.timeout > 0 && tPRequest.dst == list[0].Index)
            {
                e.Player.SendErrorMessage(GetString("玩家[{0}]已被其他玩家发出传送请求.", list[0].Name));
                return;
            }
        }
        this.TPRequests[e.Player.Index].dir = false;
        this.TPRequests[e.Player.Index].dst = (byte) list[0].Index;
        this.TPRequests[e.Player.Index].timeout = tpConfig.TimeoutCount + 1;
        e.Player.SendSuccessMessage(GetString("已成功向玩家[{0}]发出传送请求.", list[0].Name));
    }

    private void TPAccept(CommandArgs e)
    {
        for (var i = 0; i < this.TPRequests.Length; i++)
        {
            var tPRequest = this.TPRequests[i];
            if (tPRequest.timeout > 0 && tPRequest.dst == e.Player.Index)
            {
                var tSPlayer = tPRequest.dir ? e.Player : TShock.Players[i];
                var tSPlayer2 = tPRequest.dir ? TShock.Players[i] : e.Player;
                if (tSPlayer.Teleport(tSPlayer2.X, tSPlayer2.Y, 1))
                {
                    tSPlayer.SendSuccessMessage(GetString("已经传送到玩家[{0}]的当前位置.", tSPlayer2.Name));
                    tSPlayer2.SendSuccessMessage(GetString("玩家[{0}]已传送到你的当前位置.", tSPlayer.Name));
                }
                tPRequest.timeout = 0;
                return;
            }
        }
        e.Player.SendErrorMessage(GetString("你暂时没有收到其他玩家的传送请求."));
    }

    private void TPAHere(CommandArgs e)
    {
        if (e.Parameters.Count == 0)
        {
            e.Player.SendErrorMessage(GetString("格式错误! 正确格式为: {0}tpahere <玩家>", Commands.Specifier));
            return;
        }
        var search = string.Join(" ", e.Parameters.ToArray());
        var list = TSPlayer.FindByNameOrID(search);
        if (list.Count == 0)
        {
            e.Player.SendErrorMessage(GetString("找不到这位玩家!"));
            return;
        }
        if (list.Count > 1)
        {
            e.Player.SendErrorMessage(GetString("匹对到多于一位玩家!"));
            return;
        }
        if ((!list[0].TPAllow || this.TPAllows[list[0].Index]) && !e.Player.Group.HasPermission(Permissions.tpoverride))
        {
            e.Player.SendErrorMessage(GetString("你无法传送到玩家[{0}]."), list[0].Name);
            return;
        }
        if ((list[0].TPAllow && this.TPacpt[list[0].Index]) || e.Player.Group.HasPermission(Permissions.tpoverride))
        {
            var flag = true;
            var tSPlayer = flag ? TShock.Players[list[0].Index] : e.Player;
            var tSPlayer2 = flag ? e.Player : TShock.Players[list[0].Index];
            if (tSPlayer.Teleport(tSPlayer2.X, tSPlayer2.Y, 1))
            {
                tSPlayer.SendSuccessMessage(GetString("已经传送到玩家[{0}]的当前位置."), tSPlayer2.Name);
                tSPlayer2.SendSuccessMessage(GetString("玩家[{0}]已传送到你的当前位置."), tSPlayer.Name);
            }
            return;
        }
        for (var i = 0; i < this.TPRequests.Length; i++)
        {
            var tPRequest = this.TPRequests[i];
            if (tPRequest.timeout > 0 && tPRequest.dst == list[0].Index)
            {
                e.Player.SendErrorMessage(GetString("玩家[{0}]已被其他玩家发出传送请求."), list[0].Name);
                return;
            }
        }
        this.TPRequests[e.Player.Index].dir = true;
        this.TPRequests[e.Player.Index].dst = (byte) list[0].Index;
        this.TPRequests[e.Player.Index].timeout = tpConfig.TimeoutCount + 1;
        e.Player.SendSuccessMessage(GetString("已成功向玩家[{0}]发出传送请求."), list[0].Name);
    }

    private void TPAutoDeny(CommandArgs e)
    {
        if (this.TPacpt[e.Player.Index])
        {
            e.Player.SendErrorMessage(GetString("请先解除自动接受传送"));
            return;
        }
        this.TPAllows[e.Player.Index] = !this.TPAllows[e.Player.Index];
        e.Player.SendInfoMessage(GetString("{0}自动拒绝传送请求.", this.TPAllows[e.Player.Index] ? GetString("启用") : GetString("解除")));
    }

    private void TPDeny(CommandArgs e)
    {
        for (var i = 0; i < this.TPRequests.Length; i++)
        {
            var tPRequest = this.TPRequests[i];
            if (tPRequest.timeout > 0 && tPRequest.dst == e.Player.Index)
            {
                e.Player.SendSuccessMessage(GetString("已拒绝玩家[{0}]的传送请求.", TShock.Players[i].Name));
                TShock.Players[i].SendErrorMessage(GetString("玩家[{0}]拒绝你的传送请求.", e.Player.Name));
                tPRequest.timeout = 0;
                return;
            }
        }
        e.Player.SendErrorMessage(GetString("你暂时没有收到其他玩家的传送请求."));
    }

    private void TPAutoAccept(CommandArgs e)
    {
        if (this.TPAllows[e.Player.Index])
        {
            e.Player.SendErrorMessage(GetString("请先解除自动拒绝传送"));
            return;
        }
        this.TPacpt[e.Player.Index] = !this.TPacpt[e.Player.Index];
        e.Player.SendInfoMessage(GetString("{0}自动接受传送请求.", this.TPacpt[e.Player.Index] ? GetString("启用") : GetString("解除")));
    }

    private void SetupConfig()
    {
        try
        {
            if (File.Exists(tpConfigPath))
            {
                tpConfig = Config.Read(tpConfigPath);
            }
            tpConfig.Write(tpConfigPath);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString("TPR配置出现异常"));
            TShock.Log.ConsoleError(ex.ToString());
        }
    }

    private void ReloadTPR(ReloadEventArgs args)
    {
        this.SetupConfig();
        args.Player?.SendSuccessMessage(GetString("[TeleportRequest] 重新加载配置完毕。"));
    }
}