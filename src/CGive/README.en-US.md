# CGive 

- Author: Leader
- Source: None
- Execute commands offline; commands are automatically executed when the player logs into the game.


## Rest API

| Command       | Permission |                     Details                      |
|---------------|:----------:|:------------------------------------------------:|
| /getWarehouse |    None    | Get detailed information about the /give command |

## Commands

| Command                              |  Permission   |               Details               |
|--------------------------------------|:-------------:|:-----------------------------------:|
| `/cgive personal [command] [target]` | `cgive.admin` | Add a command for a specific player |
| `/cgive all [executor] [command]`    | `cgive.admin` |   Offline command for all players   |
| `/cgive list`                        | `cgive.admin` |      List of offline commands       |
| `/cgive del [id]`                    | `cgive.admin` |      Delete an offline command      |
| `/cgive reset`                       | `cgive.admin` |       Reset offline commands        |


## Config

```json5 
None
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love