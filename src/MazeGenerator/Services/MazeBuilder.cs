using System.Collections;
using Newtonsoft.Json;
using Terraria;
using TShockAPI;
using MazeGenerator.Models;

namespace MazeGenerator.Services;

public class MazeBuilder : IDisposable
{
    private readonly Dictionary<string, MazeSession> _mazeSessions = new();
    private Dictionary<string, PositionData> _positionData = new();
    private readonly Dictionary<string, (string name, string positionType, string creator)> _tempPositionData = new();
    private readonly PathFinder _pathFinder = new();

    private readonly Dictionary<string, bool> _pathDisplayInProgress = new();
    private readonly Dictionary<string, bool> _pathVisible = new();
    private readonly Dictionary<string, List<(int x, int y)>> _pathCells = new();

    private static readonly string PluginDataDir = Path.Combine(TShock.SavePath, "MazeGenerator");
    private static readonly string PositionDataPath = Path.Combine(PluginDataDir, "positions.json");
    private static readonly string SessionDataPath = Path.Combine(PluginDataDir, "sessions.json");
    private static readonly Lock TileLock = new();
    
    private readonly Lock _sessionLock = new ();
    private readonly Lock _positionLock = new ();
    private readonly Lock _pathLock = new ();

    public void Initialize()
    {
        Directory.CreateDirectory(PluginDataDir);
        this.LoadPositionData();
        this.LoadSessionData();
    }

    public void Dispose()
    {
        lock (this._sessionLock)
        lock (this._positionLock)
        lock (this._pathLock)
        {
            this.SaveSessionData();
            this.SavePositionData();

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
                lock (this._positionLock)
                {
                    this._positionData = JsonConvert.DeserializeObject<Dictionary<string, PositionData>>(
                        File.ReadAllText(PositionDataPath)) ?? new Dictionary<string, PositionData>();
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString($"[MazeGenerator] 加载位置数据失败: {ex.Message}"));
                lock (this._positionLock)
                {
                    this._positionData = new Dictionary<string, PositionData>();
                }
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

                lock (this._sessionLock)
                {
                    foreach (var session in sessions)
                    {
                        session.Value.IsGenerating = false;

                        if (!string.IsNullOrEmpty(session.Value.MazeDataBase64))
                        {
                            session.Value.MazeData = this.DecompressMazeData(session.Value.MazeDataBase64, session.Value.Size);
                        }

                        this._mazeSessions[session.Key] = session.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString($"[MazeGenerator] 加载会话数据失败: {ex.Message}"));
            }
        }
    }

    private void SaveSessionData()
    {
        try
        {
            Dictionary<string, MazeSession> sessionsToSave;

            lock (this._sessionLock)
            {
                sessionsToSave = new Dictionary<string, MazeSession>();
                foreach (var session in this._mazeSessions)
                {
                    var sessionCopy = new MazeSession
                    {
                        Name = session.Value.Name,
                        StartX = session.Value.StartX,
                        StartY = session.Value.StartY,
                        Size = session.Value.Size,
                        CellSize = session.Value.CellSize,
                        GeneratedTime = session.Value.GeneratedTime,
                        GeneratedBy = session.Value.GeneratedBy,
                        IsGenerating = session.Value.IsGenerating,
                        Entrance = session.Value.Entrance,
                        Exit = session.Value.Exit,
                        MazeData = session.Value.MazeData,
                        MazeDataBase64 = session.Value.MazeData != null ? this.CompressMazeData(session.Value.MazeData, session.Value.Size) : null
                    };
                    sessionsToSave[session.Key] = sessionCopy;
                }
            }

            File.WriteAllText(SessionDataPath, JsonConvert.SerializeObject(sessionsToSave, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"[MazeGenerator] 保存会话数据失败: {ex.Message}"));
        }
    }

    private void SavePositionData()
    {
        try
        {
            Dictionary<string, PositionData> positionsToSave;

            lock (this._positionLock)
            {
                positionsToSave = new Dictionary<string, PositionData>(this._positionData);
            }

            File.WriteAllText(PositionDataPath, JsonConvert.SerializeObject(positionsToSave, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"[MazeGenerator] 保存位置数据失败: {ex.Message}"));
        }
    }

