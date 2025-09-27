using LazyAPI;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace PlayerRandomSwapper;
[ApiVersion(2, 1)]
public class PlayerRandomSwapper : LazyPlugin
{
    public override string Author => "肝帝熙恩,少司命";
    public override string Description => GetString("一个插件，用于在指定时间后随机交换玩家位置。");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override Version Version => new Version(1, 0, 8);

    private DateTime LastCheck { get; set; } = DateTime.UtcNow;
    private int RemainingSeconds { get; set; }

    public PlayerRandomSwapper(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        this.addCommands.Add(new Command("swapplugin.toggle", this.SwapToggle, "swaptoggle", GetString("随机互换")));
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        this.RandomSwapTime();
        base.Initialize();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
        }
        base.Dispose(disposing);
    }

    private void RandomSwapTime()
    {
        var random = new Random();
        this.RemainingSeconds = Config.Instance.RandomInterval
            ? random.Next(Config.Instance.MinRandomIntervalSeconds, Config.Instance.MaxRandomIntervalSeconds)
            : Config.Instance.IntervalSeconds;
    }

    private void OnUpdate(EventArgs args)
    {
        if (!Config.Instance.pluginEnabled)
        {
            this.RandomSwapTime();
            return;
        }

        var players = TShock.Players.Where(p => p != null && p.Active && !p.Dead);
        var eligiblePlayers = players.Where(p => p.HasPermission("playerswap")).ToList();

        if (eligiblePlayers.Count < 2)
        {
            this.RandomSwapTime();
            return;
        }

        if ((DateTime.UtcNow - this.LastCheck).TotalSeconds >= 1)
        {
            this.LastCheck = DateTime.UtcNow;
            this.RemainingSeconds--;

            if (this.RemainingSeconds <= 0)
            {
                this.SwapPlayers(eligiblePlayers);
                this.RandomSwapTime();
            }
            else if (Config.Instance.BroadcastRemainingTimeEnabled && this.RemainingSeconds <= Config.Instance.BroadcastRemainingTimeThreshold)
            {
                TSPlayer.All.SendMessage(GetString($"注意：还有{this.RemainingSeconds}秒就要交换位置了！"), Color.Yellow);
            }
        }
    }

    private void SwapToggle(CommandArgs args)
    {
        if (args.Parameters.Count < 1)
        {
            args.Player.SendInfoMessage(GetString("用法:"));
            args.Player.SendInfoMessage("/swaptoggle <timer|swap|enable|interval|allowself|randominterval|mininterval|maxinterval>");
            args.Player.SendInfoMessage(GetString("/swaptoggle en - 切换随机位置互换的状态"));
            args.Player.SendInfoMessage(GetString("/swaptoggle timer [广播交换倒计时阈值] - 切换广播剩余传送时间的状态或设置广播交换倒计时阈值"));
            args.Player.SendInfoMessage(GetString("/swaptoggle swap - 切换广播玩家交换位置信息的状态"));
            args.Player.SendInfoMessage(GetString("/swaptoggle i <传送间隔秒> - 设置传送间隔时间（秒）"));
            args.Player.SendInfoMessage(GetString("/swaptoggle as - 切换允许双人模式玩家和自己交换位置的状态"));
            args.Player.SendInfoMessage(GetString("/swaptoggle ri - 切换随机传送间隔的状态"));
            args.Player.SendInfoMessage(GetString("/swaptoggle mini <最小传送间隔秒> - 设置最小传送间隔时间（秒）"));
            args.Player.SendInfoMessage(GetString("/swaptoggle maxi <最大传送间隔秒> - 设置最大传送间隔时间（秒）"));
            return;
        }

        var subCommand = args.Parameters[0].ToLower();
        switch (subCommand)
        {
            case "timer":
            case "广播时间":
                if (args.Parameters.Count == 1)
                {
                    Config.Instance.BroadcastRemainingTimeEnabled = !Config.Instance.BroadcastRemainingTimeEnabled;
                    Config.Save();
                    args.Player.SendSuccessMessage(GetString($"广播剩余传送时间已") + (Config.Instance.BroadcastRemainingTimeEnabled ? GetString("启用") : GetString("禁用")) + "。");
                }
                else if (args.Parameters.Count == 2 && int.TryParse(args.Parameters[1], out var time))
                {
                    Config.Instance.BroadcastRemainingTimeThreshold = time;
                    Config.Save();
                    args.Player.SendSuccessMessage(GetString($"广播剩余传送时间已设置为 {time} 秒"));
                }
                else
                {
                    args.Player.SendInfoMessage(GetString("用法:"));
                    args.Player.SendErrorMessage(GetString("/swaptoggle timer - 切换广播剩余传送时间的状态"));
                    args.Player.SendErrorMessage(GetString("/swaptoggle timer <时间> - 设置广播剩余传送时间"));
                }
                break;

            case "swap":
            case "广播交换":
                Config.Instance.BroadcastPlayerSwapEnabled = !Config.Instance.BroadcastPlayerSwapEnabled;
                Config.Save();
                args.Player.SendSuccessMessage(GetString("广播玩家交换位置信息已") + (Config.Instance.BroadcastPlayerSwapEnabled ? GetString("启用") : GetString("禁用")) + "。");
                break;

            case "enable":
            case "en":
            case "开关":
                Config.Instance.pluginEnabled = !Config.Instance.pluginEnabled;
                Config.Save();
                args.Player.SendSuccessMessage(GetString("随机位置互换已") + (Config.Instance.pluginEnabled ? GetString("启用") : GetString("禁用")) + "。");
                break;

            case "interval":
            case "i":
            case "传送间隔":
                if (args.Parameters.Count < 2 || !int.TryParse(GetString(args.Parameters[1]), out var interval))
                {
                    args.Player.SendErrorMessage(GetString("用法: /swaptoggle i <传送间隔秒>"));
                    return;
                }
                Config.Instance.IntervalSeconds = interval;
                Config.Save();
                args.Player.SendSuccessMessage(GetString($"传送间隔已设置为 {interval} 秒。"));
                break;

            case "allowself":
            case "as":
            case "和自己交换":
                Config.Instance.AllowSamePlayerSwap = !Config.Instance.AllowSamePlayerSwap;
                Config.Save();
                args.Player.SendSuccessMessage(GetString("允许玩家和自己交换位置已") + (Config.Instance.AllowSamePlayerSwap ? GetString("启用") : GetString("禁用")) + "。");
                break;

            case "randominterval":
            case "ri":
            case "随机传送间隔":
                Config.Instance.RandomInterval = !Config.Instance.RandomInterval;
                Config.Save();
                args.Player.SendSuccessMessage(GetString("随机传送间隔已") + (Config.Instance.RandomInterval ? GetString("启用") : GetString("禁用")) + "。");
                break;

            case "mininterval":
            case "mini":
            case "最小传送间隔":
                if (args.Parameters.Count < 2 || !int.TryParse(GetString(args.Parameters[1]), out var minInterval))
                {
                    args.Player.SendErrorMessage(GetString("用法: /swaptoggle mini <最小传送间隔秒>"));
                    return;
                }
                Config.Instance.MinRandomIntervalSeconds = minInterval;
                Config.Save();
                args.Player.SendSuccessMessage(GetString($"最小传送间隔已设置为 {minInterval} 秒。"));
                break;

            case "maxinterval":
            case "maxi":
            case "最大传送间隔":
                if (args.Parameters.Count < 2 || !int.TryParse(GetString(args.Parameters[1]), out var maxInterval))
                {
                    args.Player.SendErrorMessage(GetString("用法: /swaptoggle maxi <最大传送间隔秒>"));
                    return;
                }
                Config.Instance.MaxRandomIntervalSeconds = maxInterval;
                Config.Save();
                args.Player.SendSuccessMessage(GetString($"最大传送间隔已设置为 {maxInterval} 秒。"));
                break;

            case "help":
                args.Player.SendInfoMessage(GetString("用法:"));
                args.Player.SendInfoMessage(GetString("/swaptoggle en - 切换随机位置互换的状态"));
                args.Player.SendInfoMessage(GetString("/swaptoggle timer [广播交换倒计时阈值] - 切换广播剩余传送时间的状态或设置广播交换倒计时阈值"));
                args.Player.SendInfoMessage(GetString("/swaptoggle swap - 切换广播玩家交换位置信息的状态"));
                args.Player.SendInfoMessage(GetString("/swaptoggle i <传送间隔秒> - 设置传送间隔时间（秒）"));
                args.Player.SendInfoMessage(GetString("/swaptoggle as - 切换允许双人模式玩家和自己交换位置的状态"));
                args.Player.SendInfoMessage(GetString("/swaptoggle ri - 切换随机传送间隔的状态"));
                args.Player.SendInfoMessage(GetString("/swaptoggle mini <最小传送间隔秒> - 设置最小传送间隔时间（秒）"));
                args.Player.SendInfoMessage(GetString("/swaptoggle maxi <最大传送间隔秒> - 设置最大传送间隔时间（秒）"));
                break;

            default:
                args.Player.SendErrorMessage(GetString("用法: /swaptoggle <timer|swap|enable|interval|allowself|randominterval|mininterval|maxinterval>"));
                break;
        }
    }

    private void SwapPlayers(List<TSPlayer> players)
    {
        if (Config.Instance.MultiPlayerMode)
        {
            this.SwapMorePlayers(players);
        }
        else
        {
            this.SwapTwoPlayers(players);
        }
    }
    private void SwapTwoPlayers(List<TSPlayer> players)
    {
        var random = new Random();
        var index1 = random.Next(players.Count);
        var index2 = random.Next(players.Count);

        while (!Config.Instance.AllowSamePlayerSwap && index2 == index1)
        {
            index2 = random.Next(players.Count);
        }

        var player1 = players[index1];
        var player2 = players[index2];

        if (Config.Instance.AllowSamePlayerSwap && player1.Name == player2.Name)
        {
            TSPlayer.All.SendMessage(GetString($"{player1.Name} 尝试自交，但那是没用的"), Color.DeepSkyBlue);
            return;
        }

        this.SwapPositions(player1, player2);

        player1.SendMessage(GetString($"你与 {player2.Name} 交换了位置！"), Color.DeepSkyBlue);
        player2.SendMessage(GetString($"你与 {player1.Name} 交换了位置！"), Color.DeepSkyBlue);

        if (Config.Instance.BroadcastPlayerSwapEnabled)
        {
            TSPlayer.All.SendMessage(GetString($"{player1.Name} 和 {player2.Name} 交换了位置！"), Color.Lime);
        }
    }

    private void SwapMorePlayers(List<TSPlayer> players)
    {
        if (players.Count < 2)
        {
            return;
        }

        var sp = players.OrderBy(c => Guid.NewGuid()).ToList();

        for (var i = 1; i < players.Count; i++)
        {
            (sp[i - 1].TPlayer.position, sp[i].TPlayer.position) =
            (sp[i].TPlayer.position, sp[i - 1].TPlayer.position);
        }

        foreach (var player in players)
        {
            player.Teleport(player.TPlayer.position.X, player.TPlayer.position.Y);
        }

        if (Config.Instance.BroadcastPlayerSwapEnabled)
        {
            TSPlayer.All.SendMessage(GetString($"已经交换所有玩家位置！"), Color.Lime);
        }
    }
    private void SwapPositions(TSPlayer player1, TSPlayer player2)
    {
        var tempX1 = player1.TileX;
        var tempY1 = player1.TileY;

        var tempX2 = player2.TileX;
        var tempY2 = player2.TileY;

        player1.Teleport(tempX2 * 16, tempY2 * 16);
        player2.Teleport(tempX1 * 16, tempY1 * 16);
    }

}
