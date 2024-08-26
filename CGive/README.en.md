# CGive offline command

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Leader
- Source: None
- Execute the command offline, and the player automatically executes the command when logging in to the game.

## Update log

```
V1.0.0.1
- 优化简化部分代码，完善卸载函数
```

## Rest API

|path|limit of authority|explain|
| ------------- |:--:|:----------:|
|/getWarehouse|without|Query /give command details|

## instruction

|grammar|limit of authority|explain|
| ----------------------------- |:---------:|:--------------------:|
|/cgive personal [command] [target]|cgive.admin|Add a command for a player.|
|/cgive all [performer] [command]|cgive.admin|All players offline command|
|/cgive list|cgive.admin|Offline command list|
|/cgive del [id]|cgive.admin|Delete offline command|
|/cgive reset|cgive.admin|Reset offline command|

## deploy

```json
暂无
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.