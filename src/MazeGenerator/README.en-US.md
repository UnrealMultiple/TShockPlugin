# Maze Generator

- Author: Eustia
- Source: This repository
- Create randomly generated mazes of any size at any location in the game world
- Support one-click reset and rebuild of existing mazes
- View the complete viable path through mazes
- Configurable maze walls, background walls, and paint colors
- Join maze challenges with automatic timing and leaderboard system

## Commands

| Syntax | Alias | Permission | Description |
|--------|-------|------------|-------------|
| /maze help | none | maze.generate | Show help information |
| /maze build <name> [size] | None | maze.generate | Generate a maze of specified size (maze side length = size x cell) |
| /maze join <name> | none | maze.generate | Join a maze game |
| /maze leave | none | maze.generate | Leave current maze game |
| /maze list | none | maze.generate | List all saved maze locations |
| /maze rank [page] | none | maze.generate | View leaderboard |
| /maze del <name> | none | maze.admin | Delete position and clear blocks |
| /maze path <name> | none | maze.admin | Show/hide maze path |
| /maze reset <name> | none | maze.admin | Reset specified maze |
| /maze pos <name> <tl\|bl\|tr\|br> | none | maze.admin | Set maze position（top-left、top-right、bottom-left、bottom-right） |
## Notes
The input "maze size" is measured in cells.
The actual occupied area = (maze size × cell size)².
Make sure the generation area has enough space and follows the minimum/maximum limits.
## Configuration
> Configuration file location: tshock/MazeGenerator/MazeConfig.json
```json5
{
  "DefaultSize": 30,
  "MinSize": 5,
  "MaxSize": 60,
  "CellSize": 5,
  "MazeWallTile": 267,
  "BackgroundWall": 155,
  "BackgroundPaint": 25,
  "PathPaint": 13,
  "BoundaryCheckRange": 50,
  "LeaderboardPageSize": 10
}
```
## Feedback
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
