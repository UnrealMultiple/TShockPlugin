using MusicPlayer.Music;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace MusicPlayer;

[ApiVersion(2, 1)]
public class MusicPlayer : TerrariaPlugin
{
    public override string Author => "Olink，Cjx适配，肝帝熙恩修改, yu大改";

    public override string Description => GetString("一个简单的音乐播放插件.");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 6);

    public string songPath = Path.Combine(TShock.SavePath, "Songs");

    internal static SongPlayer?[] SongPlayers = new SongPlayer[255];

    private static bool isNoOneListening;

    private readonly Command[] addCommands;

    public MusicPlayer(Main game) : base(game)
    {
        this.addCommands = new Command[]
        {
            new ("song", this.PlaySong, "song"),
            new ("songall", this.PlaySongAll, "songall"),
            new ("songlist", this.ListFiles, "songlist")
        };
    }

    public override void Initialize()
    {
        if (!Directory.Exists(this.songPath))
        {
            Directory.CreateDirectory(this.songPath);
        }
        Array.ForEach(this.addCommands, command => Commands.ChatCommands.Add(command));
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnJoin);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Array.ForEach(this.addCommands, command => Commands.ChatCommands.Remove(command));

            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnJoin);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
        }
        base.Dispose(disposing);
    }


    private void OnJoin(GreetPlayerEventArgs args)
    {
        var who = args.Who;
        SongPlayers[who] = new SongPlayer(TShock.Players[who]); // 创建新的 SongPlayer 对象
    }

    private void OnLeave(LeaveEventArgs args)
    {
        SongPlayers[args.Who] = null; // 移除对应的 SongPlayer 对象
        ListeningCheck();
    }
    public static void ListeningCheck()
    {
        isNoOneListening = Array.TrueForAll(SongPlayers, x => x is null || !x.Listening);
    }
    public static void OnUpdate(EventArgs args)
    {
        if (isNoOneListening)
        {
            return;
        }
        for (var i = 0; i < 255; i++)
        {
            if (SongPlayers[i] is null)
            {
                continue;
            }
            var songPlayer = SongPlayers[i]!;
            if (!songPlayer.Listening)
            {
                continue;
            }
            songPlayer.CurrentSong.Update(i);
        }
    }
    public void PlaySong(CommandArgs args)
    {
        if (!args.Player.RealPlayer)
        {
            args.Player.SendErrorMessage(GetString("此命令要求在游戏内使用"));
            return;
        }
        var songPlayer = SongPlayers[args.Player.Index];
        if (songPlayer is null)
        {
            return;
        }
        var invalidUsageMessage = GetString("方式: /song <歌曲名称> [演奏乐器]\n演奏乐器: harp, theaxe, bell，默认为 harp");
        var stopPlaybackMessage = GetString("使用 /song 来停止播放.");

        if (args.Parameters.Count == 0)
        {
            if (songPlayer.Listening)
            {
                songPlayer.EndSong();
                TShock.Players[songPlayer.Player.Index].SendInfoMessage(GetString("已经为你停止播放"));
            }
            else
            {
                args.Player.SendInfoMessage(invalidUsageMessage);
                args.Player.SendInfoMessage(stopPlaybackMessage);
            }
        }
        else
        {
            var songName = args.Parameters[0];
            var filePath = Path.Combine(this.songPath, songName);

            if (File.Exists(filePath))
            {
                var notes = NoteFileParser.Read(filePath, out var tempo);
                isNoOneListening = false;
                var performer = VirtualPerformer.GetPerformer(args.Parameters.ElementAtOrDefault(1));
                performer.Create(songPlayer.Player.Index);
                songPlayer.StartSong(new PlaySongInfo(notes, tempo, performer));
                args.Player.SendInfoMessage(GetString("正在播放: {0}"), songName); // 添加这条消息来提示正在播放
            }
            else
            {
                args.Player.SendErrorMessage(GetString("加载歌曲失败: '{0}'"), songName);
            }
        }
    }


    public void PlaySongAll(CommandArgs args)
    {
        if (args.Parameters.Any())
        {
            var songName = args.Parameters[0];
            var filePath = Path.Combine(this.songPath, songName);

            if (File.Exists(filePath))
            {
                var notes = NoteFileParser.Read(filePath, out var tempo);
                isNoOneListening = false;
                for (var i = 0; i < SongPlayers.Length; i++)
                {
                    var songPlayer = SongPlayers[i];
                    if (songPlayer is not null)
                    {
                        var performer = VirtualPerformer.GetPerformer(args.Parameters.ElementAtOrDefault(1));
                        performer.Create(i);
                        songPlayer.StartSong(new PlaySongInfo(notes, tempo, performer));
                        if (TShock.Players[i].Active)
                        {
                            TShock.Players[i].SendInfoMessage(GetString("正在给您播放: {0}，使用/song停止播放"), songName);
                        }
                    }
                }
            }
            else
            {
                args.Player.SendErrorMessage(GetString("加载歌曲失败: '{0}'"), songName);
            }
        }
        else
        {
            // 如果没有提供歌曲名称，则停止所有玩家正在播放的歌曲
            foreach (var songPlayer in SongPlayers)
            {
                if (songPlayer != null && songPlayer.Listening)
                {
                    songPlayer.EndSong();
                    TShock.Players[songPlayer.Player.Index].SendInfoMessage(GetString("已经为你停止播放"));
                }
            }
            args.Player.SendInfoMessage(GetString("已经为所有玩家停止播放"));
        }
    }


    private void ListFiles(CommandArgs args)
    {
        var targetFolder = Path.Combine(TShock.SavePath, "Songs");
        if (!Directory.Exists(targetFolder))
        {
            args.Player.SendErrorMessage(GetString("目录不存在: {0}"), targetFolder);
            return;
        }

        var files = Directory.GetFiles(targetFolder);

        // 如果没有文件
        if (files.Length == 0)
        {
            args.Player.SendSuccessMessage(GetString("没有任何歌曲: {0}"), targetFolder);
            return;
        }

        // 发送文件列表到聊天
        var fileListMessage = new StringBuilder(GetString("本服务器有以下歌曲:\n"));
        foreach (var file in files)
        {
            fileListMessage.Append(Path.GetFileName(file)).AppendLine();
        }

        args.Player.SendMessage(fileListMessage.ToString(), Microsoft.Xna.Framework.Color.Yellow);
    }
}