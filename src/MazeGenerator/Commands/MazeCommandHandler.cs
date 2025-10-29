using TShockAPI;
using MazeGenerator.Models;

namespace MazeGenerator.Commands;

public class MazeCommandHandler : IDisposable
{
    public void Initialize()
    {
        TShockAPI.Commands.ChatCommands.Add(new Command("maze.generate", this.MazeCommand, "maze") { HelpText = GetString("迷宫生成器命令，输入 /maze help 查看帮助") });
    }

    public void Dispose()
    {
        TShockAPI.Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.MazeCommand);
    }

    public void HandleTileEdit(GetDataHandlers.TileEditEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        var player = args.Player;
        if (player == null)
        {
            return;
        }

        if (MazeGenerator.Instance.MazeBuilder.HandleTileEdit(player.Name, args.X, args.Y))
        {
            player.SendSuccessMessage(GetString($"迷宫位置已设置: X={args.X} Y={args.Y}"));
            player.SendSuccessMessage(GetString("现在可以使用 /maze build <名称> [大小] 生成迷宫了"));
            args.Handled = true;
        }
    }

    private void MazeCommand(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count == 0)
        {
            this.ShowHelp(player);
            return;
        }

        var subCommand = args.Parameters[0].ToLower();

        switch (subCommand)
        {
            case "help":
                this.ShowHelp(player);
                break;
            case "pos":
                if (!player.HasPermission("maze.admin"))
                {
                    player.SendErrorMessage(GetString("你没有管理员权限！"));
                    return;
                }

                this.HandleSetPosition(args);
                break;
            case "build":
                this.HandleBuildMaze(args);
                break;
            case "join":
                this.HandleJoinGame(args);
                break;
            case "leave":
                this.HandleLeaveGame(args);
                break;
            case "reset":
                if (!player.HasPermission("maze.admin"))
                {
                    player.SendErrorMessage(GetString("你没有管理员权限！"));
                    return;
                }

                this.HandleResetMaze(args);
                break;
            case "list":
                this.HandleListPositions(args);
                break;
            case "rank":
                this.HandleLeaderboard(args);
                break;
            case "del":
                if (!player.HasPermission("maze.admin"))
                {
                    player.SendErrorMessage(GetString("你没有管理员权限！"));
                    return;
                }

                this.HandleDeletePosition(args);
                break;
            case "path":
                this.HandleShowPath(args);
                break;
            default:
                player.SendErrorMessage(GetString($"未知的子命令: {subCommand}"));
                player.SendInfoMessage(GetString("输入 /maze help 查看帮助"));
                break;
        }
    }

    private void ShowHelp(TSPlayer player)
    {
        var config = Config.Instance;
        player.SendInfoMessage(GetString("=== 迷宫生成器 ==="));
        player.SendInfoMessage(GetString("/maze build <名称> [大小] - 生成迷宫"));
        player.SendInfoMessage(GetString("/maze join <名称> - 加入迷宫游戏"));
        player.SendInfoMessage(GetString("/maze leave - 退出游戏"));
        player.SendInfoMessage(GetString("/maze reset <名称> - 重置迷宫"));
        player.SendInfoMessage(GetString("/maze list - 列出所有位置"));
        player.SendInfoMessage(GetString("/maze rank [页码] - 查看排行榜"));

        if (player.HasPermission("maze.admin"))
        {
            player.SendInfoMessage(GetString("/maze pos <名称> <tl|bl|tr|br> - 设置迷宫位置（左上角、左下角、右上角、右下角）"));
            player.SendInfoMessage(GetString("/maze del <名称> - 删除位置和清除方块"));
            player.SendInfoMessage(GetString("/maze path <名称> - 显示/隐藏迷宫路径"));
        }

        player.SendInfoMessage(GetString($"最小迷宫大小: {config.MinSize}, 最大迷宫大小: {config.MaxSize}"));
        player.SendInfoMessage(GetString($"单元格大小: {config.CellSize}x{config.CellSize} 格"));
    }

    private void HandleShowPath(CommandArgs args)
    {
        var player = args.Player;

        if (!player.HasPermission("maze.admin"))
        {
            player.SendErrorMessage(GetString("你没有管理员权限！"));
            return;
        }

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/maze path <名称>"));
            return;
        }

        var name = args.Parameters[1];

        MazeGenerator.Instance.MazeBuilder.TogglePathCommand(player, name);
    }

    private void HandleSetPosition(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 3)
        {
            player.SendErrorMessage(GetString("用法：/maze pos <名称> <tl|bl|tr|br>"));
            player.SendErrorMessage(GetString("对齐方式: tl=左上角, bl=左下角, tr=右上角, br=右下角"));
            return;
        }

        var name = args.Parameters[1];
        var posType = args.Parameters[2].ToLower();

        if (posType != "tl" && posType != "bl" && posType != "tr" && posType != "br")
        {
            player.SendErrorMessage(GetString("位置参数无效。请使用：tl(左上角)、bl(左下角)、tr(右上角)、br(右下角)"));
            return;
        }

        if (MazeGenerator.Instance.MazeBuilder.SetPosition(player.Name, name, posType))
        {
            player.SendSuccessMessage(GetString($"请使用稿子点击一个方块来设置 '{name}' 的位置！"));
        }
        else
        {
            player.SendErrorMessage(GetString($"位置名称 '{name}' 已存在！"));
        }
    }

    private void HandleBuildMaze(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/maze build <名称> [大小]"));
            return;
        }

        var name = args.Parameters[1];
        var config = Config.Instance;
        var size = config.DefaultSize;

        if (args.Parameters.Count >= 3)
        {
            if (!int.TryParse(args.Parameters[2], out size) || size < config.MinSize || size > config.MaxSize)
            {
                player.SendErrorMessage(GetString($"大小必须是{config.MinSize}到{config.MaxSize}之间的整数！"));
                return;
            }
        }

        MazeGenerator.Instance.MazeBuilder.BuildMaze(player, name, size);
    }

    private void HandleJoinGame(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/maze join <名称>"));
            return;
        }

        var name = args.Parameters[1];
        var session = MazeGenerator.Instance.MazeBuilder.GetSession(name);

        if (session == null)
        {
            player.SendErrorMessage(GetString($"未找到迷宫 '{name}'，请先使用 /maze build 生成迷宫"));
            return;
        }

        if (!MazeGenerator.Instance.GameManager.JoinGame(player, name, session))
        {
            player.SendErrorMessage(GetString("无法加入游戏！"));
        }
    }

    private void HandleLeaveGame(CommandArgs args)
    {
        var player = args.Player;
        MazeGenerator.Instance.GameManager.LeaveGame(player);
    }

    private void HandleResetMaze(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/maze reset <名称>"));
            return;
        }

        var name = args.Parameters[1];

        MazeGenerator.Instance.MazeBuilder.ResetMaze(player, name);
    }

    private void HandleListPositions(CommandArgs args)
    {
        var player = args.Player;
        var positions = MazeGenerator.Instance.MazeBuilder.GetPositions();

        if (positions.Count == 0)
        {
            player.SendInfoMessage(GetString("没有保存的迷宫位置。"));
            return;
        }

        player.SendInfoMessage(GetString("=== 已保存的迷宫位置 ==="));
        foreach (var (name, pos) in positions)
        {
            var session = MazeGenerator.Instance.MazeBuilder.GetSession(name);
            var hasMaze = session != null ? GetString(" [已生成]") : "";
            player.SendInfoMessage(GetString($"- {name}: X={pos.X}, Y={pos.Y}, 对齐={pos.PositionType}{hasMaze}"));
        }
    }

    private void HandleLeaderboard(CommandArgs args)
    {
        var player = args.Player;
        var page = 1;

        if (args.Parameters.Count >= 2)
        {
            if (!int.TryParse(args.Parameters[1], out page) || page < 1)
            {
                player.SendErrorMessage(GetString("页码必须是正整数！"));
                return;
            }
        }

        var (records, totalPages, playerRank) = MazeGenerator.Instance.Leaderboard.GetLeaderboardPage(page, player.Name);

        if (records.Count == 0)
        {
            player.SendInfoMessage(GetString("排行榜为空。"));
            return;
        }

        player.SendInfoMessage(GetString($"=== 迷宫排行榜 (第 {page}/{totalPages} 页) ==="));
        for (var i = 0; i < records.Count; i++)
        {
            var record = records[i];
            var rank = ((page - 1) * Config.Instance.LeaderboardPageSize) + i + 1;
            player.SendInfoMessage(GetString($"{rank}. {record.PlayerName} - {record.MazeName} - {record.Duration:mm\\:ss\\.ff}"));
        }

        if (playerRank > 0)
        {
            player.SendInfoMessage(GetString($"你的排名: 第 {playerRank} 名"));
        }
    }

    private void HandleDeletePosition(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/maze del <名称>"));
            return;
        }

        var name = args.Parameters[1];

        if (MazeGenerator.Instance.MazeBuilder.DeletePosition(name))
        {
            MazeGenerator.Instance.Leaderboard.ClearMazeRecords(name);
            player.SendSuccessMessage(GetString($"已删除位置 '{name}' 并清除相关方块和排行榜记录"));
        }
        else
        {
            player.SendErrorMessage(GetString($"未找到位置 '{name}'"));
        }
    }
}