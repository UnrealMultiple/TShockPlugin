# CreateSpawn

- author: 少司命
- source: none
- Spawn your copied building at the spawn point when creating the map

## Commands

| Command                |    Permission     |           Description           |
|------------------------|:-----------------:|:-------------------------------:|
| /cb set 1              | create.build.copy |     Select top-left corner      |
| /cb set 2              | create.build.copy |   Select bottom-right corner    |
| /cb save [name]        | create.build.copy |      Save a building clip       |
| /cb spawn [name]       | create.build.copy |        Spawn a building         |
| /cb back               | create.build.copy |      Restore covered tiles      |
| /cb list               | create.build.copy |      List saved buildings       |
| /cb zip                | create.build.copy | Backup and clear building files |
| /cb auto add [name]    | create.build.copy |  Add build to auto-spawn list   |
| /cb auto remove [name] | create.build.copy |   Remove from auto-spawn list   |
| /cb auto list          | create.build.copy |      Show auto-spawn list       |
| /cb auto clear         | create.build.copy |      Clear auto-spawn list      |

## Configuration

> Configuration file location: tshock/CreateSpawn.json

```json5
{
  "CentreX": 0,
  "CountY": 0,
  "AdjustX": -21,
  "AdjustY": 50,
  "AutoSpawnBuilds": [
    "Spawn",
    "Lobby"
  ]
}
```

Notes:

- The plugin no longer ships with an embedded default building.
- New-world auto-spawn only uses names listed in `AutoSpawnBuilds`.

## Feedback

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
