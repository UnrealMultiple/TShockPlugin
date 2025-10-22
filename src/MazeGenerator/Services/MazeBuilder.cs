using System.Collections;
using Newtonsoft.Json;
using Terraria;
using TShockAPI;
using MazeGenerator.Models;

namespace MazeGenerator.Services;

public class MazeBuilder : IDisposable
{
    private readonly Dictionary<string, MazeSession> _mazeSessions = new ();
    private Dictionary<string, PositionData> _positionData = new ();
    private readonly Dictionary<string, (string name, string positionType, string creator)> _tempPositionData = new ();
    private readonly PathFinder _pathFinder = new ();

    private readonly Dictionary<string, bool> _pathDisplayInProgress = new ();
    private readonly Dictionary<string, bool> _pathVisible = new ();
    private readonly Dictionary<string, List<(int x, int y)>> _pathCells = new ();

    private static readonly string PluginDataDir = Path.Combine(TShock.SavePath, "MazeGenerator");
    private static readonly string PositionDataPath = Path.Combine(PluginDataDir, "positions.json");
    private static readonly string SessionDataPath = Path.Combine(PluginDataDir, "sessions.json");
    private static readonly Lock TileLock = new ();
    private readonly Lock _lock = new ();

    public void Initialize()
    {
        Directory.CreateDirectory(PluginDataDir);
        this.LoadPositionData();
        this.LoadSessionData();
    }

    public void Dispose()
    {
        lock (this._lock)
        {
            this.SaveSessionData();
            this._mazeSessions.Clear();
            this._positionData.Clear();
            this._tempPositionData.Clear();
            this._pathDisplayInProgress.Clear();
            this._pathVisible.Clear();
            this._pathCells.Clear();
        }
    }

    public void Reload()
    {
        this.LoadPositionData();
        this.LoadSessionData();
    }

    private void LoadPositionData()
    {
        if (File.Exists(PositionDataPath))
        {
            try
            {
                this._positionData = JsonConvert.DeserializeObject<Dictionary<string, PositionData>>(
                    File.ReadAllText(PositionDataPath)) ?? new Dictionary<string, PositionData>();
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[MazeGenerator] 加载位置数据失败: {ex.Message}");
                this._positionData = new Dictionary<string, PositionData>();
            }
        }
    }

    private void LoadSessionData()
    {
        if (File.Exists(SessionDataPath))
        {
            try
            {
                var sessions = JsonConvert.DeserializeObject<Dictionary<string, MazeSession>>(
                    File.ReadAllText(SessionDataPath)) ?? new Dictionary<string, MazeSession>();

                foreach (var session in sessions)
                {
                    session.Value.IsGenerating = false;
                    this._mazeSessions[session.Key] = session.Value;
                }

                TShock.Log.ConsoleInfo($"[MazeGenerator] 已加载 {sessions.Count} 个迷宫会话");
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[MazeGenerator] 加载会话数据失败: {ex.Message}");
            }
        }
    }

