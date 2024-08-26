# Betterwhitelist better whitelist

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Doudan, Liver Emperor Xixiu Xiu, CAI change
- - 出处: [gitee](https://gitee.com/Crafty/BetterWhitelist) 
- Add the player's name to the whitelist, players who only in the white list can enter the game

## Update log

```
暂无
```

## instruction

|grammar|Authority|illustrate|
|-------------------|-----------|-------------|
|`/bwl help`|`bwl.use`|Display help information|
|`/bwl add {name}`|`bwl.use`|Add the player name to the white list|
|`/bwl del {name}`|`bwl.use`|Move the player out of the whitelist|
|`/bwl list`|`bwl.use`|Show all the players on the white list|
|`/bwl true`|`bwl.use`|Enable plug -in|
|`/bwl false`|`bwl.use`|Turn off the plug -in|
|`/bwl reload`|`bwl.use`|Heavy load plug -in|

## Configuration

> Configuration file location: TSHOCK/BETTERWHITELIST/Config.json

```json
{
   "白名单玩家": [],
   "插件开关": false,
   "连接时不在白名单提示": "你不在服务器白名单中！" 
}
```

## feedback

- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love