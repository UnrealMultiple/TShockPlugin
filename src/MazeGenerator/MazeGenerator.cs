using Newtonsoft.Json;
using System.Reflection;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Maze;

[ApiVersion(2, 1)]
public class MazeGenerator : TerrariaPlugin
{
    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "Eustia";
    public override string Description => GetString("迷宫生成器");
    public override Version Version => new (1, 1, 0, 0);

    // 图格ID定义
    private const int MazeWallTile = TileID.DiamondGemspark; // 钻石晶莹宝石块图格ID
    private const int BackgroundWall = WallID.Stone; // 背景墙
    private const int BackgroundPaint = PaintID.BlackPaint; // 黑色油漆

    private readonly Dictionary<string, MazeSession> _mazeSessions = new ();
    private static Dictionary<string, PositionData> _positionData = new ();
    private static readonly Dictionary<string, (string name, string positionType, string creator)> _tempPositionData = new ();

    private static readonly string PluginDataDir = Path.Combine(TShock.SavePath, "MazeGenerator");
    private static readonly string PositionDataPath = Path.Combine(PluginDataDir, "positions.json");
    private static readonly string ConfigPath = Path.Combine(PluginDataDir, "config.json");

    private static readonly object TileLock = new ();

    public MazeGenerator(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Directory.CreateDirectory(PluginDataDir);
        this.LoadPositionData();
        this.LoadConfig();
        this.RegisterCommands();

        GetDataHandlers.TileEdit += this.OnTileEdit;
        GeneralHooks.ReloadEvent += this.OnReload;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.TileEdit -= this.OnTileEdit;
            GeneralHooks.ReloadEvent -= this.OnReload;
            this.UnregisterCommands();
        }

