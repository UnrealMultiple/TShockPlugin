using System;

namespace MazeGenerator.Models;

public class LeaderboardEntry
{
    public string PlayerName { get; set; } = string.Empty;
    public string MazeName { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime RecordDate { get; set; }
    public int MazeSize { get; set; }
}