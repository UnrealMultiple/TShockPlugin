using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static TShockAPI.GetDataHandlers;

namespace AutoTeam;

[ApiVersion(2, 1)]
public class AutoTeam : TerrariaPlugin
{
    public override string Author => "十七改，肝帝熙恩改";
    public override Version Version => new Version(2, 4, 4);
    public override string Description => "AutoTeamPlus";
    public override string Name => "更好的自动队伍";
    public static Configuration Config = null!;

    public AutoTeam(Main game) : base(game)
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
        args.Player?.SendSuccessMessage(GetString("[自动队伍] 重新加载配置完毕。"));
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
        // 切换插件的状态
        Config.Enabled = !Config.Enabled;
        var status = Config.Enabled ? GetString("启用") : GetString("禁用");
        args.Player.SendSuccessMessage(GetString("AutoTeamPlus 插件已") + status + GetString("。"));
    }


    private void Team(object? sender, PlayerTeamEventArgs args)
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
            player.SendInfoMessage(GetString($"你的队伍已切换为 {teamName}."));
        }
        else
        {
            player.SendInfoMessage(GetString("未配置，可随意切换."));
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