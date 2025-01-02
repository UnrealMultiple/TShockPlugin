# AutoTeamPlus

- Authors: 十七，肝帝熙恩
- Source: None
- Automatically assign players from a group to a specific team

## Commands

| Command          |   Permission    |                               Details                               |
|------------------|:---------------:|:-------------------------------------------------------------------:|
| /autoteam or /at | autoteam.toggle |            Toggle the automatic team assignment feature.            |
| None             |   noautoteam    | Having this permission will not automatically assign you to a team. |

## Config
> Configuration file location：tshock/AutoTeam.en-US.json
- Team Name Reference：

| 中文  | English |
|-----|---------|
| 无队伍 | none    |
| 红队  | red     |
| 绿队  | green   |
| 蓝队  | blue    |
| 黄队  | yellow  |
| 粉队  | pink    |

- Configuration Example
```json5
{
  "Enable": true,
  "GroupTemp": {
    "guest": "pink",
    "default": "蓝队",
    "owner": "红队",
    "admin": "green",
    "vip": "none"
  }
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