    private void SaveSessionData()
    {
        try
        {
            File.WriteAllText(SessionDataPath, JsonConvert.SerializeObject(this._mazeSessions, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError($"[MazeGenerator] 保存会话数据失败: {ex.Message}");
        }
    }

    private void SavePositionData()
    {
        try
        {
            File.WriteAllText(PositionDataPath, JsonConvert.SerializeObject(this._positionData, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError($"[MazeGenerator] 保存位置数据失败: {ex.Message}");
        }
    }

    public bool SetPosition(string playerName, string name, string positionType)
    {
        if (this._positionData.ContainsKey(name))
        {
            return false;
        }

        this._tempPositionData[playerName] = (name, positionType, playerName);
        return true;
    }

    public bool HandleTileEdit(string playerName, int x, int y)
    {
        if (this._tempPositionData.TryGetValue(playerName, out var tempData))
        {
            var (name, positionType, creator) = tempData;
            var position = new PositionData { X = x, Y = y, PositionType = positionType, Creator = creator };

            this._positionData[name] = position;
            this._tempPositionData.Remove(playerName);
            this.SavePositionData();
            return true;
        }

        return false;
    }

    public void BuildMaze(TSPlayer player, string name, int size)
    {
        if (!this._positionData.TryGetValue(name, out var position))
        {
            player.SendErrorMessage($"未找到位置 '{name}'");
            return;
        }

        if (size % 2 == 0)
        {
            size++;
        }

        var config = Config.Instance;

        if (this._mazeSessions.ContainsKey(name))
        {
            this.ClearMazeArea(this._mazeSessions[name]);
            this._mazeSessions.Remove(name);

            lock (this._lock)
            {
                this._pathVisible.Remove(name);
                this._pathDisplayInProgress.Remove(name);
                this._pathCells.Remove(name);
            }
        }

        var session = new MazeSession
        {
            Name = name,
            Size = size,
            CellSize = config.CellSize,
            GeneratedTime = DateTime.Now,
            GeneratedBy = player.Name,
            IsGenerating = true
        };

        this._mazeSessions[name] = session;

        Main.DelayedProcesses.Add(this.GenerateMazeCoroutine(player, name, position, size, session));
    }

    private IEnumerator GenerateMazeCoroutine(TSPlayer player, string name, PositionData position, int size, MazeSession session)
    {
        player.SendInfoMessage($"正在生成 {size}x{size} 的迷宫...");

        var totalWidth = size * session.CellSize;
        var totalHeight = size * session.CellSize;
        var (startX, startY) = this.CalculateStartPosition(position, totalWidth, totalHeight);

        session.StartX = startX;
        session.StartY = startY;

        this.ClearArea(startX, startY, totalWidth, totalHeight);
        this.DrawBackground(startX, startY, totalWidth, totalHeight);

        var mazeData = this.GenerateMazeData(size);
        if (mazeData == null)
        {
            player.SendErrorMessage("迷宫数据生成失败");
            session.IsGenerating = false;
            this.SaveSessionData();
            yield break;
        }

        session.MazeData = mazeData;

        var drawCoroutine = this.DrawMazeCoroutine(player, session, mazeData);
        while (drawCoroutine.MoveNext())
        {
            yield return drawCoroutine.Current;
        }

        session.Entrance = (session.StartX + session.CellSize, session.StartY);
        session.Exit = (session.StartX + ((size - 2) * session.CellSize), session.StartY + ((size - 1) * session.CellSize));
        session.IsGenerating = false;

        this.SaveSessionData();

        player.SendSuccessMessage($"迷宫 '{name}' 生成完成！");
        player.SendInfoMessage($"入口位置: ({session.Entrance.startX}, {session.Entrance.startY})");

        MazeGenerator.Instance.GameManager.NotifyMazeReady(name, session);
    }

    private int[,] GenerateMazeData(int size)
    {
        try
        {
            var maze = new int[size, size];
            var random = new Random(Guid.NewGuid().GetHashCode());

            for (var x = 0; x < size; x++)
            for (var y = 0; y < size; y++)
            {
                maze[x, y] = 1;
            }

            var stack = new Stack<(int x, int y)>();

            var startCellX = 1;
            var startCellY = 1;
            maze[startCellX, startCellY] = 0;
            stack.Push((startCellX, startCellY));

            int[] dx = { 0, 2, 0, -2 };
            int[] dy = { -2, 0, 2, 0 };

            while (stack.Count > 0)
            {
                var current = stack.Peek();
                var neighbors = new List<int>();

                for (var i = 0; i < 4; i++)
                {
                    var nx = current.x + dx[i];
                    var ny = current.y + dy[i];

                    if (nx > 0 && nx < size - 1 && ny > 0 && ny < size - 1 && maze[nx, ny] == 1)
                    {
                        neighbors.Add(i);
                    }
                }

                if (neighbors.Count > 0)
                {
                    var direction = neighbors[random.Next(neighbors.Count)];
                    var wallX = current.x + (dx[direction] / 2);
                    var wallY = current.y + (dy[direction] / 2);
                    var nextX = current.x + dx[direction];
                    var nextY = current.y + dy[direction];

                    maze[wallX, wallY] = 0;
                    maze[nextX, nextY] = 0;

                    stack.Push((nextX, nextY));
                }
                else
                {
                    stack.Pop();
                }
            }

            maze[1, 0] = 0;
            maze[size - 2, size - 1] = 0;

            return maze;
        }
        catch (Exception ex)
        {
            TShock.Log.Error($"[MazeGenerator] 生成迷宫数据错误: {ex}");
            return null;
        }
    }

    private IEnumerator DrawMazeCoroutine(TSPlayer player, MazeSession session, int[,] maze)
    {
        var steps = 0;
        var totalCells = session.Size * session.Size;
        var reportInterval = Math.Max(1, totalCells / 10);

        for (var x = 0; x < session.Size; x++)
        {
            for (var y = 0; y < session.Size; y++)
            {
                this.DrawCell(session.StartX, session.StartY, x, y, session.CellSize, maze[x, y] == 1);
                steps++;

                if (steps % reportInterval == 0)
                {
                    var progress = steps * 100 / totalCells;
                    player.SendInfoMessage($"迷宫绘制进度: {progress}%");
                    yield return null;
                }
            }
        }

        this.UpdateRegion(session.StartX, session.StartY,
            session.Size * session.CellSize,
            session.Size * session.CellSize);
    }

    public void TogglePathCommand(TSPlayer player, string name)
    {
        var session = this.GetSession(name);
        if (session == null)
        {
            player.SendErrorMessage($"未找到迷宫 '{name}'");
            return;
        }

        if (session.IsGenerating)
        {
            player.SendErrorMessage($"迷宫 '{name}' 正在生成中");
            return;
        }

        if (session.MazeData == null)
        {
            player.SendErrorMessage($"无法获取迷宫数据，请重新生成迷宫 '{name}'");
            return;
        }

        lock (this._lock)
        {
            if (this._pathDisplayInProgress.ContainsKey(name) && this._pathDisplayInProgress[name])
            {
                player.SendErrorMessage($"路径操作进行中，请稍候");
                return;
            }

            this._pathDisplayInProgress[name] = true;
        }

        Main.DelayedProcesses.Add(this.TogglePathCoroutine(player, name, session));
    }

    private IEnumerator TogglePathCoroutine(TSPlayer player, string name, MazeSession session)
    {
        var isPathVisible = this._pathVisible.ContainsKey(name) && this._pathVisible[name];

        if (isPathVisible)
        {
            player.SendInfoMessage($"隐藏路径中...");
            var hideCoroutine = this.HidePathCoroutine(player, session);
            while (hideCoroutine.MoveNext())
            {
                yield return hideCoroutine.Current;
            }

            this._pathVisible[name] = false;
            player.SendSuccessMessage($"路径已隐藏");
        }
        else
        {
            player.SendInfoMessage($"显示路径中...");
            var showCoroutine = this.ShowPathCoroutine(player, session, session.MazeData);
            while (showCoroutine.MoveNext())
            {
                yield return showCoroutine.Current;
            }

            this._pathVisible[name] = true;
            player.SendSuccessMessage($"路径显示完成");
        }

        lock (this._lock)
        {
            this._pathDisplayInProgress[name] = false;
        }
    }

    private IEnumerator ShowPathCoroutine(TSPlayer player, MazeSession session, int[,] maze)
    {
        var path = this._pathFinder.FindPath(session, maze);
        var config = Config.Instance;

        if (path.Count == 0)
        {
            player.SendErrorMessage("找不到路径");
            yield break;
        }

        player.SendInfoMessage($"显示路径中 ({path.Count} 步)...");

        this._pathCells[session.Name] = new List<(int x, int y)>();

        for (var i = 0; i < path.Count; i++)
        {
            var cell = path[i];

            this._pathCells[session.Name].Add((cell.x, cell.y));

            this.PaintPathCell(session, cell.x, cell.y, config.PathPaint);

            if (i % 5 == 0 || i == path.Count - 1)
            {
                this.UpdateRegion(session.StartX, session.StartY,
                    session.Size * session.CellSize,
                    session.Size * session.CellSize);
            }

            if (i % 10 == 0)
            {
                yield return null;
            }

            if (i % 20 == 0 || i == path.Count - 1)
            {
                player.SendInfoMessage($"路径显示进度: {i + 1}/{path.Count} ({(i + 1) * 100 / path.Count}%)");
            }
        }

        this.UpdateRegion(session.StartX, session.StartY,
            session.Size * session.CellSize,
            session.Size * session.CellSize);
    }

    private IEnumerator HidePathCoroutine(TSPlayer player, MazeSession session)
    {
        var config = Config.Instance;

        if (!this._pathCells.ContainsKey(session.Name))
        {
            player.SendErrorMessage("没有找到路径记录，无法隐藏");
            yield break;
        }

        var pathCells = this._pathCells[session.Name];
        player.SendInfoMessage($"正在隐藏 {pathCells.Count} 个路径单元格...");

        for (var i = 0; i < pathCells.Count; i++)
        {
            var cell = pathCells[i];

            this.PaintPathCell(session, cell.x, cell.y, config.BackgroundPaint);

            if (i % 10 == 0 || i == pathCells.Count - 1)
            {
                this.UpdateRegion(session.StartX, session.StartY,
                    session.Size * session.CellSize,
                    session.Size * session.CellSize);
            }

            if (i % 50 == 0)
            {
                yield return null;
            }

            if (i % 100 == 0 || i == pathCells.Count - 1)
            {
                player.SendInfoMessage($"隐藏进度: {i + 1}/{pathCells.Count} ({(i + 1) * 100 / pathCells.Count}%)");
            }
        }

        this._pathCells.Remove(session.Name);

        this.UpdateRegion(session.StartX, session.StartY,
            session.Size * session.CellSize,
            session.Size * session.CellSize);
    }

    private void PaintPathCell(MazeSession session, int cellX, int cellY, int paintId)
    {
        lock (TileLock)
        {
            for (var dx = 0; dx < session.CellSize; dx++)
            {
                for (var dy = 0; dy < session.CellSize; dy++)
                {
                    var tileX = session.StartX + (cellX * session.CellSize) + dx;
                    var tileY = session.StartY + (cellY * session.CellSize) + dy;

                    if (tileX >= 0 && tileX < Main.maxTilesX && tileY >= 0 && tileY < Main.maxTilesY)
                    {
                        // 关键：严格按照旧代码，只在非激活图格上刷油漆
                        if (!Main.tile[tileX, tileY].active())
                        {
                            WorldGen.paintWall(tileX, tileY, (byte) paintId, true);
                        }
                    }
                }
            }
        }
    }

    public void ResetMaze(TSPlayer player, string name)
    {
        var session = this.GetSession(name);
        if (session == null)
        {
            player.SendErrorMessage($"未找到迷宫 '{name}'");
            return;
        }

        player.SendInfoMessage($"正在重置迷宫 '{name}'...");

        MazeGenerator.Instance.GameManager.LeaveAllPlayersFromMaze(name);

        lock (this._lock)
        {
            this._pathVisible.Remove(name);
            this._pathDisplayInProgress.Remove(name);
            this._pathCells.Remove(name);
        }

        if (this._positionData.TryGetValue(name, out var position))
        {
            // 重新生成迷宫
            this.BuildMaze(player, name, session.Size);
        }
    }

    private void DrawBackground(int startX, int startY, int width, int height)
    {
        var config = Config.Instance;
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
                        Main.tile[tileX, tileY].wall = (ushort) config.BackgroundWall;
                        WorldGen.paintWall(tileX, tileY, (byte) config.BackgroundPaint, true);
                        Main.tile[tileX, tileY].active(true);
                        Main.tile[tileX, tileY].type = (ushort) config.MazeWallTile;
                    }
                }
            }
        }

        this.UpdateRegion(startX, startY, width, height);
    }

