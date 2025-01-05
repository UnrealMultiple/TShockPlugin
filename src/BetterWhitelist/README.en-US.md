# BetterWhitelist

- 作者: 豆沙，肝帝熙恩修,Cai改
- Source: [gitee](https://gitee.com/Crafty/BetterWhitelist)
- Add player names to the whitelist, only players on the whitelist can enter the game.

## Commands

| 语法                | Permission | 说明                                |
| ----------------- | ---------- | --------------------------------- |
| `/bwl help`       | `bwl.use`  | Show help information             |
| `/bwl add {name}` | `bwl.use`  | Add player name to the whitelist  |
| `/bwl del {name}` | `bwl.use`  | Remove player from the whitelist  |
| `/bwl list`       | `bwl.use`  | Show all players on the whitelist |
| `/bwl true`       | `bwl.use`  | Enable the plugin                 |
| `/bwl false`      | `bwl.use`  | Disable the plugin                |
| `/bwl reload`     | `bwl.use`  | Reload the plugin                 |

## Config

> Configuration file location：tshock/BetterWhitelist.en-US.json

```json5
{
  "WhitePlayers": [], // Whitelisted players
  "Enable": false, // Plugin switch
  "NotInWhiteList": "你不在服务器白名单中！" // Message when not on the whitelist
}
```

## 更新日志

```
v2.6.1
准备更新TS 5.2.1,修正文档，初始配置内容更改
v2.6
添加英文翻译
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
