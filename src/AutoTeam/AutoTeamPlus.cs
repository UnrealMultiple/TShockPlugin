using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static TShockAPI.GetDataHandlers;

namespace AutoTeam;

[ApiVersion(2, 1)]
public class AutoTeam : LazyPlugin
{
    public override string Author => "十七改，肝帝熙恩改";
    public override Version Version => new Version(2, 4, 8);
    public override string Description => GetString("自动分配一个组的玩家到特定队伍");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public AutoTeam(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnJoin);
        PlayerHooks.PlayerPostLogin += this.OnLogin;
        GetDataHandlers.PlayerTeam += this.Team;
        Commands.ChatCommands.Add(new Command("autoteam.toggle", this.TogglePlugin, "autoteam", "at"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
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
        Configuration.Instance.Enabled = !Configuration.Instance.Enabled;
        var status = Configuration.Instance.Enabled ? GetString("启用") : GetString("禁用");
        args.Player.SendSuccessMessage(GetString("AutoTeamPlus 插件已") + status);
        Configuration.Save();
    }


    private void Team(object? sender, PlayerTeamEventArgs args)
    {
        if (args.Player == null)
        {
            return;
        }

        if (this.ShouldSkipAutoTeam(args.Player))
        {
            return;
        }

        this.SetTeam(args.Player);
        args.Handled = true;
    }

    private void OnJoin(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player == null)
        {
            return;
        }

        if (this.ShouldSkipAutoTeam(player))
        {
            return;
        }

        this.SetTeam(player);
    }

    private void OnLogin(PlayerPostLoginEventArgs args)
    {
        if (args.Player == null)
        {
            return;
        }

        if (this.ShouldSkipAutoTeam(args.Player))
        {
            return;
        }

        this.SetTeam(args.Player);
    }

    private bool ShouldSkipAutoTeam(TSPlayer player)
    {
        if (!Configuration.Instance.Enabled)
        {
            return true;
        }

        if (player.Group == null || player.Group.HasPermission("noautoteam"))
        {
            return true;
        }

        var groupName = player.Group.Name;
        return Configuration.Instance.GetTeamForGroup(groupName) == GetString("未配置");
    }


    private void SetTeam(TSPlayer player)
    {

        var groupName = player.Group.Name;
        var teamName = Configuration.Instance.GetTeamForGroup(groupName);

        // 获取队伍索引
        var teamIndex = this.GetTeamIndex(teamName);

        if (teamIndex != -1)
        {
            player.SetTeam(teamIndex);
            player.SendInfoMessage(GetString($"你的队伍已切换为 {teamName}."));
        }
        else
        {
            player.SendInfoMessage(GetString($"队伍配置错误，您当前队伍为 {teamName}"));
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