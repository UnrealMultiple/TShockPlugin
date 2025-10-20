using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Terraria;
using Terraria.ID;
using TShockAPI;
using MazeGenerator.Models;

namespace MazeGenerator.Services;

public class MazeBuilder : IDisposable
{
    private readonly Dictionary<string, MazeSession> _mazeSessions = new ();
    private Dictionary<string, PositionData> _positionData = new ();
    private readonly Dictionary<string, (string name, string positionType, string creator)> _tempPositionData = new ();

    private static readonly string PluginDataDir = Path.Combine(TShock.SavePath, "MazeGenerator");
    private static readonly string PositionDataPath = Path.Combine(PluginDataDir, "positions.json");
    private static readonly object TileLock = new ();

    public void Initialize()
    {
        Directory.CreateDirectory(PluginDataDir);
        this.LoadPositionData();
    }

    public void Dispose()
    {
        lock (this._lock)
        {
            this._mazeSessions.Clear();
            this._positionData.Clear();
            this._tempPositionData.Clear();
        }
    }

    public void Reload()
    {
        this.LoadPositionData();
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

    public async Task<bool> BuildMaze(TSPlayer player, string name, int size)
    {
        if (!this._positionData.TryGetValue(name, out var position))
        {
            return false;
        }

        // 确保迷宫大小为奇数
        if (size % 2 == 0)
        {
            size++;
        }

        var config = Config.Instance;

        // 检查是否已有会话
        if (this._mazeSessions.ContainsKey(name))
        {
            this.ClearMazeArea(this._mazeSessions[name]);
            this._mazeSessions.Remove(name);
        }

        // 创建会话并标记为生成中
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

        // 异步生成迷宫
        await Task.Run(() => this.GenerateMaze(player, name, position, size, session));
        return true;
    }

    private async Task GenerateMaze(TSPlayer player, string name, PositionData position, int size, MazeSession session)
    {
        try
        {
            player.SendInfoMessage($"正在生成 {size}x{size} 的迷宫...");

            // 计算起始位置
            var totalWidth = size * session.CellSize;
            var totalHeight = size * session.CellSize;
            var (startX, startY) = this.CalculateStartPosition(position, totalWidth, totalHeight);

            session.StartX = startX;
            session.StartY = startY;

            // 预清理区域并绘制背景
            this.ClearArea(startX, startY, totalWidth, totalHeight);
            this.DrawBackground(startX, startY, totalWidth, totalHeight);

            // 使用DFS算法生成迷宫
            await this.GenerateMazeDFSAnimated(player, startX, startY, size, session.CellSize);

            // 设置入口和出口
            session.Entrance = (startX + session.CellSize, startY);
            session.Exit = (startX + ((size - 2) * session.CellSize), startY + ((size - 1) * session.CellSize));
            session.IsGenerating = false;

            // 通知游戏管理器迷宫已就绪
            MazeGenerator.Instance.GameManager.NotifyMazeReady(name, session);

            player.SendSuccessMessage($"迷宫 '{name}' 生成完成！");
        }
        catch (Exception ex)
        {
            player.SendErrorMessage($"生成迷宫时发生错误: {ex.Message}");
            TShock.Log.Error($"[MazeGenerator] 生成迷宫错误: {ex}");
            session.IsGenerating = false;
        }
    }

    // 使用您提供的有效DFS迷宫生成算法
    private async Task GenerateMazeDFSAnimated(TSPlayer player, int startX, int startY, int size, int cellSize)
    {
        var config = Config.Instance;
        
        // 迷宫数据：true表示墙，false表示路径
        var maze = new bool[size, size];

        // 初始化所有位置为墙
        for (var x = 0; x < size; x++)
        for (var y = 0; y < size; y++)
        {
            maze[x, y] = true;
        }

        var random = new Random();
        var stack = new Stack<(int x, int y)>();
        var steps = 0;

        // 从起点开始（确保是奇数坐标）
        var startCellX = 1;
        var startCellY = 1;
        maze[startCellX, startCellY] = false;
        stack.Push((startCellX, startCellY));

        // 绘制起点
        this.DrawCell(startX, startY, startCellX, startCellY, cellSize, false);
        await Task.Delay(config.FrameDelay);

        // 方向数组：上、右、下、左
        int[] dx = { 0, 2, 0, -2 };
        int[] dy = { -2, 0, 2, 0 };

        // 用于可视化的当前路径颜色
        var pathTiles = new HashSet<(int, int)>();

        while (stack.Count > 0)
        {
            var current = stack.Peek();
            var neighbors = new List<int>();

            // 检查四个方向的邻居
            for (var i = 0; i < 4; i++)
            {
                var nx = current.x + dx[i];
                var ny = current.y + dy[i];

                if (nx > 0 && nx < size - 1 && ny > 0 && ny < size - 1 && maze[nx, ny])
                {
                    neighbors.Add(i);
                }
            }

            if (neighbors.Count > 0)
            {
                // 随机选择一个邻居
                var direction = neighbors[random.Next(neighbors.Count)];
                var wallX = current.x + (dx[direction] / 2);
                var wallY = current.y + (dy[direction] / 2);
                var nextX = current.x + dx[direction];
                var nextY = current.y + dy[direction];

                // 打通墙和邻居
                maze[wallX, wallY] = false;
                maze[nextX, nextY] = false;

                // 可视化：显示当前路径
                pathTiles.Add((current.x, current.y));
                pathTiles.Add((wallX, wallY));
                pathTiles.Add((nextX, nextY));

                // 绘制打通的过程
                this.DrawCell(startX, startY, wallX, wallY, cellSize, false);
                await Task.Delay(config.FrameDelay / 2);

                this.DrawCell(startX, startY, nextX, nextY, cellSize, false);
                await Task.Delay(config.FrameDelay / 2);

                // 可视化：更新当前路径显示
                foreach (var (px, py) in pathTiles)
                {
                    this.DrawCell(startX, startY, px, py, cellSize, false);
                }

                await Task.Delay(config.FrameDelay / 4);

                stack.Push((nextX, nextY));
                steps++;
            }
            else
            {
                // 回溯时从路径中移除当前点
                pathTiles.Remove((current.x, current.y));
                stack.Pop();
            }
        }

        // 创建入口和出口
        maze[1, 0] = false; // 入口
        maze[size - 2, size - 1] = false; // 出口

        // 绘制入口和出口
        this.DrawCell(startX, startY, 1, 0, cellSize, false);
        this.DrawCell(startX, startY, size - 2, size - 1, cellSize, false);

        // 最终更新整个区域
        this.UpdateRegion(startX, startY, size * cellSize, size * cellSize);
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
                        // 设置背景墙
                        Main.tile[tileX, tileY].wall = (ushort)config.BackgroundWall;
                        WorldGen.paintWall(tileX, tileY, (byte)config.BackgroundPaint, true);

                        // 初始化为墙壁
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
                            // 设置墙壁
                            Main.tile[tileX, tileY].active(true);
                            Main.tile[tileX, tileY].type = (ushort)config.MazeWallTile;
                        }
                        else
                        {
                            // 清除路径（会显示背景墙）
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

    public void ClearMaze(string name)
    {
        if (this._mazeSessions.TryGetValue(name, out var session))
        {
            this.ClearMazeArea(session);
            this._mazeSessions.Remove(name);
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

    // 修复的 DeletePosition 方法 - 确保总是清除方块
    public bool DeletePosition(string name)
    {
        lock (this._lock)
        {
            bool found = false;

            // 1. 如果有会话记录，清除迷宫方块
            if (this._mazeSessions.TryGetValue(name, out var session))
            {
                this.ClearMazeArea(session);
                this._mazeSessions.Remove(name);
                found = true;
            }

            // 2. 如果有位置数据，尝试根据位置数据清除区域（即使重启后）
            if (this._positionData.TryGetValue(name, out var position))
            {
                // 计算大致的迷宫区域并清除
                var config = Config.Instance;
                int estimatedSize = config.MaxSize; // 使用最大尺寸确保覆盖
                int totalWidth = estimatedSize * config.CellSize;
                int totalHeight = estimatedSize * config.CellSize;
                
                var (startX, startY) = this.CalculateStartPosition(position, totalWidth, totalHeight);
                this.ClearArea(startX, startY, totalWidth, totalHeight);
                
                this._positionData.Remove(name);
                found = true;
            }

            // 3. 如果有临时数据，也清除
            var tempEntries = this._tempPositionData.Where(x => x.Value.name == name).ToList();
            if (tempEntries.Count > 0)
            {
                foreach (var entry in tempEntries)
                {
                    this._tempPositionData.Remove(entry.Key);
                }
                found = true;
            }

            if (found)
            {
                this.SavePositionData();
            }

            return found;
        }
    }

    private readonly object _lock = new ();
}