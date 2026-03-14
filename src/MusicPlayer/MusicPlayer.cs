using MusicPlayer.Music;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace MusicPlayer;

[ApiVersion(2, 1)]
public class MusicPlayer : TerrariaPlugin
{
    public override string Author => "Olink，Cjx适配，肝帝熙恩修改, yu大改, 星梦优化";

    public override string Description => GetString("一个简单的音乐播放插件.");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 8);

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
        SongPlayers[who] = new SongPlayer(TShock.Players[who]); 
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

    private bool TryResolveSong(string search, out string? filePath, out string? songName, out string errorMessage)
    {
        filePath = null;
        songName = null;
        errorMessage = string.Empty;
        if (!Directory.Exists(this.songPath))
        {
            errorMessage = GetString("歌曲目录不存在，请联系管理员");
            return false;
        }

        var txtFiles = Directory.GetFiles(this.songPath, "*.txt");
        if (txtFiles.Length == 0)
        {
            errorMessage = GetString("歌曲目录中没有找到任何 .txt 文件");
            return false;
        }

        Array.Sort(txtFiles);

        if (int.TryParse(search, out int index))
        {
            if (index < 1 || index > txtFiles.Length)
            {
                errorMessage = GetString("索引号超出范围，当前共有 {0} 首歌曲", txtFiles.Length);
                return false;
            }
            filePath = txtFiles[index - 1];
            songName = Path.GetFileName(filePath);
            return true;
        }
        else
        {
            var targetPath = Path.Combine(this.songPath, search);
            if (!File.Exists(targetPath))
            {
                targetPath = Path.Combine(this.songPath, search + ".txt");
                if (!File.Exists(targetPath))
                {
                    errorMessage = GetString("未找到歌曲: '{0}'，请使用 /songlist 查看可用歌曲", search);
                    return false;
                }
            }
            filePath = targetPath;
            songName = Path.GetFileName(filePath);
            return true;
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
        
        if (args.Parameters.Count == 0)
        {
            if (songPlayer.Listening)
            {
                songPlayer.EndSong();
                TShock.Players[songPlayer.Player.Index].SendInfoMessage(GetString("已经为你停止播放"));
            }
            else
            {
                args.Player.SendInfoMessage(GetString("方式: /song <歌曲名称/索引号> [演奏乐器]\n演奏乐器: harp, theaxe, bell，默认为 harp"));
                args.Player.SendInfoMessage(GetString("使用 /song 来停止播放."));
                args.Player.SendInfoMessage(GetString("使用 /songlist 查看歌曲索引."));
            }
        }
        else
        {
            var search = args.Parameters[0];
            
            if (!TryResolveSong(search, out var filePath, out var songName, out var errorMessage))
            {
                args.Player.SendErrorMessage(errorMessage);
                return;
            }

            var notes = NoteFileParser.Read(filePath!, out var tempo);
            isNoOneListening = false;
            var performer = VirtualPerformer.GetPerformer(args.Parameters.ElementAtOrDefault(1));
            performer.Create(songPlayer.Player.Index);
            songPlayer.StartSong(new PlaySongInfo(notes, tempo, performer));
            args.Player.SendInfoMessage(GetString("正在播放: {0}"), songName);
        }
    }

    public void PlaySongAll(CommandArgs args)
    {
        if (args.Parameters.Any())
        {
            var search = args.Parameters[0];
            
            if (!TryResolveSong(search, out var filePath, out var songName, out var errorMessage))
            {
                args.Player.SendErrorMessage(errorMessage);
                return;
            }

            var notes = NoteFileParser.Read(filePath!, out var tempo);
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
            args.Player.SendErrorMessage(GetString("歌曲目录不存在: {0}"), targetFolder);
            return;
        }

        var files = Directory.GetFiles(targetFolder, "*.txt");
        
        if (files.Length == 0)
        {
            args.Player.SendSuccessMessage(GetString("没有任何歌曲文件: {0}"), targetFolder);
            return;
        }

        Array.Sort(files);
        
        int page = 1;
        const int pageSize = 10;
        
        if (args.Parameters.Count > 0 && int.TryParse(args.Parameters[0], out int parsedPage))
        {
            page = parsedPage;
        }
        
        if (page < 1)
        {
            page = 1;
        }
        
        int totalPages = (files.Length + pageSize - 1) / pageSize;
        
        if (page > totalPages)
        {
            page = totalPages;
        }
        
        int startIndex = (page - 1) * pageSize;
        int endIndex = Math.Min(startIndex + pageSize, files.Length);
        
        var fileListMessage = new StringBuilder();
        fileListMessage.AppendLine(GetString("=== 歌曲列表 (第 {0}/{1} 页) ===", page, totalPages));
        
        for (int i = startIndex; i < endIndex; i++)
        {
            fileListMessage.AppendLine($"[{i + 1}] {Path.GetFileName(files[i])}");
        }
        
        fileListMessage.AppendLine(GetString("共 {0} 首歌曲，使用 /song <索引号> 或 /song <文件名> 播放", files.Length));
        
        if (totalPages > 1)
        {
            fileListMessage.Append(GetString("翻页指令: /songlist {0}", page < totalPages ? page + 1 : 1));
        }

        args.Player.SendMessage(fileListMessage.ToString(), Microsoft.Xna.Framework.Color.Yellow);
    }
}
