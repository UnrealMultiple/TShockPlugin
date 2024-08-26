# BetterWhitelist

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Authors: Dousha, Gan Di Xi Enxiu, Cai Gai,
- - 出处: [gitee](https://gitee.com/Crafty/BetterWhitelist) 
- Add the player's name to the white list, and only players in the white list can enter the game.

## Update log

```
暂无
```

## instruction

|grammar|limit of authority|explain|
|-------------------|-----------|-------------|
|`/bwl help`|`bwl.use`|Display help information|
|`/bwl add {name}`|`bwl.use`|Add the player name to the white list|
|`/bwl del {name}`|`bwl.use`|Remove players from the white list|
|`/bwl list`|`bwl.use`|Show all players on the white list|
|`/bwl true`|`bwl.use`|Enable plug-in|
|`/bwl false`|`bwl.use`|Close plug-in|
|`/bwl reload`|`bwl.use`|Overloaded plug-in|

## deploy

> Configuration file location: tshock/betterwhitelist/config.json.

```json
{
   "白名单玩家": [],
   "插件开关": false,
   "连接时不在白名单提示": "你不在服务器白名单中！" 
}
```

## feedback

- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.