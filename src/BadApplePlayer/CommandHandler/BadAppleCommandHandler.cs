using BadApplePlayer.Models;
using TShockAPI;

namespace BadApplePlayer.CommandHandler;

public class CommandHandler(BadApplePlayer plugin)
{
    public void BadAppleCommand(CommandArgs args)
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
            case "position":
                this.HandleSetPosition(args);
                break;

            case "play":
                this.HandlePlay(args);
                break;

            case "pause":
                this.HandlePause(args);
                break;

            case "resume":
                this.HandleResume(args);
                break;

            case "stop":
                this.HandleStop(args);
                break;

            case "list":
                this.HandleListPositions(args);
                break;

            case "playing":
                this.HandleListPlaying(args);
                break;

            case "del":
            case "delete":
                this.HandleDeletePosition(args);
                break;

            default:
                player.SendErrorMessage(GetString($"未知的子命令: {subCommand}"));
                player.SendInfoMessage(GetString("输入 /badapple help 查看帮助"));
                break;
        }
    }

    private void ShowHelp(TSPlayer player)
    {
        player.SendInfoMessage(GetString("=== BadApple 视频播放器 ==="));
        player.SendInfoMessage(GetString("/badapple play <名称> [loop] - 播放视频"));
        player.SendInfoMessage(GetString("/badapple pause <名称> - 暂停播放"));
        player.SendInfoMessage(GetString("/badapple resume <名称> - 继续播放"));
        player.SendInfoMessage(GetString("/badapple stop <名称> - 停止播放"));
        player.SendInfoMessage(GetString("/badapple list - 列出所有位置"));
        player.SendInfoMessage(GetString("/badapple playing - 查看播放中的会话"));

        if (player.HasPermission("badapple.admin"))
        {
            player.SendInfoMessage(GetString("/badapple pos <名称> <tl|bl|tr|br> - 设置播放位置"));
            player.SendInfoMessage(GetString("/badapple del <名称> - 删除位置（管理员）"));
        }
    }

    private void HandleSetPosition(CommandArgs args)
    {
        var player = args.Player;

        if (!player.HasPermission("badapple.admin"))
        {
            player.SendErrorMessage(GetString("你没有权限使用此命令！"));
            return;
        }

        if (args.Parameters.Count < 3)
        {
            player.SendErrorMessage(GetString("用法：/badapple pos <名称> <tl|bl|tr|br>"));
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

        var positionData = BadApplePlayer.GetPositionData();
        if (positionData.ContainsKey(name))
        {
            player.SendErrorMessage(GetString($"位置名称 '{name}' 已存在！"));
            return;
        }

        var tempData = BadApplePlayer.GetTempPositionData();
        tempData[player.Name] = (name, posType, player.Name);

        player.SendSuccessMessage(GetString($"请使用稿子点击一个方块来设置 '{name}' 的{plugin.GetPositionName(posType)}位置！"));
    }

    private void HandlePlay(CommandArgs args)
    {
        var player = args.Player;

        var video = BadApplePlayer.GetVideo();
        if (video == null)
        {
            player.SendErrorMessage(GetString("视频数据未加载，无法播放！"));
            return;
        }

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/badapple play <名称> [loop]"));
            return;
        }

        var name = args.Parameters[1];
        var loop = args.Parameters.Count > 2 && args.Parameters[2].ToLower() == "loop";

        var positionData = BadApplePlayer.GetPositionData();
        if (!positionData.TryGetValue(name, out var position))
        {
            player.SendErrorMessage(GetString($"未找到位置 '{name}'，请先使用 /badapple pos 设置位置"));
            return;
        }

        var sessions = plugin.GetPlaybackSessions();
        if (sessions.TryGetValue(name, out var existingSession))
        {
            if (existingSession.IsPlaying)
            {
                player.SendErrorMessage(GetString($"位置 '{name}' 已经在播放中！"));
                player.SendInfoMessage(GetString($"当前进度: {existingSession.CurrentFrame}/{existingSession.Video.FrameCount}"));
                return;
            }
            else
            {
                sessions.Remove(name);
            }
        }

        var session = new PlaybackSession(name, position, video, loop, player.Name);
        sessions[name] = session;

        Task.Run(async () =>
        {
            try
            {
                await plugin.PlayVideoAsync(session);
            }
            catch (Exception ex)
            {
                TShock.Log.Error(GetString($"Exception in PlayVideoAsync for session '{name}': {ex}"));
                player.SendErrorMessage(GetString($"播放BadApple视频时发生错误: {ex.Message}"));
            }
        });

        player.SendSuccessMessage(GetString($"开始在位置 '{name}' 播放BadApple视频{(loop ? " (循环模式)" : "")}！"));
        player.SendInfoMessage(GetString($"控制命令: /badapple pause {name} | /badapple resume {name} | /badapple stop {name}"));
        TSPlayer.All.SendInfoMessage(GetString($"[BadApple] {player.Name} 开始在 '{name}' 播放视频"));
    }

    private void HandlePause(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/badapple pause <名称>"));
            return;
        }

        var name = args.Parameters[1];

        var sessions = plugin.GetPlaybackSessions();
        if (!sessions.TryGetValue(name, out var session))
        {
            player.SendErrorMessage(GetString($"位置 '{name}' 没有正在播放的视频"));
            return;
        }

        if (!session.IsPlaying)
        {
            player.SendErrorMessage(GetString($"位置 '{name}' 的播放已结束"));
            return;
        }

        if (session.IsPaused)
        {
            player.SendInfoMessage(GetString($"位置 '{name}' 的播放已经处于暂停状态"));
            return;
        }

        session.IsPaused = true;
        player.SendSuccessMessage(GetString($"已暂停位置 '{name}' 的播放 (进度: {session.CurrentFrame}/{session.Video.FrameCount})"));
        TSPlayer.All.SendInfoMessage(GetString($"[BadApple] {player.Name} 暂停了 '{name}' 的播放"));
    }

    private void HandleResume(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/badapple resume <名称>"));
            return;
        }

        var name = args.Parameters[1];

        var sessions = plugin.GetPlaybackSessions();
        if (!sessions.TryGetValue(name, out var session))
        {
            player.SendErrorMessage(GetString($"位置 '{name}' 没有正在播放的视频"));
            return;
        }

        if (!session.IsPlaying)
        {
            player.SendErrorMessage(GetString($"位置 '{name}' 的播放已结束"));
            return;
        }

        if (!session.IsPaused)
        {
            player.SendInfoMessage(GetString($"位置 '{name}' 的播放已经在进行中"));
            return;
        }

        session.IsPaused = false;
        player.SendSuccessMessage(GetString($"已继续位置 '{name}' 的播放"));
        TSPlayer.All.SendInfoMessage(GetString($"[BadApple] {player.Name} 继续了 '{name}' 的播放"));
    }

    private void HandleStop(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/badapple stop <名称>"));
            return;
        }

        var name = args.Parameters[1];

        var sessions = plugin.GetPlaybackSessions();
        if (!sessions.TryGetValue(name, out var session))
        {
            player.SendErrorMessage(GetString($"位置 '{name}' 没有正在播放的视频"));
            return;
        }

        session.Stop();
        sessions.Remove(name);
        player.SendSuccessMessage(GetString($"已停止位置 '{name}' 的播放"));
        TSPlayer.All.SendInfoMessage(GetString($"[BadApple] {player.Name} 停止了 '{name}' 的播放"));
    }

    private void HandleListPositions(CommandArgs args)
    {
        var player = args.Player;
        var positionData = BadApplePlayer.GetPositionData();
        var sessions = plugin.GetPlaybackSessions();

        if (positionData.Count == 0)
        {
            player.SendInfoMessage(GetString("没有保存的BadApple位置。"));
            player.SendInfoMessage(GetString("使用 /badapple pos <名称> <tl|bl|tr|br> 创建新位置"));
            return;
        }

        player.SendInfoMessage(GetString("=== 已保存的BadApple位置 ==="));
        foreach (var (name, pos) in positionData)
        {
            var playingStatus = sessions.ContainsKey(name) ? GetString(" [播放中]") : "";
            player.SendInfoMessage(GetString($"- {name}: X={pos.X}, Y={pos.Y}, 对齐={pos.PositionType}, 创建者={pos.Creator}{playingStatus}"));
        }
    }

    private void HandleListPlaying(CommandArgs args)
    {
        var sessions = plugin.GetPlaybackSessions();
        if (sessions.Count == 0)
        {
            args.Player.SendInfoMessage(GetString("当前没有正在播放的BadApple视频"));
            return;
        }

        args.Player.SendInfoMessage(GetString($"=== 正在播放的会话 ({sessions.Count}) ==="));
        foreach (var kvp in sessions)
        {
            var name = kvp.Key;
            var session = kvp.Value;
            var status = session.IsPaused ? GetString("已暂停") : GetString("播放中");
            var progress = $"{session.CurrentFrame}/{session.Video.FrameCount}";
            var loopInfo = session.Loop ? GetString(" [循环]") : "";
            var starter = session.StartedBy;

            args.Player.SendInfoMessage(GetString($"- {name}: {status} | 进度:{progress}{loopInfo} | 发起人:{starter}"));
        }
    }

    private void HandleDeletePosition(CommandArgs args)
    {
        var player = args.Player;

        if (!player.HasPermission("badapple.admin"))
        {
            player.SendErrorMessage(GetString("你没有权限使用此命令！"));
            return;
        }

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/badapple del <名称>"));
            return;
        }

        var name = args.Parameters[1];
        var positionData = BadApplePlayer.GetPositionData();
        var tempData = BadApplePlayer.GetTempPositionData();
        var sessions = plugin.GetPlaybackSessions();
        var video = BadApplePlayer.GetVideo();

        List<KeyValuePair<string, (string name, string positionType, string creator)>>? tempEntries;
        if (!positionData.ContainsKey(name))
        {
            tempEntries = tempData.Where(x => x.Value.name == name).ToList();
            if (tempEntries.Count > 0)
            {
                foreach (var entry in tempEntries)
                {
                    tempData.Remove(entry.Key);
                }

                player.SendSuccessMessage(GetString($"已删除未完成的位置设置 '{name}'"));
            }
            else
            {
                player.SendErrorMessage(GetString($"未找到位置 '{name}'"));
            }

            return;
        }

        var found = false;

        if (sessions.TryGetValue(name, out var session))
        {
            session.Stop();
            plugin.ClearPlaybackArea(session);
            sessions.Remove(name);
            TSPlayer.All.SendWarningMessage(GetString($"[BadApple] 位置 '{name}' 已被删除，播放已停止"));
            found = true;
        }
        
        else if (video != null && positionData.TryGetValue(name, out var position))
        {
            plugin.ClearAreaByPosition(position, video.Width, video.Height);
            found = true;
        }

        tempEntries = tempData.Where(x => x.Value.name == name).ToList();
        if (tempEntries.Count > 0)
        {
            foreach (var entry in tempEntries)
            {
                tempData.Remove(entry.Key);
            }

            found = true;
        }

        if (positionData.ContainsKey(name))
        {
            positionData.Remove(name);
            plugin.SavePositionData();
            found = true;
        }

        if (found)
        {
            player.SendSuccessMessage(GetString($"已删除位置 '{name}' 并清除相关方块"));
        }
        else
        {
            player.SendErrorMessage(GetString($"未找到位置 '{name}'"));
        }
    }
}