# CGive 离线命令

- Author: Leader
- Source: None
- Execute commands offline; commands are automatically executed when the player logs into the game.

## Rest API

| 路径            | Permission |                        说明                        |
| ------------- | :--------: | :----------------------------------------------: |
| /getWarehouse |    None    | Get detailed information about the /give command |

## Commands

| 语法                                   |   Permission  |                  说明                 |
| ------------------------------------ | :-----------: | :---------------------------------: |
| `/cgive personal [command] [target]` | `cgive.admin` | Add a command for a specific player |
| `/cgive all [executor] [command]`    | `cgive.admin` |   Offline command for all players   |
| `/cgive list`                        | `cgive.admin` |       List of offline commands      |
| `/cgive del [id]`                    | `cgive.admin` |      Delete an offline command      |
| `/cgive reset`                       | `cgive.admin` |        Reset offline commands       |

## Config

```json5
None
```

## 更新日志

```
v1.0.0.4
- i18n和README_EN.md
v1.0.0.3
- i18n预备
v1.0.0.2
- 完善rest卸载函数
V1.0.0.1
- 优化简化部分代码，完善卸载函数
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
