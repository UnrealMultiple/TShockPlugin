# BetterWhitelist

- Authors: 豆沙,肝帝熙恩,Cai
- Source: [gitee](https://gitee.com/Crafty/BetterWhitelist)
- Add player names to the whitelist, only players on the whitelist can enter the game.

## Commands

| Command           | Permission |              Details               |
|-------------------|:----------:|:----------------------------------:|
| `/bwl help`       | `bwl.use`  |       Show help information        |
| `/bwl add {name}` | `bwl.use`  |  Add player name to the whitelist  |
| `/bwl del {name}` | `bwl.use`  |  Remove player from the whitelist  |
| `/bwl list`       | `bwl.use`  | 	Show all players on the whitelist |
| `/bwl true`       | `bwl.use`  |         Enable the plugin          |
| `/bwl false`      | `bwl.use`  |        	Disable the plugin         |

## Config
> Configuration file location：tshock/BetterWhitelist.en-US.json
```json5
{
  "WhitePlayers": [], // Whitelisted players
  "Enable": false, // Plugin switch
  "NotInWhiteList": "你不在服务器白名单中！" // Message when not on the whitelist
}
```

## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love
