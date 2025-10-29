using Terraria;
using TShockAPI;
using MazeGenerator.Models;
using Microsoft.Xna.Framework;

namespace MazeGenerator.Services;

public class MazeGameManager : IDisposable
{
    private readonly Dictionary<string, PlayerGameData> _activePlayers = new ();
    private readonly Dictionary<string, List<string>> _waitingQueues = new ();
    private readonly Lock _lock = new ();

    public void Dispose()
    {
        lock (this._lock)
        {
            this._activePlayers.Clear();
            this._waitingQueues.Clear();
        }
    }

    public void Update()
    {
        this.CheckPlayerBoundaries();
    }

    public void HandlePlayerUpdate(GetDataHandlers.PlayerUpdateEventArgs args)
    {
        var player = TShock.Players[args.PlayerId];
        if (player == null)
        {
            return;
        }

        this.CheckPlayerGameStatus(player);
    }

    public void HandlePlayerLeave(string playerName)
    {
        lock (this._lock)
        {
            this._activePlayers.Remove(playerName);

            foreach (var queue in this._waitingQueues.Where(q => q.Value.Contains(playerName)))
            {
                queue.Value.Remove(playerName);
            }
        }
    }

    public void LeaveAllPlayersFromMaze(string mazeName)
    {
        lock (this._lock)
        {
            var playersToRemove = this._activePlayers
                .Where(kvp => kvp.Value.MazeName == mazeName)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var playerName in playersToRemove)
            {
                var player = TShock.Players.FirstOrDefault(p => p?.Name == playerName);
                if (player != null)
                {
                    player.SendInfoMessage(GetString($"迷宫 '{mazeName}' 正在重置，你已自动退出游戏"));
                }

                this._activePlayers.Remove(playerName);
            }

            if (this._waitingQueues.ContainsKey(mazeName))
            {
                this._waitingQueues.Remove(mazeName);
            }
        }
    }

    public bool JoinGame(TSPlayer player, string mazeName, MazeSession session)
    {
        lock (this._lock)
        {
            if (this._activePlayers.ContainsKey(player.Name))
            {
                var currentGame = this._activePlayers[player.Name];
                if (currentGame.MazeName == mazeName)
                {
                    player.SendErrorMessage(GetString($"你已经加入了迷宫 '{mazeName}' 的游戏！"));
                }
                else
                {
                    player.SendErrorMessage(GetString($"你已经在迷宫 '{currentGame.MazeName}' 的游戏中，请先使用 /maze leave 退出当前游戏！"));
                }

                return false;
            }

            var currentQueue = this.GetPlayerQueue(player.Name);
            if (!string.IsNullOrEmpty(currentQueue))
            {
                if (currentQueue == mazeName)
                {
                    player.SendErrorMessage(GetString($"你已经在迷宫 '{mazeName}' 的等待队列中！"));
                }
                else
                {
                    player.SendErrorMessage(GetString($"你已经在迷宫 '{currentQueue}' 的等待队列中，请先退出当前队列！"));
                }

                return false;
            }

            if (session.IsGenerating)
            {
                if (!this._waitingQueues.ContainsKey(mazeName))
                {
                    this._waitingQueues[mazeName] = new List<string>();
                }

                this._waitingQueues[mazeName].Add(player.Name);
                player.SendInfoMessage(GetString($"迷宫正在生成中，你已被添加到等待队列。当前位置: {this._waitingQueues[mazeName].Count}"));
                return true;
            }

            return this.StartGameForPlayer(player, mazeName, session);
        }
    }

    private string GetPlayerQueue(string playerName)
    {
        foreach (var queue in this._waitingQueues)
        {
            if (queue.Value.Contains(playerName))
            {
                return queue.Key;
            }
        }

        return string.Empty;
    }

    private bool StartGameForPlayer(TSPlayer player, string mazeName, MazeSession session)
    {
        var spawnX = session.Entrance.startX;
        var spawnY = session.Entrance.startY;

        var gameData = new PlayerGameData
        {
            PlayerName = player.Name,
            MazeName = mazeName,
            JoinTime = DateTime.Now,
            IsPlaying = true,
            HasStarted = false,
            SpawnPoint = (spawnX, spawnY)
        };

        this._activePlayers[player.Name] = gameData;

        this.TeleportToMazeStart(player, session);
        player.SendSuccessMessage(GetString($"已加入迷宫游戏 '{mazeName}'！找到出口即可完成游戏。"));
        player.SendInfoMessage(GetString("使用 /maze leave 可以退出游戏。"));

        return true;
    }

    public void NotifyMazeReady(string mazeName, MazeSession session)
    {
        lock (this._lock)
        {
            if (this._waitingQueues.TryGetValue(mazeName, out var queue) && queue.Count > 0)
            {
                foreach (var playerName in queue.ToList())
                {
                    var player = TShock.Players.FirstOrDefault(p => p?.Name == playerName);
                    if (player != null)
                    {
                        if (!this._activePlayers.ContainsKey(playerName))
                        {
                            this.StartGameForPlayer(player, mazeName, session);
                        }

                        queue.Remove(playerName);
                    }
                }
            }
        }
    }

    public void LeaveGame(TSPlayer player)
    {
        lock (this._lock)
        {
            var leftGame = false;

            if (this._activePlayers.TryGetValue(player.Name, out var gameData))
            {
                this._activePlayers.Remove(player.Name);
                this.TeleportToSpawn(player);
                player.SendSuccessMessage(GetString($"已退出迷宫游戏 '{gameData.MazeName}'。"));
                leftGame = true;
            }

            var queueName = this.GetPlayerQueue(player.Name);
            if (!string.IsNullOrEmpty(queueName))
            {
                if (this._waitingQueues.TryGetValue(queueName, out var queue))
                {
                    queue.Remove(player.Name);
                    player.SendSuccessMessage(GetString($"已从迷宫 '{queueName}' 的等待队列中移除。"));
                    leftGame = true;
                }
            }

            if (!leftGame)
            {
                player.SendErrorMessage(GetString("你不在任何迷宫游戏或等待队列中！"));
            }
        }
    }

    private void CheckPlayerCompletion(TSPlayer player, int x, int y)
    {
        lock (this._lock)
        {
            if (this._activePlayers.TryGetValue(player.Name, out var gameData) && gameData.IsPlaying)
            {
                var session = MazeGenerator.Instance.MazeBuilder.GetSession(gameData.MazeName);
                if (session != null && this.IsAtExit(x, y, session.Exit))
                {
                    this.CompleteGame(player, gameData, session);
                }
            }
        }
    }

    private void CompleteGame(TSPlayer player, PlayerGameData gameData, MazeSession session)
    {
        gameData.IsPlaying = false;
        gameData.FinishTime = DateTime.Now;

        var duration = gameData.Duration.GetValueOrDefault();
        player.SendSuccessMessage(GetString($"恭喜！你完成了迷宫 '{gameData.MazeName}'！"));
        player.SendSuccessMessage(GetString($"用时: {duration:mm\\:ss\\.ff}"));

        MazeGenerator.Instance.Leaderboard.AddRecord(new LeaderboardEntry
        {
            PlayerName = player.Name,
            MazeName = gameData.MazeName,
            Duration = duration,
            RecordDate = DateTime.Now,
            MazeSize = session.Size
        });

        this._activePlayers.Remove(player.Name);
    }

    private void CheckPlayerBoundaries()
    {
        lock (this._lock)
        {
            foreach (var kvp in this._activePlayers.ToList())
            {
                var playerName = kvp.Key;
                var gameData = kvp.Value;

                var player = TShock.Players.FirstOrDefault(p => p?.Name == playerName);
                if (player == null || !player.Active)
                {
                    this._activePlayers.Remove(playerName);
                    continue;
                }

                var session = MazeGenerator.Instance.MazeBuilder.GetSession(gameData.MazeName);
                if (session == null)
                {
                    continue;
                }

                if (this.IsOutsideMazeArea((int) (player.TPlayer.position.X / 16), (int) (player.TPlayer.position.Y / 16), session))
                {
                    this.TeleportToMazeStart(player, session);
                    player.SendWarningMessage(GetString("你已离开迷宫区域，已被传送回起点！"));
                }
            }
        }
    }

    private void CheckPlayerGameStatus(TSPlayer player)
    {
        lock (this._lock)
        {
            if (this._activePlayers.TryGetValue(player.Name, out var gameData) && gameData.IsPlaying)
            {
                var x = (int) (player.X / 16);
                var y = (int) (player.Y / 16);

                this.CheckPlayerCompletion(player, x, y);

                if (!gameData.HasStarted && this.IsInMazeArea(x, y, gameData.MazeName))
                {
                    gameData.HasStarted = true;
                    gameData.JoinTime = DateTime.Now;
                    player.SendInfoMessage(GetString("计时开始！"));
                }
            }
        }
    }

    private bool IsInMazeArea(int x, int y, string mazeName)
    {
        var session = MazeGenerator.Instance.MazeBuilder.GetSession(mazeName);
        if (session == null)
        {
            return false;
        }

        return x >= session.StartX && x < session.StartX + (session.Size * session.CellSize) &&
               y >= session.StartY && y < session.StartY + (session.Size * session.CellSize);
    }

    private bool IsOutsideMazeArea(int x, int y, MazeSession session)
    {
        var config = Config.Instance;
        return x < session.StartX - config.BoundaryCheckRange ||
               x >= session.StartX + (session.Size * session.CellSize) + config.BoundaryCheckRange ||
               y < session.StartY - config.BoundaryCheckRange ||
               y >= session.StartY + (session.Size * session.CellSize) + config.BoundaryCheckRange;
    }

    private bool IsAtExit(int x, int y, (int endX, int endY) exit)
    {
        return Math.Abs(x - exit.endX) <= 2 && Math.Abs(y - exit.endY) <= 2;
    }

    private void TeleportToMazeStart(TSPlayer player, MazeSession session)
    {
        var worldX = session.Entrance.startX * 16f;
        var worldY = session.Entrance.startY * 16f;

        player.Teleport(worldX, worldY);
        player.TPlayer.velocity = Vector2.Zero;

    }

    private void TeleportToSpawn(TSPlayer player)
    {
        if (player.TPlayer == null)
        {
            return;
        }

        var spawnX = player.TPlayer.SpawnX;
        var spawnY = player.TPlayer.SpawnY;

        if (spawnX < 0 || spawnY < 0)
        {
            spawnX = Main.spawnTileX;
            spawnY = Main.spawnTileY;
        }

        float worldX = (spawnX * 16) + 8;
        float worldY = (spawnY * 16) + 8;

        player.Teleport(worldX, worldY);
        player.TPlayer.velocity = Vector2.Zero;
    }
}