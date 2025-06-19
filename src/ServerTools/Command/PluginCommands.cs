using LazyAPI.Attributes;
using Microsoft.Xna.Framework;
using ServerTools.DB;
using Terraria;
using TShockAPI;

namespace ServerTools.Command;

[Command("selfkick", "kickself", "自踢")]
[Permissions("servertool.user.kick")]
public class KickSelf
{
    [Main]
    [RealPlayer]
    public static void Kick(CommandArgs args)
    {
        args.Player.Disconnect(GetString("你要求被踢出！"));
    }
}

[Command("selfkill", "killself", "自杀")]
[Permissions("servertool.user.kill")]
public class KillSelf
{
    [Main]
    [RealPlayer]
    public static void Kill(CommandArgs args)
    {
        args.Player.KillPlayer();
    }
}

[Command("ghost")]
[Permissions("servertool.user.ghost")]
public class Ghost
{
    [Main]
    [RealPlayer]
    public static void GhoshPlayer(CommandArgs args)
    {
        args.Player.TPlayer.ghost = !args.Player.TPlayer.ghost;
        args.Player.SendData(PacketTypes.PlayerInfo, "", args.Player.Index);
        args.Player.SendData(PacketTypes.PlayerUpdate, "", args.Player.Index);
    }
}

[Command("clp")]
[Permissions("tshock.clear")]
public class ClearOtherProj
{
    [Main]
    public static void Clear(CommandArgs args)
    {
        var radius = 50;
        if (args.Parameters.Count == 1)
        {
            if (!int.TryParse(args.Parameters[0], out radius) || radius <= 0)
            {
                args.Player.SendErrorMessage(GetString("错误的范围!"));
                return;
            }
        }
        var cleared = 0;
        for (var i = 0; i < Main.maxProjectiles; i++)
        {
            var dx = Main.projectile[i].position.X - args.Player.X;
            var dy = Main.projectile[i].position.Y - args.Player.Y;
            if (Main.projectile[i].active && !Main.projectile[i].sentry && !Main.projectile[i].minion && (dx * dx) + (dy * dy) <= radius * radius * 256)
            {
                Main.projectile[i].active = false;
                Main.projectile[i].type = 0;
                TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", i);
                cleared++;
            }
        }
        TShock.Utils.Broadcast(string.Format(GetString("{0}清除了{1}范围内{2}个射弹!"), args.Player.Name, radius, cleared), Color.Yellow);
    }
}

[Command("oc")]
[Permissions("servertool.user.cmd")]
public class OthersExecuteCommand
{
    [Main]
    public static void Execute(CommandArgs args, TSPlayer player, string cmd)
    {
        player.tempGroup = new SuperAdminGroup();
        try
        {
            Commands.HandleCommand(player, cmd);
        }
        finally
        {
            player.tempGroup = null;
        }

        args.Player.SendSuccessMessage(GetString($"已为玩家{player.Name}执行命令`{cmd}`"));
    }
}

[Command("rank", "排行")]
[Permissions("servertool.user.rank")]
public class PlayerRank
{
    [Alias("online", "在线")]
    [Flexible]
    public static void Online(CommandArgs args)
    {
        var OnlineInfo = PlayerOnline.GetOnlineRank().Select(online => GetString($"{online.Name} 在线时长: {Math.Ceiling(Convert.ToDouble(online.Duration * 1.0f / 60))}分钟").Color(TShockAPI.Utils.GreenHighlight)).ToList();
        args.SendPage(OnlineInfo, 1, new PaginationTools.Settings
        {
            MaxLinesPerPage = 30,
            NothingToDisplayString = GetString("当前没有玩家在线数据"),
            HeaderFormat = GetString("在线排行 ({0}/{1})："),
            FooterFormat = GetString("输入 {0}在线排行 {{0}} 查看更多").SFormat(Commands.Specifier)
        });
    }

    [Alias("dead", "死亡")]
    [Flexible]
    public static void Death(CommandArgs args)
    {
        var line = DB.PlayerDeath.GetDeathRank().Select(x => GetString($"[{x.Name}] => 死亡{x.Count}次"));
        args.SendPage(line, 1, new PaginationTools.Settings
        {
            MaxLinesPerPage = 30,
            NothingToDisplayString = GetString("没有死亡数据!"),
            HeaderFormat = GetString("死亡排行 ({0}/{1})："),
            FooterFormat = GetString("输入 {0}死亡排行 {{0}} 查看更多").SFormat(Commands.Specifier)
        });
    }
}