    private void DrawCell(int startX, int startY, int cellX, int cellY, int cellSize, bool isWall)
    {
        var config = Config.Instance;
        lock (TileLock)
        {
            for (var dx = 0; dx < cellSize; dx++)
            {
                for (var dy = 0; dy < cellSize; dy++)
                {
                    var tileX = startX + (cellX * cellSize) + dx;
                    var tileY = startY + (cellY * cellSize) + dy;

                    if (tileX >= 0 && tileX < Main.maxTilesX && tileY >= 0 && tileY < Main.maxTilesY)
                    {
                        if (isWall)
                        {
                            Main.tile[tileX, tileY].active(true);
                            Main.tile[tileX, tileY].type = (ushort) config.MazeWallTile;
                        }
                        else
                        {
                            Main.tile[tileX, tileY].active(false);
                        }
                    }
                }
            }
        }

        this.UpdateRegion(startX + (cellX * cellSize), startY + (cellY * cellSize), cellSize, cellSize);
    }

    private (int startX, int startY) CalculateStartPosition(PositionData position, int width, int height)
    {
        return position.PositionType switch
        {
            "tl" => (position.X, position.Y),
            "bl" => (position.X, position.Y - height + 1),
            "tr" => (position.X - width + 1, position.Y),
            "br" => (position.X - width + 1, position.Y - height + 1),
            _ => (position.X, position.Y)
        };
    }

