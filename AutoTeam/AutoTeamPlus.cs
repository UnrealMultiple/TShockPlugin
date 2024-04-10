using System;
using System.Collections.Generic;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static TShockAPI.GetDataHandlers;
using Terraria;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace autoteam
{
    [ApiVersion(2, 1)]
    public class Autoteam : TerrariaPlugin
    {
    public override string Author => "十七改，肝帝熙恩改";
    public override Version Version => new Version(2, 3);
    public override string Description => "自动队伍";
    public override string Name => "AutoTeamPlus";
    public static Configuration Config;

    private readonly Dictionary<string, string> teamNames = new Dictionary<string, string>
    {
        { "none", "无队伍" },
        { "red", "红队" },
        { "green", "绿队" },
        { "blue", "蓝队" },
        { "yellow", "黄队" },
        { "pink", "粉队" }
    };

    public Autoteam(Main game) : base(game) 
        {
            LoadConfig();
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
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnJoin);
        PlayerHooks.PlayerPostLogin += OnLogin;
        GetDataHandlers.PlayerTeam += Team;
            Commands.ChatCommands.Add(new Command("autoteam.toggle", TogglePlugin, "autoteam", "at"));
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

            string action = parameters[0].ToLower();
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

        protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnJoin);
            PlayerHooks.PlayerPostLogin -= OnLogin;
            GetDataHandlers.PlayerTeam -= Team;
        }
        base.Dispose(disposing);
    }

    private void Team(object sender, PlayerTeamEventArgs args)
    {
        SetTeam(args.Player);
        args.Handled = true;
    }

    private void OnJoin(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        SetTeam(player);
    }

    private void OnLogin(PlayerPostLoginEventArgs args)
    {
        var player = args.Player;
        SetTeam(player);
    }

        private void SetTeam(TSPlayer player)
        {
            if (!Config.Enabled)
                return;

            if (player == null || player.Group == null)
                return;

            if (player.Group.HasPermission("noautoteam"))
                return;

            string groupName = player.Group.Name.ToLower();
            string teamName = Config.GetTeamForGroup(groupName);

            // 获取队伍索引
            int teamIndex = GetTeamIndex(teamName);

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
            switch (teamName.ToLower())
            {
                case "none":
                case "无队伍":
                    return 0;
                case "red":
                case "红队":
                    return 1;
                case "green":
                case "绿队":
                    return 2;
                case "blue":
                case "蓝队":
                    return 3;
                case "yellow":
                case "黄队":
                    return 4;
                case "pink":
                case "粉队":
                    return 5;
                default:
                    return -1;
            }
        }

    }
}
