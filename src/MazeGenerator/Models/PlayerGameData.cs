namespace MazeGenerator.Models;

public class PlayerGameData
{
    public string PlayerName { get; set; } = string.Empty;
    public string MazeName { get; set; } = string.Empty;
    public DateTime JoinTime { get; set; }
    public DateTime? FinishTime { get; set; }
    public bool IsPlaying { get; set; }
    public bool HasStarted { get; set; }
    public (int x, int y) SpawnPoint { get; set; }

    public TimeSpan? Duration => this.FinishTime.HasValue ? this.FinishTime.Value - this.JoinTime : null;
}