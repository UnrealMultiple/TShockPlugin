using Microsoft.Xna.Framework;
using ServerTools.DB;
using Terraria;
using TShockAPI;

namespace ServerTools;

public partial class Plugin
{
    private void OthersCmd(CommandArgs args)
    {
        if (args.Parameters.Count < 2)
        {
            args.Player.SendErrorMessage(GetString("语法错误，正确语法:/oc [玩家名称] [指令]"));
            return;
        }
        var target = args.Parameters[0];
        var cmd = args.Parameters[1];
        var player = TShock.Players.FirstOrDefault(p => p != null && p.Active && p.Name == target);
        if (player == null)
        {
            args.Player.SendErrorMessage(GetString("目标玩家不在线无法执行此命令!"));
            return;
        }
        player.tempGroup = new SuperAdminGroup();
        Commands.HandleCommand(player, cmd);
        player.tempGroup = null;
        if(args.Parameters.Count == 2)
        {
            args.Player.SendSuccessMessage(GetString($"已为玩家{player.Name}执行命令`{cmd}`"));
        }
    }

    public void OnlineRank(CommandArgs args)
    {
        void ShowOnline(List<string> line)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out var pageNumber))
            {
                return;
            }

            PaginationTools.SendPage(
                    args.Player,
                    pageNumber,
                    line,
                    new PaginationTools.Settings
                    {
                        MaxLinesPerPage = 8,
                        NothingToDisplayString = GetString("当前没有玩家在线数据"),
                        HeaderFormat = GetString("在线排行 ({0}/{1})："),
                        FooterFormat = GetString("输入 {0}在线排行 {{0}} 查看更多").SFormat(Commands.Specifier)
                    }
                );
        }
        var OnlineInfo = PlayerOnline.GetOnlineRank().Select(online => GetString($"{online.Name} 在线时长: {Math.Ceiling(Convert.ToDouble(online.Duration * 1.0f / 60))}分钟").Color(TShockAPI.Utils.GreenHighlight)).ToList();
        ShowOnline(OnlineInfo.ToList());
    }


    private void DeathRank(CommandArgs args)
    {
        void Show(List<string> line)
        {
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out var pageNumber))
            {
                return;
            }

            PaginationTools.SendPage(
                args.Player,
                pageNumber,
                line,
                new PaginationTools.Settings
                {
                    MaxLinesPerPage = 6,
                    NothingToDisplayString = GetString("没有死亡数据!"),
                    HeaderFormat = GetString("死亡排行 ({0}/{1})："),
                    FooterFormat = GetString("输入 {0}死亡排行 {{0}} 查看更多").SFormat(Commands.Specifier)
                }
            );
        }
        var line = DB.PlayerDeath.GetDeathRank().Select(x => GetString($"[{x.Name}] => 死亡{x.Count}次")).ToList();
        Show(line);
    }

    private void Clear(CommandArgs args)
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

    private void JourneyDiff(CommandArgs args)
    {
        if (args.Parameters.Count == 1)
        {
            if (!Main._currentGameModeInfo.IsJourneyMode)
            {
                args.Player.SendErrorMessage(GetString("必须在旅途模式下才能设置难度！"));
                return;
            }
            if (this.SetJourneyDiff(args.Parameters[0]))
            {
                args.Player.SendSuccessMessage(GetString("难度成功设置为 {0}!"), args.Parameters[0]);
            }
        }
        else
        {
            args.Player.SendErrorMessage(GetString("正确语法: /旅途难度 <难度模式>"));
            args.Player.SendErrorMessage(GetString("有效的难度模式: master，journey，normal，expert"));
        }
    }

    private void Ghost(CommandArgs args)
    {
        args.Player.TPlayer.ghost = !args.Player.TPlayer.ghost;
        args.Player.SendData(PacketTypes.PlayerInfo, "", args.Player.Index);
        args.Player.SendData(PacketTypes.PlayerUpdate, "", args.Player.Index);
    }

    private void SelfKill(CommandArgs args)
    {
        args.Player.KillPlayer();
    }

    private void SelfKick(CommandArgs args)
    {
        args.Player.Disconnect(GetString("你要求被踢出！"));
    }

    private void RWall(CommandArgs args)
    {
        TShock.Warps.Warps.FindAll(x => x.Name.StartsWith(GetString("花苞"))).ForEach(n => TShock.Warps.Remove(n.Name));
        args.Player.SendSuccessMessage(GetString("已移除所有花苞传送点!"));
    }

    private void WallQ(CommandArgs args)
    {
        var Now = DateTime.Now;
        var cd = (Now - this.LastCommandUseTime).TotalSeconds;
        var count = 0;
        if (cd > 5)
        {
            var n = 1;
            int x = 0, y = 0;
            for (var i = 0; i < Main.tile.Width; i++)
            {
                for (var j = 0; j < Main.tile.Height; j++)
                {
                    var tile = Main.tile[i, j];
                    var sync = Math.Abs(i - x) + Math.Abs(j - y) > 2;
                    if (tile != null && tile.type == 238 && sync)
                    {
                        x = i;
                        y = j;
                        count++;
                        if (!TShock.Warps.Warps.Any(s => s.Position.X == i && s.Position.Y == j))
                        {
                            while (TShock.Warps.Warps.Any(x => x.Name == GetString("花苞") + n))
                            {
                                n++;
                            }
                            TShock.Warps.Add(i, j, GetString("花苞") + n);
                        }
                    }
                }
            }
            args.Player.SendInfoMessage(GetString("已为你搜索到{0}个花苞，输入/warp list可以查看结果"), count);
            this.LastCommandUseTime = Now;
        }
        else
        {
            args.Player.SendErrorMessage(GetString("你不能过于频繁的使用此指令!"));
        }
    }
}