    private string CompressMazeData(int[,] mazeData, int size)
    {
        try
        {
            var byteCount = ((size * size) + 7) / 8;
            var bitmap = new byte[byteCount];

            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var index = (y * size) + x;
                    var byteIndex = index / 8;
                    var bitIndex = index % 8;

                    if (mazeData[x, y] == 1)
                    {
                        bitmap[byteIndex] |= (byte)(1 << bitIndex);
                    }
                }
            }

            return Convert.ToBase64String(bitmap);
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[MazeGenerator] 压缩迷宫数据失败: {ex}"));
            return string.Empty;
        }
    }

    private int[,]? DecompressMazeData(string base64Data, int size)
    {
        try
        {
            var bitmap = Convert.FromBase64String(base64Data);
            var mazeData = new int[size, size];

            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var index = (y * size) + x;
                    var byteIndex = index / 8;
                    var bitIndex = index % 8;

                    if (byteIndex < bitmap.Length && (bitmap[byteIndex] & (1 << bitIndex)) != 0)
                    {
                        mazeData[x, y] = 1;
                    }
                    else
                    {
                        mazeData[x, y] = 0;
                    }
                }
            }

            return mazeData;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[MazeGenerator] 解压缩迷宫数据失败: {ex}"));
            return null;
        }
    }

    public bool SetPosition(string playerName, string name, string positionType)
    {
        lock (this._positionLock)
        {
            if (this._positionData.ContainsKey(name))
            {
                return false;
            }
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

            lock (this._positionLock)
            {
                this._positionData[name] = position;
            }

            this._tempPositionData.Remove(playerName);
            this.SavePositionData();
            return true;
        }

        return false;
    }

    public void BuildMaze(TSPlayer player, string name, int size)
    {
        PositionData position;
    
        lock (this._positionLock)
        {
            if (!this._positionData.TryGetValue(name, out position))
            {
                player.SendErrorMessage(GetString($"未找到位置 '{name}'"));
                return;
            }
        }

        if (size % 2 == 0)
        {
            size++;
        }

        var config = Config.Instance;

        lock (this._sessionLock)
        {
            if (this._mazeSessions.ContainsKey(name))
            {
                this.ClearMazeArea(this._mazeSessions[name]);
                this._mazeSessions.Remove(name);
            }

            lock (this._pathLock)
            {
                this._pathVisible.Remove(name);
                this._pathDisplayInProgress.Remove(name);
                this._pathCells.Remove(name);
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
    }

    private IEnumerator GenerateMazeCoroutine(TSPlayer player, string name, PositionData position, int size, MazeSession session)
    {
        return this.GenerateMazeCoroutineInternal(player, name, position, size, session);
    }

    private IEnumerator GenerateMazeCoroutineInternal(TSPlayer player, string name, PositionData position, int size, MazeSession session)
    {
        player.SendInfoMessage(GetString($"正在生成 {size}x{size} 的迷宫..."));

        var totalWidth = size * session.CellSize;
        var totalHeight = size * session.CellSize;
        var (startX, startY) = this.CalculateStartPosition(position, totalWidth, totalHeight);

        lock (this._sessionLock)
        {
            if (!this._mazeSessions.ContainsKey(name))
            {
                player.SendErrorMessage(GetString("迷宫生成被取消"));
                yield break;
            }

            session.StartX = startX;
            session.StartY = startY;
        }

        this.ClearArea(startX, startY, totalWidth, totalHeight);
        this.DrawBackground(startX, startY, totalWidth, totalHeight);

        int[,]? mazeData;
        try
        {
            mazeData = this.GenerateMazeData(size);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"[MazeGenerator] 生成迷宫数据时发生错误: {ex}"));
            player.SendErrorMessage(GetString("生成迷宫数据失败，请稍后重试"));
            yield break;
        }

        if (mazeData == null)
        {
            player.SendErrorMessage(GetString("生成迷宫数据失败"));
            yield break;
        }

        lock (this._sessionLock)
        {
            if (!this._mazeSessions.ContainsKey(name))
            {
                player.SendErrorMessage(GetString("迷宫生成被取消"));
                yield break;
            }

            session.MazeData = mazeData;
        }

        var drawCoroutine = this.DrawMazeCoroutineSafe(player, session, mazeData);
        while (drawCoroutine.MoveNext())
        {
            yield return drawCoroutine.Current;
        }

        lock (this._sessionLock)
        {
            if (!this._mazeSessions.ContainsKey(name))
            {
                player.SendErrorMessage(GetString("迷宫生成被取消"));
                yield break;
            }

            session.Entrance = (session.StartX, session.StartY + session.CellSize); 
            session.Exit = (session.StartX + ((size - 1) * session.CellSize), session.StartY + ((size - 2) * session.CellSize)); 
            session.IsGenerating = false;
        }

        this.SaveSessionData();

        player.SendSuccessMessage(GetString($"迷宫 '{name}' 生成完成！"));
        player.SendInfoMessage(GetString($"入口位置: ({session.Entrance.startX}, {session.Entrance.startY})"));

        MazeGenerator.Instance.GameManager.NotifyMazeReady(name, session);
    }

    private int[,]? GenerateMazeData(int size)
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

            maze[0, 1] = 0; 
            maze[size - 1, size - 2] = 0; 

            return maze;
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[MazeGenerator] 生成迷宫数据错误: {ex}"));
            return null;
        }
    }

    private IEnumerator DrawMazeCoroutineSafe(TSPlayer player, MazeSession session, int[,]? maze)
    {
        var steps = 0;
        var totalCells = session.Size * session.Size;
        var reportInterval = Math.Max(1, totalCells / 10);

        var maxCellsPerYield = session.Size < 20 ? int.MaxValue :
            session.Size < 50 ? 800 :
            session.Size < 80 ? 400 :
            200;

        var cellsInBatch = 0;
        int batchStartCellX = -1, batchStartCellY = -1, batchEndCellX = -1, batchEndCellY = -1;

        for (var x = 0; x < session.Size; x++)
        {
            for (var y = 0; y < session.Size; y++)
            {
                try
                {
                    this.DrawCell(session.StartX, session.StartY, x, y, session.CellSize, maze != null && maze[x, y] == 1);
                }
                catch (Exception ex)
                {
                    TShock.Log.Error(GetString($"[MazeGenerator] 绘制单元格时发生错误: {ex}"));
                }

                steps++;
                cellsInBatch++;

                batchStartCellX = batchStartCellX == -1 ? x : batchStartCellX;
                batchStartCellY = batchStartCellY == -1 ? y : batchStartCellY;
                batchEndCellX = Math.Max(batchEndCellX, x);
                batchEndCellY = Math.Max(batchEndCellY, y);

                if (cellsInBatch >= maxCellsPerYield || (x == session.Size - 1 && y == session.Size - 1))
                {
                    var rectX = session.StartX + (batchStartCellX * session.CellSize);
                    var rectY = session.StartY + (batchStartCellY * session.CellSize);
                    var rectW = (batchEndCellX - batchStartCellX + 1) * session.CellSize;
                    var rectH = (batchEndCellY - batchStartCellY + 1) * session.CellSize;

                    this.UpdateRegion(rectX, rectY, rectW, rectH);

                    cellsInBatch = 0;
                    batchStartCellX = batchStartCellY = batchEndCellX = batchEndCellY = -1;

                    if (steps % reportInterval == 0)
                    {
                        var progress = steps * 100 / totalCells;
                        player.SendInfoMessage(GetString($"迷宫绘制进度: {progress}%"));
                    }

                    if (maxCellsPerYield != int.MaxValue)
                    {
                        yield return null;
                    }
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
            player.SendErrorMessage(GetString($"未找到迷宫 '{name}'"));
            return;
        }

        if (session.IsGenerating)
        {
            player.SendErrorMessage(GetString($"迷宫 '{name}' 正在生成中"));
            return;
        }

        if (session.MazeData == null)
        {
            player.SendErrorMessage(GetString($"无法获取迷宫数据，请重新生成迷宫 '{name}'"));
            return;
        }

        lock (this._pathLock)
        {
            if (this._pathDisplayInProgress.ContainsKey(name) && this._pathDisplayInProgress[name])
            {
                player.SendErrorMessage(GetString($"路径操作进行中，请稍候"));
                return;
            }

            this._pathDisplayInProgress[name] = true;
        }

        Main.DelayedProcesses.Add(this.TogglePathCoroutine(player, name, session));
    }

    private IEnumerator TogglePathCoroutine(TSPlayer player, string name, MazeSession session)
    {
        return this.TogglePathCoroutineSafe(player, name, session);
    }

    private IEnumerator TogglePathCoroutineSafe(TSPlayer player, string name, MazeSession session)
    {
        bool isPathVisible;

        lock (this._pathLock)
        {
            isPathVisible = this._pathVisible.ContainsKey(name) && this._pathVisible[name];
        }

        if (isPathVisible)
        {
            player.SendInfoMessage(GetString($"隐藏路径中..."));
            var hideCoroutine = this.HidePathCoroutine(player, session);
            while (hideCoroutine.MoveNext())
            {
                yield return hideCoroutine.Current;
            }

            lock (this._pathLock)
            {
                this._pathVisible[name] = false;
            }

            player.SendSuccessMessage(GetString($"路径已隐藏"));
        }
        else
        {
            player.SendInfoMessage(GetString($"显示路径中..."));
            if (session.MazeData != null)
            {
                var showCoroutine = this.ShowPathCoroutine(player, session, session.MazeData);
                while (showCoroutine.MoveNext())
                {
                    yield return showCoroutine.Current;
                }
            }

            lock (this._pathLock)
            {
                this._pathVisible[name] = true;
            }

            player.SendSuccessMessage(GetString($"路径显示完成"));
        }

        lock (this._pathLock)
        {
            this._pathDisplayInProgress[name] = false;
        }
    }

    private IEnumerator ShowPathCoroutine(TSPlayer player, MazeSession session, int[,] maze)
    {
        List<(int x, int y)> path;
        try
        {
            path = this._pathFinder.FindPath(session, maze);
        }
        catch (Exception ex)
        {
            TShock.Log.Error(GetString($"[MazeGenerator] 查找路径时发生错误: {ex}"));
            player.SendErrorMessage(GetString("查找路径失败"));
            yield break;
        }

        var config = Config.Instance;

        if (path.Count == 0)
        {
            player.SendErrorMessage(GetString("找不到路径"));
            yield break;
        }

        lock (this._pathLock)
        {
            this._pathCells[session.Name] = new List<(int x, int y)>();
        }

        var batchSize = path.Count < 200 ? 200 :
            path.Count < 800 ? 300 :
            path.Count < 2000 ? 600 :
            800;

        var paintedInBatch = 0;
        int batchStartCellX = -1, batchStartCellY = -1, batchEndCellX = -1, batchEndCellY = -1;

        for (var i = 0; i < path.Count; i++)
        {
            var cell = path[i];

            lock (this._pathLock)
            {
                this._pathCells[session.Name].Add((cell.x, cell.y));
            }

            try
            {
                this.PaintPathCell(session, cell.x, cell.y, config.PathPaint);
            }
            catch (Exception ex)
            {
                TShock.Log.Error(GetString($"[MazeGenerator] 绘制路径单元格时发生错误: {ex}"));
            }

            batchStartCellX = batchStartCellX == -1 ? cell.x : batchStartCellX;
            batchStartCellY = batchStartCellY == -1 ? cell.y : batchStartCellY;
            batchEndCellX = Math.Max(batchEndCellX, cell.x);
            batchEndCellY = Math.Max(batchEndCellY, cell.y);

            paintedInBatch++;

            if (paintedInBatch >= batchSize || i == path.Count - 1)
            {
                if (batchStartCellX != -1)
                {
                    this.UpdateRegion(
                        session.StartX + (batchStartCellX * session.CellSize),
                        session.StartY + (batchStartCellY * session.CellSize),
                        (batchEndCellX - batchStartCellX + 1) * session.CellSize,
                        (batchEndCellY - batchStartCellY + 1) * session.CellSize
                    );
                }

                paintedInBatch = 0;
                batchStartCellX = batchStartCellY = batchEndCellX = batchEndCellY = -1;

                yield return null;
            }
        }

        this.UpdateRegion(session.StartX, session.StartY,
            session.Size * session.CellSize,
            session.Size * session.CellSize);
    }

    private IEnumerator HidePathCoroutine(TSPlayer player, MazeSession session)
    {
        var config = Config.Instance;

        List<(int x, int y)> pathCells;
        lock (this._pathLock)
        {
            if (!this._pathCells.ContainsKey(session.Name))
            {
                player.SendErrorMessage(GetString("没有找到路径记录，无法隐藏"));
                yield break;
            }
            pathCells = this._pathCells[session.Name];
        }

        var total = pathCells.Count;
        var batchSize = total < 200 ? 400 :
            total < 800 ? 600 :
            total < 2000 ? 800 :
            1200;

        var restoredInBatch = 0;
        int batchStartCellX = -1, batchStartCellY = -1, batchEndCellX = -1, batchEndCellY = -1;

        for (var i = 0; i < pathCells.Count; i++)
        {
            var cell = pathCells[i];

            try
            {
                this.PaintPathCell(session, cell.x, cell.y, config.BackgroundPaint);
            }
            catch (Exception ex)
            {
                TShock.Log.Error(GetString($"[MazeGenerator] 恢复路径单元格时发生错误: {ex}"));
            }

            if (batchStartCellX == -1)
            {
                batchStartCellX = batchEndCellX = cell.x;
                batchStartCellY = batchEndCellY = cell.y;
            }
            else
            {
                batchEndCellX = Math.Max(batchEndCellX, cell.x);
                batchEndCellY = Math.Max(batchEndCellY, cell.y);
            }

            restoredInBatch++;

            if (restoredInBatch >= batchSize || i == pathCells.Count - 1)
            {
                this.UpdateRegion(
                    session.StartX + (batchStartCellX * session.CellSize),
                    session.StartY + (batchStartCellY * session.CellSize),
                    (batchEndCellX - batchStartCellX + 1) * session.CellSize,
                    (batchEndCellY - batchStartCellY + 1) * session.CellSize
                );

                restoredInBatch = 0;
                batchStartCellX = batchStartCellY = batchEndCellX = batchEndCellY = -1;
                yield return null;
            }
        }

        lock (this._pathLock)
        {
            this._pathCells.Remove(session.Name);
        }

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
                        if (!Main.tile[tileX, tileY].active())
                        {
                            WorldGen.paintWall(tileX, tileY, (byte)paintId, true);
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
            player.SendErrorMessage(GetString($"未找到迷宫 '{name}'"));
            return;
        }

        player.SendInfoMessage(GetString($"正在重置迷宫 '{name}'..."));

        MazeGenerator.Instance.GameManager.LeaveAllPlayersFromMaze(name);

        lock (this._pathLock)
        {
            this._pathVisible.Remove(name);
            this._pathDisplayInProgress.Remove(name);
            this._pathCells.Remove(name);
        }

        lock (this._positionLock)
        {
            if (this._positionData.TryGetValue(name, out _))
            {
                this.BuildMaze(player, name, session.Size);
            }
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
                        Main.tile[tileX, tileY].wall = (ushort)config.BackgroundWall;
                        WorldGen.paintWall(tileX, tileY, (byte)config.BackgroundPaint, true);
                        Main.tile[tileX, tileY].active(true);
                        Main.tile[tileX, tileY].type = (ushort)config.MazeWallTile;
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
                            Main.tile[tileX, tileY].type = (ushort)config.MazeWallTile;
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
                    (byte)size);
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
        lock (this._sessionLock)
        {
            return this._mazeSessions.GetValueOrDefault(name);
        }
    }

    public Dictionary<string, PositionData> GetPositions()
    {
        lock (this._positionLock)
        {
            return new Dictionary<string, PositionData>(this._positionData);
        }
    }

    public bool DeletePosition(string name)
    {
        var found = false;

        lock (this._sessionLock)
        lock (this._positionLock)
        lock (this._pathLock)
        {
            if (this._mazeSessions.TryGetValue(name, out var session))
            {
                this.ClearMazeArea(session);
                this._mazeSessions.Remove(name);
                found = true;
            }

            if (this._positionData.TryGetValue(name, out var position))
            {
                var config = Config.Instance;

                var sizeToClear = session?.Size ?? config.DefaultSize;
            
                var totalWidth = sizeToClear * config.CellSize;
                var totalHeight = sizeToClear * config.CellSize;

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