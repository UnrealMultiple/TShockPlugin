using Newtonsoft.Json;

namespace MazeGenerator.Models;

public class MazeSession
{
    public string Name { get; set; } = string.Empty;
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int Size { get; set; }
    public int CellSize { get; set; } = 5;
    public DateTime GeneratedTime { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public bool IsGenerating { get; set; }
    public (int startX, int startY) Entrance { get; set; }
    public (int endX, int endY) Exit { get; set; }

    [JsonIgnore] public int[,]? MazeData { get; set; }

    [JsonProperty("MazeData")] public string? MazeDataBase64 { get; set; }
}