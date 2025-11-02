namespace MazeGenerator.Models;

public class PositionData
{
    public int X { get; set; }
    public int Y { get; set; }
    public string PositionType { get; set; } = "tl";
    public string Creator { get; set; } = string.Empty;
}