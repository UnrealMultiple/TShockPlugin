using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using Terraria.ID;

namespace MazeGenerator.Models;

[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "MazeGenerator/MazeConfig";

    [LocalizedPropertyName(CultureType.Chinese, "默认迷宫大小")]
    [LocalizedPropertyName(CultureType.English, "Default Size")]
    public int DefaultSize { get; set; } = 30;

    [LocalizedPropertyName(CultureType.Chinese, "最小迷宫大小")]
    [LocalizedPropertyName(CultureType.English, "Minimum Size")]
    public int MinSize { get; set; } = 5;

    [LocalizedPropertyName(CultureType.Chinese, "最大迷宫大小")]
    [LocalizedPropertyName(CultureType.English, "Maximum Size")]
    public int MaxSize { get; set; } = 60;

    [LocalizedPropertyName(CultureType.Chinese, "单元格大小")]
    [LocalizedPropertyName(CultureType.English, "Cell Size")]
    public int CellSize { get; set; } = 5;

    [LocalizedPropertyName(CultureType.Chinese, "迷宫墙壁图格ID")]
    [LocalizedPropertyName(CultureType.English, "Maze Wall Tile ID")]
    public int MazeWallTile { get; set; } = TileID.DiamondGemspark;

    [LocalizedPropertyName(CultureType.Chinese, "背景墙壁ID")]
    [LocalizedPropertyName(CultureType.English, "Background Wall ID")]
    public int BackgroundWall { get; set; } = WallID.DiamondGemspark;

    [LocalizedPropertyName(CultureType.Chinese, "背景油漆ID")]
    [LocalizedPropertyName(CultureType.English, "Background Paint ID")]
    public int BackgroundPaint { get; set; } = PaintID.BlackPaint;

    [LocalizedPropertyName(CultureType.Chinese, "路径显示油漆ID")]
    [LocalizedPropertyName(CultureType.English, "Path Paint ID")]
    public int PathPaint { get; set; } = PaintID.DeepRedPaint;

    [LocalizedPropertyName(CultureType.Chinese, "游戏区域边界检查范围")]
    [LocalizedPropertyName(CultureType.English, "Game Area Boundary Check Range")]
    public int BoundaryCheckRange { get; set; } = 50;

    [LocalizedPropertyName(CultureType.Chinese, "排行榜显示每页记录数")]
    [LocalizedPropertyName(CultureType.English, "Leaderboard Records Per Page")]
    public int LeaderboardPageSize { get; set; } = 10;
}