        base.Dispose(disposing);
    }

    private void RegisterCommands()
    {
        Commands.ChatCommands.Add(new Command("maze.generate", this.MazeCommand, "maze") { HelpText = GetString("迷宫生成器命令，输入 /maze help 查看帮助") });
    }

    private void UnregisterCommands()
    {
        Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.MazeCommand);
    }

    private void OnReload(ReloadEventArgs e)
    {
        this.LoadPositionData();
        this.LoadConfig();
        e.Player.SendSuccessMessage(GetString("[MazeGenerator] 插件已重新加载!"));
    }

    private void OnTileEdit(object? sender, GetDataHandlers.TileEditEventArgs args)
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

        if (_tempPositionData.TryGetValue(player.Name, out var tempData))
        {
            var (name, positionType, creator) = tempData;

            var position = new PositionData { X = args.X, Y = args.Y, PositionType = positionType, Creator = creator };

            _positionData[name] = position;
            _tempPositionData.Remove(player.Name);
            this.SavePositionData();

            player.SendSuccessMessage(GetString($"迷宫位置 '{name}' 已设置: X={args.X} Y={args.Y} (对齐方式: {positionType})"));
            player.SendSuccessMessage(GetString($"现在可以使用 /maze play {name} [大小] 生成迷宫了"));

            args.Handled = true;
        }
    }

    private void LoadPositionData()
    {
        if (File.Exists(PositionDataPath))
        {
            try
            {
                _positionData = JsonConvert.DeserializeObject<Dictionary<string, PositionData>>(
                    File.ReadAllText(PositionDataPath)) ?? new Dictionary<string, PositionData>();
                TShock.Log.ConsoleInfo(GetString($"[MazeGenerator] 已加载 {_positionData.Count} 个位置"));
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString($"[MazeGenerator] 加载位置数据失败: {ex.Message}"));
                _positionData = new Dictionary<string, PositionData>();
            }
        }
    }

    private void SavePositionData()
    {
        try
        {
            File.WriteAllText(PositionDataPath, JsonConvert.SerializeObject(_positionData, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"[MazeGenerator] 保存位置数据失败: {ex.Message}"));
        }
    }

    private static PluginConfig _config = new ();

    private void LoadConfig()
    {
        if (File.Exists(ConfigPath))
        {
            try
            {
                _config = JsonConvert.DeserializeObject<PluginConfig>(File.ReadAllText(ConfigPath)) ?? new PluginConfig();
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString($"[MazeGenerator] 加载配置失败: {ex.Message}"));
                _config = new PluginConfig();
            }
        }

        this.SaveConfig();
    }

    private void SaveConfig()
    {
        try
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_config, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"[MazeGenerator] 保存配置失败: {ex.Message}"));
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
            case "position":
                if (!player.HasPermission("maze.admin"))
                {
                    player.SendErrorMessage(GetString("你没有管理员权限！"));
                    return;
                }

                this.HandleSetPosition(args);
                break;
            case "play":
            case "generate":
                this.HandleGenerateMaze(args);
                break;
            case "reset":
                this.HandleResetMaze(args);
                break;
            case "clear":
                this.HandleClearMaze(args);
                break;
            case "list":
                this.HandleListPositions(args);
                break;
            case "del":
            case "delete":
                if (!player.HasPermission("maze.admin"))
                {
                    player.SendErrorMessage(GetString("你没有管理员权限！"));
                    return;
                }

                this.HandleDeletePosition(args);
                break;
            case "config":
                if (!player.HasPermission("maze.admin"))
                {
                    player.SendErrorMessage(GetString("你没有管理员权限！"));
                    return;
                }

                this.HandleConfig(args);
                break;
            default:
                player.SendErrorMessage(GetString($"未知的子命令: {subCommand}"));
                player.SendInfoMessage(GetString("输入 /maze help 查看帮助"));
                break;
        }
    }

    private void ShowHelp(TSPlayer player)
    {
        player.SendInfoMessage(GetString("=== 迷宫生成器 ==="));
        player.SendInfoMessage(GetString("/maze play <名称> [大小] - 生成迷宫"));
        player.SendInfoMessage(GetString("/maze reset <名称> - 重置迷宫（重新随机生成）"));
        player.SendInfoMessage(GetString("/maze clear <名称> - 清除迷宫"));
        player.SendInfoMessage(GetString("/maze list - 列出所有位置"));

        if (player.HasPermission("maze.admin"))
        {
            player.SendInfoMessage(GetString("/maze pos <名称> <tl|bl|tr|br> - 设置迷宫位置"));
            player.SendInfoMessage(GetString("/maze del <名称> - 删除位置"));
            player.SendInfoMessage(GetString("/maze config <键> <值> - 设置配置"));
        }

        player.SendInfoMessage(GetString($"最小迷宫大小: {_config.MinSize}, 最大迷宫大小: {_config.MaxSize}"));
        player.SendInfoMessage(GetString($"单元格大小: {_config.CellSize}x{_config.CellSize} 格"));
        player.SendInfoMessage(GetString($"生成速度: {_config.FrameDelay} 毫秒/步"));
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

        if (_positionData.ContainsKey(name))
        {
            player.SendErrorMessage(GetString($"位置名称 '{name}' 已存在！"));
            return;
        }

        _tempPositionData[player.Name] = (name, posType, player.Name);
        player.SendSuccessMessage(GetString($"请使用稿子点击一个方块来设置 '{name}' 的{this.GetPositionName(posType)}位置！"));
        player.SendInfoMessage(GetString("注意：请确保点击的位置有足够的空间生成迷宫！"));
    }

    private void HandleGenerateMaze(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/maze play <名称> [大小]"));
            player.SendInfoMessage(GetString($"大小范围: {_config.MinSize} - {_config.MaxSize}"));
            return;
        }

        var name = args.Parameters[1];
        if (!_positionData.TryGetValue(name, out var position))
        {
            player.SendErrorMessage(GetString($"未找到位置 '{name}'，请先使用 /maze pos 设置位置"));
            return;
        }

        // 解析尺寸参数
        var size = _config.DefaultSize;

        if (args.Parameters.Count >= 3)
        {
            if (!int.TryParse(args.Parameters[2], out size) || size < _config.MinSize || size > _config.MaxSize)
            {
                player.SendErrorMessage(GetString($"大小必须是{_config.MinSize}到{_config.MaxSize}之间的整数！"));
                return;
            }
        }

        // 确保迷宫大小为奇数
        if (size % 2 == 0)
        {
            size++;
        }

        // 检查是否已有会话
        if (this._mazeSessions.ContainsKey(name))
        {
            player.SendInfoMessage(GetString($"位置 '{name}' 已存在迷宫，正在清除..."));
            this.ClearMazeArea(this._mazeSessions[name]);
            this._mazeSessions.Remove(name);
        }

        Task.Run(() => this.GenerateMazeForPlayer(player, name, position, size));
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

        if (!this._mazeSessions.TryGetValue(name, out var session))
        {
            player.SendErrorMessage(GetString($"未找到 '{name}' 的迷宫记录！请先使用 /maze play 生成迷宫"));
            return;
        }

        player.SendInfoMessage(GetString($"正在重置迷宫 '{name}'..."));

        // 使用相同的参数重新生成迷宫
        var position = _positionData[name];
        Task.Run(() => this.GenerateMazeForPlayer(player, name, position, session.Size));
    }

    private void HandleClearMaze(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 2)
        {
            player.SendErrorMessage(GetString("用法：/maze clear <名称>"));
            return;
        }

        var name = args.Parameters[1];

        if (!_positionData.ContainsKey(name))
        {
            player.SendErrorMessage(GetString($"未找到位置 '{name}'"));
            return;
        }

        if (this._mazeSessions.TryGetValue(name, out var session))
        {
            this.ClearMazeArea(session);
            this._mazeSessions.Remove(name);
            player.SendSuccessMessage(GetString($"已清除 '{name}' 的迷宫"));
        }
        else
        {
            var (startX, startY) = this.CalculateStartPosition(_positionData[name], 1, 1);
            this.ClearArea(startX, startY, _config.MaxSize * _config.CellSize, _config.MaxSize * _config.CellSize);
            player.SendSuccessMessage(GetString($"已尝试清除 '{name}' 的区域"));
        }
    }

    private void HandleListPositions(CommandArgs args)
    {
        var player = args.Player;

        if (_positionData.Count == 0)
        {
            player.SendInfoMessage(GetString("没有保存的迷宫位置。"));
            if (player.HasPermission("maze.admin"))
            {
                player.SendInfoMessage(GetString("使用 /maze pos <名称> <tl|bl|tr|br> 创建新位置"));
            }

            return;
        }

        player.SendInfoMessage(GetString("=== 已保存的迷宫位置 ==="));
        foreach (var (name, pos) in _positionData)
        {
            var hasMaze = this._mazeSessions.ContainsKey(name) ? GetString(" [已生成]") : "";
            var ownerInfo = player.HasPermission("maze.admin") ? $", 创建者={pos.Creator}" : "";
            player.SendInfoMessage(GetString($"- {name}: X={pos.X}, Y={pos.Y}, 对齐={pos.PositionType}{ownerInfo}{hasMaze}"));
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

        var tempEntry = _tempPositionData.FirstOrDefault(x => x.Value.name == name);
        if (!string.IsNullOrEmpty(tempEntry.Value.name))
        {
            _tempPositionData.Remove(tempEntry.Key);
            player.SendSuccessMessage(GetString($"已删除未完成的位置设置 '{name}'"));
            return;
        }

        if (!_positionData.ContainsKey(name))
        {
            player.SendErrorMessage(GetString($"未找到位置 '{name}'"));
            return;
        }

        if (this._mazeSessions.TryGetValue(name, out var session))
        {
            this.ClearMazeArea(session);
            this._mazeSessions.Remove(name);
        }

        _positionData.Remove(name);
        this.SavePositionData();
        player.SendSuccessMessage(GetString($"已删除位置 '{name}'"));
    }

    private void HandleConfig(CommandArgs args)
    {
        var player = args.Player;

        if (args.Parameters.Count < 3)
        {
            player.SendErrorMessage(GetString("用法：/maze config <键> <值>"));
            player.SendInfoMessage(GetString("可用键: defaultsize, minsize, maxsize, cellsize, framedelay"));
            return;
        }

        var key = args.Parameters[1].ToLower();
        var value = args.Parameters[2];

        try
        {
            switch (key)
            {
                case "defaultsize":
                    _config.DefaultSize = Math.Max(_config.MinSize, Math.Min(Convert.ToInt32(value), _config.MaxSize));
                    break;
                case "minsize":
                    _config.MinSize = Math.Max(5, Convert.ToInt32(value));
                    if (_config.DefaultSize < _config.MinSize)
                    {
                        _config.DefaultSize = _config.MinSize;
                    }

                    break;
                case "maxsize":
                    _config.MaxSize = Math.Max(_config.MinSize, Convert.ToInt32(value));
                    if (_config.DefaultSize > _config.MaxSize)
                    {
                        _config.DefaultSize = _config.MaxSize;
                    }

                    break;
                case "cellsize":
                    _config.CellSize = Math.Max(3, Math.Min(Convert.ToInt32(value), 10));
                    break;
                case "framedelay":
                    _config.FrameDelay = Math.Max(50, Math.Min(Convert.ToInt32(value), 2000));
                    break;
                default:
                    player.SendErrorMessage($"未知的配置项：{key}");
                    return;
            }

            this.SaveConfig();
            player.SendSuccessMessage(GetString($"已设置配置：{key}={value}"));
        }
        catch (Exception ex)
        {
            player.SendErrorMessage("设置配置时发生错误：" + ex.Message);
        }
    }

    private async Task GenerateMazeForPlayer(TSPlayer player, string name, PositionData position, int size)
    {
        try
        {
            player.SendInfoMessage(GetString($"正在生成 {size}x{size} 的迷宫 (单元格大小: {_config.CellSize}x{_config.CellSize})..."));

            // 计算起始位置
            var totalWidth = size * _config.CellSize;
            var totalHeight = size * _config.CellSize;
            var (startX, startY) = this.CalculateStartPosition(position, totalWidth, totalHeight);

            if (startX < 0 || startY < 0 || startX + totalWidth >= Main.maxTilesX || startY + totalHeight >= Main.maxTilesY)
            {
                player.SendErrorMessage(GetString("错误：迷宫位置超出世界边界！"));
                return;
            }

            // 预清理区域并绘制背景
            this.ClearArea(startX, startY, totalWidth, totalHeight);
            this.DrawBackground(startX, startY, totalWidth, totalHeight);

            // 使用DFS算法逐步生成迷宫
            var session = new MazeSession
            {
                Name = name,
                StartX = startX,
                StartY = startY,
                Size = size,
                CellSize = _config.CellSize,
                GeneratedTime = DateTime.Now,
                GeneratedBy = player.Name
            };

            this._mazeSessions[name] = session;

            // 逐步生成迷宫
            await this.GenerateMazeDFSAnimated(player, startX, startY, size, _config.CellSize);

            player.SendSuccessMessage(GetString($"迷宫 '{name}' 生成完成！"));
            player.SendInfoMessage(GetString($"使用 /maze reset {name} 重新生成随机迷宫"));
            player.SendInfoMessage(GetString($"使用 /maze clear {name} 清除迷宫"));
        }
        catch (Exception ex)
        {
            player.SendErrorMessage(GetString($"生成迷宫时发生错误: {ex.Message}"));
            TShock.Log.Error(GetString($"[MazeGenerator] 生成迷宫错误: {ex}"));
        }
    }

    private (int startX, int startY) CalculateStartPosition(PositionData position, int width, int height)
    {
        return position.PositionType switch
        {
            "tl" => (position.X, position.Y), // 左上角
            "bl" => (position.X, position.Y - height + 1), // 左下角
            "tr" => (position.X - width + 1, position.Y), // 右上角
            "br" => (position.X - width + 1, position.Y - height + 1), // 右下角
            _ => (position.X, position.Y) // 默认左上角
        };
    }

    private async Task GenerateMazeDFSAnimated(TSPlayer player, int startX, int startY, int size, int cellSize)
    {
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
        await Task.Delay(_config.FrameDelay);

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
                await Task.Delay(_config.FrameDelay / 2);

                this.DrawCell(startX, startY, nextX, nextY, cellSize, false);
                await Task.Delay(_config.FrameDelay / 2);

                // 可视化：更新当前路径显示
                foreach (var (px, py) in pathTiles)
                {
                    this.DrawCell(startX, startY, px, py, cellSize, false);
                }

                await Task.Delay(_config.FrameDelay / 4);

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
                        // 设置背景墙为黑色
                        Main.tile[tileX, tileY].wall = (ushort) BackgroundWall;
                        WorldGen.paintWall(tileX, tileY, (byte) BackgroundPaint, true);

                        // 初始化为墙壁
                        Main.tile[tileX, tileY].active(true);
                        Main.tile[tileX, tileY].type = (ushort) MazeWallTile;
                        Main.tile[tileX, tileY].frameX = -1;
                        Main.tile[tileX, tileY].frameY = -1;
                    }
                }
            }
        }

        // 初始更新整个区域
        this.UpdateRegion(startX, startY, width, height);
    }

    private void DrawCell(int startX, int startY, int cellX, int cellY, int cellSize, bool isWall)
    {
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
                            Main.tile[tileX, tileY].type = (ushort) MazeWallTile;
                        }
                        else
                        {
                            // 清除路径
                            Main.tile[tileX, tileY].active(false);
                        }
                    }
                }
            }
        }

        // 只更新当前单元格区域，优化带宽
        this.UpdateRegion(
            startX + (cellX * cellSize),
            startY + (cellY * cellSize),
            cellSize,
            cellSize);
    }

    private void UpdateRegion(int x, int y, int width, int height)
    {
        const int maxChunkSize = 15; // 减小块大小以减少带宽使用

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

    private string GetPositionName(string posType)
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

    private static string GetString(string text)
    {
        return text;
    }
}

// 数据类
public class PositionData
{
    public int X { get; set; }
    public int Y { get; set; }
    public string PositionType { get; set; } = "tl";
    public string Creator { get; set; } = string.Empty;
}

public class MazeSession
{
    public string Name { get; set; } = string.Empty;
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int Size { get; set; } 
    public int CellSize { get; set; } = 5; 
    public DateTime GeneratedTime { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
}

public class PluginConfig
{
    public int DefaultSize { get; set; } = 15; 
    public int MinSize { get; set; } = 7; 
    public int MaxSize { get; set; } = 31; 
    public int CellSize { get; set; } = 5; 
    public int FrameDelay { get; set; } = 200; 
}