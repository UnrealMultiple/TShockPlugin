using Newtonsoft.Json;
using MazeGenerator.Models;
using TShockAPI;

namespace MazeGenerator.Services;

public class LeaderboardService : IDisposable
{
    private List<LeaderboardEntry> _leaderboard = new ();
    private static readonly string LeaderboardPath = Path.Combine(TShock.SavePath, "MazeGenerator", "leaderboard.json");

    public void Initialize()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(LeaderboardPath)!);
        this.LoadLeaderboard();
    }

    public void Dispose()
    {
        this.SaveLeaderboard();
    }

    public void Reload()
    {
        this.LoadLeaderboard();
    }

    private void LoadLeaderboard()
    {
        if (File.Exists(LeaderboardPath))
        {
            try
            {
                this._leaderboard = JsonConvert.DeserializeObject<List<LeaderboardEntry>>(
                    File.ReadAllText(LeaderboardPath)) ?? new List<LeaderboardEntry>();
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[MazeGenerator] 加载排行榜失败: {ex.Message}");
                this._leaderboard = new List<LeaderboardEntry>();
            }
        }
    }

    private void SaveLeaderboard()
    {
        try
        {
            File.WriteAllText(LeaderboardPath, JsonConvert.SerializeObject(this._leaderboard, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError($"[MazeGenerator] 保存排行榜失败: {ex.Message}");
        }
    }

    public void AddRecord(LeaderboardEntry entry)
    {
        var existingRecord = this._leaderboard
            .FirstOrDefault(x => x.PlayerName == entry.PlayerName && x.MazeName == entry.MazeName);

        if (existingRecord != null)
        {
            if (entry.Duration < existingRecord.Duration)
            {
                this._leaderboard.Remove(existingRecord);
                this._leaderboard.Add(entry);
                TShock.Log.ConsoleInfo($"[MazeGenerator] 更新记录: {entry.PlayerName} 在 {entry.MazeName} 的新时间 {entry.Duration} 替换了旧时间 {existingRecord.Duration}");
            }
            else
            {
                TShock.Log.ConsoleInfo($"[MazeGenerator] 忽略较慢记录: {entry.PlayerName} 在 {entry.MazeName} 的时间 {entry.Duration} 比现有记录 {existingRecord.Duration} 慢");
                return;
            }
        }
        else
        {
            this._leaderboard.Add(entry);
        }

        this._leaderboard = this._leaderboard.OrderBy(x => x.Duration).ToList();
        this.SaveLeaderboard();
    }

    public (List<LeaderboardEntry> page, int totalPages, int playerRank) GetLeaderboardPage(int page, string? playerName = null)
    {
        var config = Config.Instance;
        var pageSize = config.LeaderboardPageSize;

        var allRecords = this._leaderboard.OrderBy(x => x.Duration).ToList();
        var totalPages = (int) Math.Ceiling((double) allRecords.Count / pageSize);

        page = Math.Max(1, Math.Min(page, totalPages));
        var skip = (page - 1) * pageSize;

        var pageRecords = allRecords.Skip(skip).Take(pageSize).ToList();

        var playerRank = -1;
        if (!string.IsNullOrEmpty(playerName))
        {
            playerRank = allRecords.FindIndex(x => x.PlayerName == playerName) + 1;
        }

        return (pageRecords, totalPages, playerRank);
    }
}