    private void UpdateRegion(int x, int y, int width, int height)
    {
        const int maxChunkSize = 15;
        for (var cx = 0; cx < width; cx += maxChunkSize)
        {
            for (var cy = 0; cy < height; cy += maxChunkSize)
            {
                var chunkWidth = Math.Min(maxChunkSize, width - cx);
                var chunkHeight = Math.Min(maxChunkSize, height - cy);
                var size = Math.Max(chunkWidth, chunkHeight);

                TSPlayer.All.SendTileSquareCentered(
                    x + cx + (chunkWidth / 2),
                    y + cy + (chunkHeight / 2),
                    (byte) size);
            }
        }
    }

    private void ClearMazeArea(MazeSession session)
    {
        var totalWidth = session.Size * session.CellSize;
        var totalHeight = session.Size * session.CellSize;
        this.ClearArea(session.StartX, session.StartY, totalWidth, totalHeight);
    }

    private void ClearArea(int startX, int startY, int width, int height)
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
                        Main.tile[tileX, tileY].active(false);
                        Main.tile[tileX, tileY].wall = 0;
                    }
                }
            }
        }

        this.UpdateRegion(startX, startY, width, height);
    }

    public MazeSession? GetSession(string name)
    {
        return this._mazeSessions.TryGetValue(name, out var session) ? session : null;
    }

    public Dictionary<string, PositionData> GetPositions()
    {
        return new Dictionary<string, PositionData>(this._positionData);
    }

    public bool DeletePosition(string name)
    {
        lock (this._lock)
        {
            var found = false;

            if (this._mazeSessions.TryGetValue(name, out var session))
            {
                this.ClearMazeArea(session);
                this._mazeSessions.Remove(name);
                found = true;
            }

            if (this._positionData.TryGetValue(name, out var position))
            {
                var config = Config.Instance;
                var estimatedSize = config.MaxSize;
                var totalWidth = estimatedSize * config.CellSize;
                var totalHeight = estimatedSize * config.CellSize;

                var (startX, startY) = this.CalculateStartPosition(position, totalWidth, totalHeight);
                this.ClearArea(startX, startY, totalWidth, totalHeight);

                this._positionData.Remove(name);
                found = true;
            }

            var tempEntries = this._tempPositionData.Where(x => x.Value.name == name).ToList();
            if (tempEntries.Count > 0)
            {
                foreach (var entry in tempEntries)
                {
                    this._tempPositionData.Remove(entry.Key);
                }

                found = true;
            }

            this._pathVisible.Remove(name);
            this._pathDisplayInProgress.Remove(name);
            this._pathCells.Remove(name);

            if (found)
            {
                this.SavePositionData();
                this.SaveSessionData();
            }

            return found;
        }
    }
}