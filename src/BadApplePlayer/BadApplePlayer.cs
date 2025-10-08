using BadApplePlayer.Models;
using System.Reflection;
using Newtonsoft.Json;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace BadApplePlayer;

[ApiVersion(2, 1)]
public class BadApplePlayer : TerrariaPlugin
{
    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "Eustia";
    public override string Description => GetString("BadApple播放器");
    public override Version Version => new (1, 0, 0, 0);

    private const int BaseWall = WallID.DiamondGemspark;
    private const int BaseColor = PaintID.WhitePaint;
    private const int CodeWall = WallID.DiamondGemspark;
    private const int CodeColor = PaintID.ShadowPaint;
    private const bool IsGlowPaintApplied = true;

    private readonly Dictionary<string, PlaybackSession> _playbackSessions = new ();
    private static BadAppleVideo? _video;
    private static Dictionary<string, PositionData> _positionData = new ();
    private static readonly Dictionary<string, (string name, string positionType, string creator)> _tempPositionData = new ();

    private static readonly string PluginDataDir = Path.Combine(TShock.SavePath, "BadApplePlayer");
    private static readonly string PositionDataPath = Path.Combine(PluginDataDir, "positions.json");

    private readonly CommandHandler.CommandHandler _commandHandler;
    private static readonly Lock TileLock = new ();

    public BadApplePlayer(Main game) : base(game)
    {
        this._commandHandler = new CommandHandler.CommandHandler(this);
    }

    public override void Initialize()
    {
        Directory.CreateDirectory(PluginDataDir);
        this.LoadPositionData();
        this.LoadVideoFromEmbeddedResource();
        this.RegisterCommands();

        GetDataHandlers.TileEdit += this.OnTileEdit;
        GeneralHooks.ReloadEvent += this.OnReload;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var session in this._playbackSessions.Values)
            {
                session.Stop();
            }

            this._playbackSessions.Clear();

