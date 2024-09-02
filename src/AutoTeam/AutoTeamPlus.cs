using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static TShockAPI.GetDataHandlers;

namespace autoteam;

[ApiVersion(2, 1)]
public class Autoteam : TerrariaPlugin
{
    public override string Author => "十七改，肝帝熙恩改";
    public override Version Version => new Version(2, 4, 1);
    public override string Description => "自动队伍";
    public override string Name => "AutoTeamPlus";
    public static Configuration Config;

    public Autoteam(Main game) : base(game)
    {
    }

    private static void LoadConfig()
    {
        Config = Configuration.Read(Configuration.FilePath);
        Config.Write(Configuration.FilePath);
    }

    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player?.SendSuccessMessage("[{0}] 重新加载配置完毕。", typeof(Autoteam).Name);
    }

    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += ReloadConfig;
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnJoin);
        PlayerHooks.PlayerPostLogin += this.OnLogin;
        GetDataHandlers.PlayerTeam += this.Team;
        Commands.ChatCommands.Add(new Command("autoteam.toggle", this.TogglePlugin, "autoteam", "at"));
        LoadConfig();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnJoin);
            PlayerHooks.PlayerPostLogin -= this.OnLogin;
            GetDataHandlers.PlayerTeam -= this.Team;
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.TogglePlugin);
        }
        base.Dispose(disposing);
    }

    private void TogglePlugin(CommandArgs args)
    {
        var player = args.Player;
        var parameters = args.Parameters;

        if (parameters.Count < 1)
        {
            player.SendErrorMessage("用法: /autoteam <on|off>");
            return;
        }

        var action = parameters[0].ToLower();
        switch (action)
        {
            case "on":
                Config.Enabled = true;
                player.SendSuccessMessage("AutoTeamPlus 插件已启用.");
                break;
            case "off":
                Config.Enabled = false;
                player.SendSuccessMessage("AutoTeamPlus 插件已禁用.");
                break;
            default:
                player.SendErrorMessage("无效的操作。请使用 'on' 或 'off'。");
                break;
        }
    }


    private void Team(object sender, PlayerTeamEventArgs args)
    {
        this.SetTeam(args.Player);
        args.Handled = true;
    }

    private void OnJoin(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        this.SetTeam(player);
    }

    private void OnLogin(PlayerPostLoginEventArgs args)
    {
        var player = args.Player;
        this.SetTeam(player);
    }

    private void SetTeam(TSPlayer player)
    {
        if (!Config.Enabled)
        {
            return;
        }

        if (player == null || player.Group == null)
        {
            return;
        }

        if (player.Group.HasPermission("noautoteam"))
        {
            return;
        }

        var groupName = player.Group.Name.ToLower();
        var teamName = Config.GetTeamForGroup(groupName);

        // 获取队伍索引
        var teamIndex = this.GetTeamIndex(teamName);

        if (teamIndex != -1)
        {
            player.SetTeam(teamIndex);
            player.SendInfoMessage($"你的队伍已切换为 {teamName}.");
        }
        else
        {
            player.SendInfoMessage("未配置，可随意切换.");
        }
    }

    private int GetTeamIndex(string teamName)
    {
        return teamName.ToLower() switch
        {
            "none" or "无队伍" => 0,
            "red" or "红队" => 1,
            "green" or "绿队" => 2,
            "blue" or "蓝队" => 3,
            "yellow" or "黄队" => 4,
            "pink" or "粉队" => 5,
            _ => -1,
        };
    }

}