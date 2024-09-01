# AutoTeamPlus

- Authors: 十七，肝帝熙恩
- Source: No
- 自动分配一个组的玩家到特定队伍

## Commands

| Command          | permission |          Details          |
|------------------| :-----------------: | :------: |
| /autoteam or /at | autoteam.toggle  |   Toggle the automatic team assignment feature.   |
| No               | noautoteam  |   Having this permission will not automatically assign you to a team.   |

## Config
> Configuration file location：tshock/AutoTeamPlus.json
```json
{
  "组的队伍": { //Group -> Team
    "组名字": "队伍名称中文或English",//本行和下面两行均可改
    "default": "red",
    "admin": "green"
  },
  "开启插件": true //Enable
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