            this.UnregisterCommands();
            this.UnregisterEvents();
        }

        base.Dispose(disposing);
    }

    private void LoadVideoFromEmbeddedResource()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "BadApplePlayer.Resources.badapple.dat";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                TShock.Log.ConsoleError(GetString("[BadApplePlayer] 未找到嵌入的视频资源！"));
                return;
            }

            _video = new BadAppleVideo();
            _video.LoadCompressed(stream);
            TShock.Log.ConsoleInfo(GetString($"[BadApplePlayer] 视频加载成功! 尺寸: {_video.Width}x{_video.Height}, FPS: {_video.Fps}, 帧数: {_video.FrameCount}"));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"[BadApplePlayer] 视频加载失败: {ex.Message}"));
        }
    }

    private void RegisterCommands()
    {
        Commands.ChatCommands.Add(new Command("badapple.use", this._commandHandler.BadAppleCommand, "badapple") { HelpText = GetString("BadApple视频播放器命令，输入 /badapple help 查看帮助") });
    }

    private void UnregisterCommands()
    {
        Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this._commandHandler.BadAppleCommand);
    }

    private void UnregisterEvents()
    {
        GetDataHandlers.TileEdit -= this.OnTileEdit;
        GeneralHooks.ReloadEvent -= this.OnReload;
    }

    private void OnReload(ReloadEventArgs e)
    {
        this.LoadPositionData();
        e.Player.SendSuccessMessage(GetString("[BadApplePlayer] 插件已重新加载!"));
    }

    private void OnTileEdit(object? sender, GetDataHandlers.TileEditEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        var player = TShock.Players[args.Player.Index];
        if (player == null || !player.HasPermission("badapple.use"))
        {
            return;
        }

        if (_tempPositionData.TryGetValue(player.Name, out var tempData))
        {
            var (name, positionType, creator) = tempData;

            var position = new PositionData { X = args.X, Y = args.Y, PositionType = positionType, Creator = creator };

            _positionData[name] = position;
            _tempPositionData.Remove(player.Name);

            this.SavePositionData();

            player.SendSuccessMessage(GetString($"位置 '{name}' 已设置: X={args.X} Y={args.Y} (对齐方式: {positionType})"));
            player.SendSuccessMessage(GetString($"现在可以使用 /badapple play {name} 播放视频了"));

            args.Handled = true;
        }
    }

    private void LoadPositionData()
    {
        if (File.Exists(PositionDataPath))
        {
            _positionData = JsonConvert.DeserializeObject<Dictionary<string, PositionData>>(File.ReadAllText(PositionDataPath)) ?? new Dictionary<string, PositionData>();
            TShock.Log.ConsoleInfo(GetString($"[BadApplePlayer] 已加载 {_positionData.Count} 个位置"));
        }
    }

    internal void SavePositionData()
    {
        File.WriteAllText(PositionDataPath, JsonConvert.SerializeObject(_positionData, Formatting.Indented));
    }

    internal Dictionary<string, PlaybackSession> GetPlaybackSessions()
    {
        return this._playbackSessions;
    }

    internal static Dictionary<string, PositionData> GetPositionData()
    {
        return _positionData;
    }

    internal static Dictionary<string, (string name, string positionType, string creator)> GetTempPositionData()
    {
        return _tempPositionData;
    }

    internal static BadAppleVideo? GetVideo()
    {
        return _video;
    }

    internal async Task PlayVideoAsync(PlaybackSession session)
    {
        var position = session.Position;
        var video = session.Video;

        var (startX, startY) = this.CalculateStartPosition(position, video.Width, video.Height);

        if (startX < 0 || startY < 0 || startX + video.Width >= Main.maxTilesX || startY + video.Height >= Main.maxTilesY)
        {
            TShock.Log.Error(GetString($"[BadApplePlayer] 位置 '{session.PositionName}' 超出世界边界！"));
            session.Stop();
            this._playbackSessions.Remove(session.PositionName);
            TSPlayer.All.SendErrorMessage(GetString($"[BadApple] 位置 '{session.PositionName}' 播放失败：超出世界边界"));
            return;
        }

        var frameDelay = video.Fps > 0 ? 1000 / video.Fps : 60;
        var currentFrameState = new bool[video.Width, video.Height];
        var startTime = DateTime.Now;

        try
        {
            this.InitializePlaybackArea(startX, startY, video.Width, video.Height);

            while (session.IsPlaying && session.CurrentFrame < video.FrameCount)
            {
                while (session.IsPaused && session.IsPlaying)
                {
                    await Task.Delay(100);
                }

                if (!session.IsPlaying)
                {
                    break;
                }

                var frameStart = DateTime.Now;

                var changes = video.GetFrameChanges(session.CurrentFrame,
                    session.CurrentFrame == 0 ? null : currentFrameState);

                if (session.CurrentFrame == 0)
                {
                    this.RenderFirstFrame(startX, startY, video.Width, video.Height, changes, currentFrameState);
                }
                else
                {
                    this.RenderDeltaFrame(startX, startY, changes, currentFrameState);
                }

                session.CurrentFrame++;

                if (session.Loop && session.CurrentFrame >= video.FrameCount)
                {
                    session.CurrentFrame = 0;
                    Array.Clear(currentFrameState, 0, currentFrameState.Length);
                }

                var elapsedMs = (DateTime.Now - frameStart).TotalMilliseconds;
                var sleepTime = frameDelay - elapsedMs;
                if (sleepTime > 0)
                {
                    await Task.Delay((int) sleepTime);
                }
            }

            if (session.IsPlaying)
            {
                var totalTime = (DateTime.Now - startTime).TotalSeconds;
                TSPlayer.All.SendSuccessMessage(GetString($"[BadApple] 位置 '{session.PositionName}' 播放完成！总时长: {totalTime:F2}秒"));
            }
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[BadApplePlayer] 播放错误: {ex}"));
            TSPlayer.All.SendErrorMessage(GetString($"[BadApple] 位置 '{session.PositionName}' 播放出错"));
        }
        finally
        {
            session.Stop();
            this._playbackSessions.Remove(session.PositionName);
        }
    }

    private void RenderFirstFrame(int startX, int startY, int width, int height,
        List<(int x, int y, bool isWhite)> changes, bool[,] stateBuffer)
    {
        var whitePixels = new HashSet<(int, int)>(changes.Select(c => (c.x, c.y)));

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var isWhite = whitePixels.Contains((x, y));
                stateBuffer[x, y] = isWhite;
                this.UpdateTile(startX + x, startY + y, isWhite);
            }
        }

        const int CHUNK_SIZE = 25;
        for (var cx = 0; cx < width; cx += CHUNK_SIZE)
        {
            for (var cy = 0; cy < height; cy += CHUNK_SIZE)
            {
                var chunkWidth = Math.Min(CHUNK_SIZE, width - cx);
                var chunkHeight = Math.Min(CHUNK_SIZE, height - cy);
                var size = Math.Max(chunkWidth, chunkHeight);

                TSPlayer.All.SendTileSquareCentered(
                    startX + cx + (chunkWidth / 2),
                    startY + cy + (chunkHeight / 2),
                    (byte) size);
            }
        }
    }
    /// <summary>
    /// 增量渲染
    /// </summary>
    private void RenderDeltaFrame(int startX, int startY,
        List<(int x, int y, bool isWhite)> changes, bool[,] stateBuffer)
    {
        if (changes.Count == 0)
        {
            return;
        }

        foreach (var (x, y, isWhite) in changes)
        {
            if (x < stateBuffer.GetLength(0) && y < stateBuffer.GetLength(1))
            {
                stateBuffer[x, y] = isWhite;
                this.UpdateTile(startX + x, startY + y, isWhite);
            }
        }

        if (changes.Count <= 100)
        {
            foreach (var (x, y, _) in changes)
            {
                if (x < stateBuffer.GetLength(0) && y < stateBuffer.GetLength(1))
                {
                    TSPlayer.All.SendTileSquareCentered(startX + x, startY + y, 1);
                }
            }
        }
        else if (changes.Count <= 1000)
        {
            foreach (var (x, y, _) in changes)
            {
                if (x < stateBuffer.GetLength(0) && y < stateBuffer.GetLength(1))
                {
                    TSPlayer.All.SendTileSquareCentered(startX + x, startY + y, 3);
                }
            }
        }
        else
        {
            var minX = changes.Min(c => c.x);
            var maxX = changes.Max(c => c.x);
            var minY = changes.Min(c => c.y);
            var maxY = changes.Max(c => c.y);

            var width = maxX - minX + 1;
            var height = maxY - minY + 1;

            if (width <= 40 && height <= 40)
            {
                var centerX = startX + ((minX + maxX) / 2);
                var centerY = startY + ((minY + maxY) / 2);
                var size = Math.Max(width, height);
                TSPlayer.All.SendTileSquareCentered(centerX, centerY, (byte) size);
            }
            else
            {
                const int CHUNK = 25;
                for (var cx = minX; cx <= maxX; cx += CHUNK)
                {
                    for (var cy = minY; cy <= maxY; cy += CHUNK)
                    {
                        var chunkWidth = Math.Min(CHUNK, maxX - cx + 1);
                        var chunkHeight = Math.Min(CHUNK, maxY - cy + 1);
                        var size = Math.Max(chunkWidth, chunkHeight);

                        TSPlayer.All.SendTileSquareCentered(
                            startX + cx + (chunkWidth / 2),
                            startY + cy + (chunkHeight / 2),
                            (byte) size);
                    }
                }
            }
        }
    }

    private void InitializePlaybackArea(int startX, int startY, int width, int height)
    {
        lock (TileLock)
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var tileX = startX + x;
                    var tileY = startY + y;
                    if (tileX >= 0 && tileX < Main.maxTilesX && tileY >= 0 && tileY < Main.maxTilesY)
                    {
                        Main.tile[tileX, tileY].wall = BaseWall;
                        WorldGen.paintWall(tileX, tileY, BaseColor, true);
                        WorldGen.paintCoatWall(tileX, tileY, 1, true);
                    }
                }
            }
        }
    
        const int chunkSize = 25;
        for (var cx = 0; cx < width; cx += chunkSize)
        {
            for (var cy = 0; cy < height; cy += chunkSize)
            {
                var chunkWidth = Math.Min(chunkSize, width - cx);
                var chunkHeight = Math.Min(chunkSize, height - cy);
                var size = Math.Max(chunkWidth, chunkHeight);
                TSPlayer.All.SendTileSquareCentered(
                    startX + cx + (chunkWidth / 2),
                    startY + cy + (chunkHeight / 2),
                    (byte)size);
            }
        }
    }

    internal void ClearPlaybackArea(PlaybackSession session)
    {
        var position = session.Position;
        var (startX, startY) = this.CalculateStartPosition(position, session.Video.Width, session.Video.Height);
    
        lock (TileLock)
        {
            for (var x = 0; x < session.Video.Width; x++)
            {
                for (var y = 0; y < session.Video.Height; y++)
                {
                    var tileX = startX + x;
                    var tileY = startY + y;
                    if (tileX >= 0 && tileX < Main.maxTilesX && tileY >= 0 && tileY < Main.maxTilesY)
                    {
                        Main.tile[tileX, tileY].wall = 0;
                    }
                }
            }
        }
    
        const int chunkSize = 25;
        for (var cx = 0; cx < session.Video.Width; cx += chunkSize)
        {
            for (var cy = 0; cy < session.Video.Height; cy += chunkSize)
            {
                var chunkWidth = Math.Min(chunkSize, session.Video.Width - cx);
                var chunkHeight = Math.Min(chunkSize, session.Video.Height - cy);
                var size = Math.Max(chunkWidth, chunkHeight);
                TSPlayer.All.SendTileSquareCentered(
                    startX + cx + (chunkWidth / 2),
                    startY + cy + (chunkHeight / 2),
                    (byte)size);
            }
        }
    }

    private (int startX, int startY) CalculateStartPosition(PositionData position, int width, int height)
    {
        return position.PositionType switch
        {
            "tl" => (position.X, position.Y - 1),
            "bl" => (position.X, position.Y - height),
            "tr" => (position.X - width + 1, position.Y - 1),
            "br" => (position.X - width + 1, position.Y - height),
            _ => (position.X, position.Y - 1)
        };
    }

    private void UpdateTile(int x, int y, bool isWhite)
    {
        if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
        {
            return;
        }
    
        lock (TileLock)
        {
            Main.tile[x, y].wall = (ushort)(isWhite ? BaseWall : CodeWall);
            WorldGen.paintWall(x, y, (byte)(isWhite ? BaseColor : CodeColor), true);
            WorldGen.paintCoatWall(x, y, IsGlowPaintApplied ? (byte)1 : (byte)0, true);
        }
    }

    internal string GetPositionName(string posType)
    {
        return posType switch
        {
            "tl" => GetString("左上角"),
            "bl" => GetString("左下角"),
            "tr" => GetString("右上角"),
            "br" => GetString("右下角"),
            _ => GetString("未知")
        };
    }
}